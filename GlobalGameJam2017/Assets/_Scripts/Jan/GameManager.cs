using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    public static GameManager thisManager;
    public List<GameObject> players = new List<GameObject>();

    private void Awake()
    {
        thisManager = this;
    }

    [Command]
    public void CmdAddToGameManager(int id)
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject g in allPlayers)
            if(g.GetComponent<PlayerManager>().thisID == id)
            {
                players.Add(g);
                break;
            }
    }
}
