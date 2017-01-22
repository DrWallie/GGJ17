using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalGameManager : MonoBehaviour {

    public static LocalGameManager thisManager;
    public ScoreBoard scoreBoard;
    public static PlayerManager thisPlayer;
    public GameObject[] spawnPoints;

    private void Awake()
    {
        thisManager = this;
        GetSpawnPoints();
    }

    private void GetSpawnPoints()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
    }

    public float timeToRespawn;
    public IEnumerator RespawnPlayer()
    {
        thisPlayer.gameObject.SetActive(false);
        yield return new WaitForSeconds(timeToRespawn);

        //respawn
        int pos = UnityEngine.Random.Range(0, spawnPoints.Length);
        thisPlayer.transform.position = spawnPoints[pos].transform.position;
        thisPlayer.gameObject.SetActive(true);
    }

    [Serializable]
	public class ScoreBoard
    {
        public Transform board;
        [Tooltip("Also add the top score here, from bottom to top = worst to best.")]
        public List<ScorePlayer> scores;
    }

    [Serializable]
    public class ScorePlayer
    {
        public Text name;
        public Text score;
    }

    public void ShowScores(List<PlayerManager> pMs, bool enable) //leave pMs null if disabling
    {
        scoreBoard.board.gameObject.SetActive(enable);
        if (!(pMs != null)) //because unity will give errors otherwise
            return;
        int _i = 0;
        for(int i = pMs.Count - 1; i >= 0; i--)
        {
            ScorePlayer s = scoreBoard.scores[i];
            PlayerManager pM = pMs[_i];
            s.name.text = pM.playerName;
            s.score.text = pM.kills + "/" + pM.deaths;
            _i++;
        }

        //after that only empty the "empty"

        for (int i = pMs.Count; i < scoreBoard.scores.Count; i++)
        {
            ScorePlayer s = scoreBoard.scores[i];
            s.name.text = "";
            s.score.text = "";
        }
    }
}
