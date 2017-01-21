using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FFA_Manager : NetworkBehaviour {

    public List<PlayerInfo> players = new List<PlayerInfo>();

    #region References

    public static FFA_Manager thisManager;

    #endregion

    private void Awake()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        thisManager = this;
    }

    //create a new instance when a player joins
    public class PlayerInfo
    {
        public Transform player; //the player in the scene
        public string name;
        public int kills;
        public int deaths;

        //shortcut
        public PlayerCombat playerCombat;
        public PlayerController playerController;
        public PlayerManager playerManager;

        public PlayerInfo(Transform _player)
        {
            player = _player;
            name = "Mysterious Challenger";

            //create some shortcuts
            playerCombat = _player.GetComponent<PlayerCombat>();
            playerController = _player.GetComponent<PlayerController>();
            playerManager = _player.GetComponent<PlayerManager>();
        }
    }

    public void AddPlayer(Transform player)
    {
        players.Add(new PlayerInfo(player));
        int iD = players.Count - 1;
        players[iD].playerManager.thisID = iD;
        print(players.Count);
    }

    public void RemovePlayer(int playerID)
    {
        players.RemoveAt(playerID);
    }
}
