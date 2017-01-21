using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(LocalGameManager))]
public class GameManager : NetworkBehaviour {

    public static GameManager thisManager;
    public List<GameObject> players = new List<GameObject>(); //niet getest of dit juist gevuld is als iemand disconnect

    #region Inspector Settings

    public int maxGameTimeInMinutes = 8;
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
        timeLeft = maxGameTimeInMinutes * 60;
        //to make sure enough players are in the game
        while (players.Count < minPlayerCount)
            yield return null;

        //start game
        foreach(GameObject p in players)
        {
            PlayerManager pM = p.GetComponent<PlayerManager>();
            pM.RpcActivatePlayer(true);
        }

        yield return new WaitForSeconds(timeLeft);
        //end game
        EndGame();
    }

    private void EndGame()
    {
        StopAllCoroutines();

        //stop game
        foreach (GameObject p in players)
        {
            PlayerManager pM = p.GetComponent<PlayerManager>();
            pM.RpcActivatePlayer(false);
        }

        CheckFinalScores();
    }

    private void CheckFinalScores()
    {
        List<PlayerManager> pMs = GetScores();
        //do something nice with this list in playerManager
        //the list goes from 0 = lowest to length = highest

        for (int i = 0; i < pMs.Count; i++)
        {
            PlayerManager pM = pMs[i].GetComponent<PlayerManager>();
            pM.RpcShowScores();
        }
    }

    private List<PlayerManager> GetScores()
    {
        //show ordered list with player scores
        List<PlayerManager> pMs = new List<PlayerManager>();
        foreach (GameObject p in players)
        {
            PlayerManager pM = p.GetComponent<PlayerManager>();
            if (pMs.Count == 0) //if list is empty
            {
                pMs.Add(pM);
                continue;
            }
            bool inserted = false;
            for (int i = 0; i < pMs.Count; i++)
                if (pM.kills < pMs[i].kills)
                {
                    pMs.Insert(i, pM);
                    inserted = true;
                    break;
                }
            if (!inserted) //this means that it is the best score yet
                pMs.Add(pM);
        }
        return pMs;
    }

    public void CheckIfWon(int id)
    {
        PlayerManager pM = players[id - 1].GetComponent<PlayerManager>();
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
