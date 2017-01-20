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

        if (!mayFire)
            return;

        if (Input.GetButton("Fire1"))
            UseWeapon();

        //F2 for switching stances

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
        dW.currentStance = dW.stance1;
    }

    #endregion

    #region Weapon Functions

    private void UseWeapon()
    {
        if (!CheckAmmo())
            return;
        CmdFireWeapon();
    }

    private bool CheckAmmo()
    {
        DoubleWeapon dW = weapons[equippedWeapon];
        if (dW.ammo - dW.currentStance.bulletsPerShot < 0)
            return false;
        return true;
    }

    [Command]
    private void CmdFireWeapon()
    {
        DoubleWeapon dW = weapons[equippedWeapon];
        WeaponStance w = dW.currentStance;

        //calculate offset
        float xOffset = Random.Range(-w.offsetX, w.offsetX);
        float yOffset = Random.Range(-w.offsetY, w.offsetY);

        //using offset info to give the bullet rotation
        Quaternion offsetBullet = transform.rotation;
        offsetBullet.x += xOffset;
        offsetBullet.y += yOffset;

        GameObject bullet = Instantiate(
            dW.currentStance.bullet, //prefab
            dW.thisWeapon.outputBullet.position, //output position
            offsetBullet);
        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * w.speedBullets;

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
        yield return new WaitForSeconds(
            weapons[equippedWeapon].currentStance.timeBetweenBullets);
        mayFire = true;
    }

    #endregion
}
