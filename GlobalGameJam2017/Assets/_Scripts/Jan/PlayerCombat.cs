using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {

    public DoubleWeapon[] weapons;
    public int equippedWeapon;

    private void Awake()
    {
        InitializeFirstWeapon();
    }

    #region Equip / Disequip Weapon

    private void InitializeFirstWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            DoubleWeapon dW = weapons[i];
            if (dW.ammo > 0)
            {
                EquipWeapon(i);
                break;
            }
        }
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
        weapons[equippedWeapon].thisWeapon.weaponTransform.gameObject.SetActive(equip);
    }

    #endregion
}
