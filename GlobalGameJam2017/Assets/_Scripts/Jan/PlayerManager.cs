using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

    [HideInInspector]
    public int thisID;

    public override void OnStartLocalPlayer()
    {
        FFA_Manager.thisManager.AddPlayer(transform);
    }
}
