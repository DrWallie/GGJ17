using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

    [SyncVar]
    public int thisID;
    private GameManager gameManager;

    public void Start()
    {
        //get ID based on how many players there are
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        thisID = allPlayers.Length;

        gameManager = GameManager.thisManager;
        if (isServer)
            gameManager.CmdAddToGameManager(thisID);
    }
}

       
