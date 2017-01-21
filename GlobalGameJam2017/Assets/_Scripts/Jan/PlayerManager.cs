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
    [SyncVar]
    public string playerName;

    private GameManager gameManager;

    [HideInInspector]
    public PlayerCombat playerCombat;
    [HideInInspector]
    public PlayerController playerController;
    public Transform cam;

    private void Awake()
    {
        health = maxHealth;

        if (isLocalPlayer)
        {
            cam.gameObject.SetActive(true);
            if (PlayerPrefs.HasKey(MainMenuScript.namePref))
                playerName = PlayerPrefs.GetString(MainMenuScript.namePref);
            else
                playerName = "Mysterious Challenger";
        }
        playerCombat = GetComponent<PlayerCombat>();
        playerController = GetComponent<PlayerController>();
    }

    #region Game Manager

    public void Start()
    {
        if (isLocalPlayer)
        {
            //get ID based on how many players there are
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            thisID = allPlayers.Length;
        }

        if (isServer)
        {
            gameManager = GameManager.thisManager;
            gameManager.CmdAddToGameManager(thisID);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetButtonDown("LShift"))
            ShowScores();
        else if (Input.GetButtonUp("LShift"))
            LocalGameManager.thisManager.ShowScores(null, false);
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
    public void RpcActivatePlayer(bool enable)
    {
        playerCombat.enabled = enable;
        playerController.enabled = enable;
    }

    [ClientRpc]
    public void RpcShowScores()
    {
        ShowScores();
    }

    private void ShowScores()
    {
        if (!isLocalPlayer)
            return;
        print(1);
        List<PlayerManager> scores = GetScores();
        LocalGameManager.thisManager.ShowScores(scores, true); //send the data to the lgm to be shown in the menu
    }

    private List<PlayerManager> GetScores()
    {
        //show ordered list with player scores
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        List<PlayerManager> pMs = new List<PlayerManager>();
        foreach (GameObject p in allPlayers)
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