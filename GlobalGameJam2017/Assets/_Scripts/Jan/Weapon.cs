using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponStance {

    public int damage;
    [Tooltip("you use this to control the accuracy of the weapon, you random range between this for a shot.")]
    public float offsetX, offsetY;
    [Tooltip("the addforce the victim has when hit.")]
    public float forceBullets;
    public bool bouncingBullets;
    public int bulletsPerShot;
}

[Serializable]
public class DoubleWeapon
{
    public int ammo, maxAmmo;
    public Weapon thisWeapon;
    [Tooltip("Your two weapon stances.")]
    public WeaponStance stance1, stance2;
    [HideInInspector]
    public WeaponStance currentStance;

    public DoubleWeapon()
    {
        currentStance = stance1;
    }
}

[Serializable]
public class Weapon
{
    public Transform weaponTransform;
    [Tooltip("This is where your bullets come from.")]
    public Transform outputBullet;
}
