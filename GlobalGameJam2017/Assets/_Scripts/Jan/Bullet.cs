using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    [HideInInspector]
    public int weaponIndex = - 1;
    [HideInInspector]
    public int stance;

    
}
