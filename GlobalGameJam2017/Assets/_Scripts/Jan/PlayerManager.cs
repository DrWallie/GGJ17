using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerCombat), typeof(PlayerController))]
public class PlayerManager : NetworkBehaviour {

    [SyncVar]
    public int thisID,
        deaths,
        kills;
    private GameManager gameManager;

    [HideInInspector]
    public PlayerCombat playerCombat;
    [HideInInspector]
    public PlayerController playerController;

    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        playerController = GetComponent<PlayerController>();
    }

    #region Game Manager

    public void Start()
    {
        //get ID based on how many players there are
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        thisID = allPlayers.Length;

        gameManager = GameManager.thisManager;
        if (isServer)
            gameManager.CmdAddToGameManager(thisID);
    }

    [Command]
    public void CmdOnKill()
    {
        kills++;
        gameManager.CheckIfWon(thisID);
    }

    [Command]
    public void CmdOnDeath()
    {
        deaths++;
    }

    #endregion

    //this is purely visual for the clock locally
    public IEnumerator Timer(int maxTime)
    {
        float timeLeft = maxTime;

        while (timeLeft > 0f)
        {
            //show something visual
            timeLeft -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //this does not need to have consequences, you already do this in the gamemanager
    }
}