using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupArmor : Pickup {

    public int addedArmor;

    protected override void AddPickupToPlayer(PlayerManager pM)
    {
        pM.armor += addedArmor;
    }
}
