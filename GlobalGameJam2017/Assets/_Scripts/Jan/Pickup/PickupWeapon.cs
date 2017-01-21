using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Pickup {

    public int thisWeaponIndex;
    public int ammoRestored;

    protected override void OnTriggerEnter(Collider c)
    {
        if (c.transform.tag == "Player") {
            PlayerManager p = c.GetComponent<PlayerManager>();
            if (p == LocalGameManager.thisPlayer)
            {
                PlayerCombat pC = p.playerCombat;
                pC.EquipWeapon(thisWeaponIndex);
                pC.AddAmmo(ammoRestored);
                StartCoroutine(Respawn());
            }
        }
    }
}
