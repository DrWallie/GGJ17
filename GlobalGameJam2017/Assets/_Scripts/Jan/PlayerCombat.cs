using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCombat : NetworkBehaviour {

    public DoubleWeapon[] weapons;
    public int equippedWeapon;
    private bool mayFire = true;

    private void Awake()
    {
        InitializeFirstWeapon();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0f)
            SwitchStances(wheel);

        if (!mayFire)
            return;

        if (Input.GetButton("Fire1"))
            UseWeapon();

        //F3 for delegates unique weapon arts
    }

    #region Equip / Disequip Weapon

    private void InitializeFirstWeapon()
    {
        EquipWeapon(true);
    }

    public void EquipWeapon(DoubleWeapon dW)
    {
        EquipWeapon(false);
        for(int i = 0; i < weapons.Length; i++)
            if(weapons[i] == dW)
            {
                equippedWeapon = i;
                break;
            }
        EquipWeapon(true);
    }

    public void EquipWeapon(int numIndex)
    {
        EquipWeapon(false);
        equippedWeapon = numIndex;
        EquipWeapon(true);
    }

    /// <summary>
    /// dont directly use this, use one of the above
    /// </summary>
    private void EquipWeapon(bool equip)
    {
        DoubleWeapon dW = weapons[equippedWeapon];
        dW.thisWeapon.weaponTransform.gameObject.SetActive(equip);
        dW.currentStance = 0; //reset stance to first
    }

    #endregion

    #region Weapon Functions

    public void AddAmmo(int addedAmmo)
    {
        int newValue = weapons[equippedWeapon].ammo + addedAmmo;
        int maxAmmo = weapons[equippedWeapon].maxAmmo;
        weapons[equippedWeapon].ammo = (newValue > maxAmmo) ? maxAmmo: newValue;
    }

    private void SwitchStances(float wheelInput)
    {
        DoubleWeapon dW = weapons[equippedWeapon];

        if (dW.stances.Length == 1) //if there is no other stance
            return;

        int dir = (wheelInput < 0)? -1: 1; //direction

        //update stance
        int newDir = dW.currentStance + dir;

        if (newDir < 0)
            dW.currentStance = dW.stances.Length - 1;
        else if (newDir == dW.stances.Length)
            dW.currentStance = 0;
        else
            dW.currentStance = newDir;
    }

    #region Shoot

    private void UseWeapon()
    {
        if (!CheckAmmo())
            return;
        CmdFireWeapon();
    }

    private bool CheckAmmo()
    {
        DoubleWeapon dW = weapons[equippedWeapon];
        if (dW.ammo - dW.stances[dW.currentStance].bulletsPerShot < 0)
            return false;
        return true;
    }

    [Command]
    private void CmdFireWeapon()
    {
        DoubleWeapon dW = weapons[equippedWeapon];
        WeaponStance w = dW.stances[dW.currentStance];

        //calculate offset
        float xOffset = Random.Range(-w.offsetX, w.offsetX);
        float yOffset = Random.Range(-w.offsetY, w.offsetY);

        //using offset info to give the bullet rotation
        Quaternion offsetBullet = transform.rotation;
        offsetBullet.x += xOffset;
        offsetBullet.y += yOffset;

        GameObject bullet = Instantiate(
            dW.stances[dW.currentStance].bullet, //prefab
            dW.thisWeapon.outputBullet.position, //output position
            offsetBullet);
        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * w.speedBullets;
        Bullet b = bullet.GetComponent<Bullet>();
        b.weaponIndex = equippedWeapon;
        b.stance = dW.currentStance;

        //destroy after set time
        Destroy(bullet, w.lifeTimeBullets);

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        //remove ammo
        dW.ammo -= w.bulletsPerShot;

        //meegeven dat ie mag bouncen, en force bullets

        StartCoroutine(timeBetweenAutomaticFire());
    }

    //to make sure you dont fire a million bullets per second
    private IEnumerator timeBetweenAutomaticFire()
    {
        mayFire = false;
        DoubleWeapon dW = weapons[equippedWeapon];
        yield return new WaitForSeconds(
            dW.stances[dW.currentStance].timeBetweenBullets);
        mayFire = true;
    }

    #endregion

    #endregion
}
