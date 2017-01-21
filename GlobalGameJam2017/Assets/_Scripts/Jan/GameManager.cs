using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    public static GameManager thisManager;
    public List<GameObject> players = new List<GameObject>();

    #region Inspector Settings

    public int maxGameTime = 8;
    public int maxKills = 20;
    public int minPlayerCount = 2;

    #endregion

    private void Awake()
    {
        thisManager = this;
    }

    private void Start()
    {
        StartCoroutine(GameHandler());
    }

    [HideInInspector]
    public float timeLeft;
    private IEnumerator GameHandler()
    {
        timeLeft = maxGameTime;
        //to make sure enough players are in the game
        while (players.Count < minPlayerCount)
            yield return null;

        //start game
        foreach(GameObject p in players)
        {
            PlayerManager pM = p.GetComponent<PlayerManager>();
            pM.playerCombat.enabled = true;
            pM.playerController.enabled = true;
        }

        while(maxGameTime > 0f) //DO NOT SYNCH THIS, THERE IS A PERFORMANCE REASON I ALSO DID THIS LOCALLY
        {
            timeLeft -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //end game
        EndGame();
    }

    private void EndGame()
    {
        StopAllCoroutines();
        RpcCheckFinalScores();
        //stop game
        //start game
        foreach (GameObject p in players)
        {
            PlayerManager pM = p.GetComponent<PlayerManager>();
            pM.playerCombat.enabled = false;
            pM.playerController.enabled = false;
        }
    }

    [ClientRpc] //this allows it to be used on clients
    private void RpcCheckFinalScores()
    {
        //show ordered list with player scores
    }

    public void CheckIfWon(int id)
    {
        PlayerManager pM = players[id].GetComponent<PlayerManager>();
        if(pM.kills == maxKills)
            EndGame();
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
