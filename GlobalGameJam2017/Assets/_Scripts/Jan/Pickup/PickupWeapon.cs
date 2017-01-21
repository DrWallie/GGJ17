using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Pickup {

    public int thisWeaponIndex;
    public int ammoRestored;

    protected override void AddPickupToPlayer(PlayerManager pM)
    {
        PlayerCombat pC = pM.playerCombat;
        pC.EquipWeapon(thisWeaponIndex);
        pC.AddAmmo(ammoRestored);
    }
}
