using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerCombat), typeof(PlayerController))]
public class PlayerManager : NetworkBehaviour {

    [SyncVar]
    public int thisID,
        deaths,
        kills,
        health,
        armor;
    public int maxHealth;

    private GameManager gameManager;

    [HideInInspector]
    public PlayerCombat playerCombat;
    [HideInInspector]
    public PlayerController playerController;
    public Transform cam;

    private void Awake()
    {
        health = maxHealth;

        if(isLocalPlayer)
            cam.gameObject.SetActive(true);
        playerCombat = GetComponent<PlayerCombat>();
        playerController = GetComponent<PlayerController>();
    }

    #region Game Manager

    public void Start()
    {
        //get ID based on how many players there are
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        thisID = allPlayers.Length;

        if (isServer)
        {
            gameManager = GameManager.thisManager;
            gameManager.CmdAddToGameManager(thisID);
        }
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
        //instead of destroying try to disable it, since vital information is stored here.
        deaths++;
    }

    [ClientRpc]
    public void RpcActivatePlayer()
    {
        playerCombat.enabled = true;
        playerController.enabled = true;
    }

    [ClientRpc]
    public void RpcShowScores(int yourRanking)
    {
        print(yourRanking); //I somehow cant send classes so this will have to do
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

        //this does not need to have consequences, I already do this in the gamemanager
    }
}