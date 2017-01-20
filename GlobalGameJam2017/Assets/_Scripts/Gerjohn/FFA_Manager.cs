using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFA_Manager : MonoBehaviour {

    public List<GameObject> players = new List<GameObject>();
    public List<string> playerNames = new List<string>();
    public List<int> playerKills = new List<int>();
    public List<int> playerDeaths = new List<int>();
    
    public void AddNewPlayer(GameObject g, string pName)
    {
        players.Add(g);
        playerNames.Add(pName);
        playerKills.Add(0);
        playerDeaths.Add(0);
    }
}
