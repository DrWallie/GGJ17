using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalGameManager : MonoBehaviour {

    public static LocalGameManager thisManager;
    public ScoreBoard scoreBoard;

    private void Awake()
    {
        thisManager = this;
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
        for(int i = 0; i < pMs.Count; i++)
        {
            ScorePlayer s = scoreBoard.scores[i];
            PlayerManager pM = pMs[i];
            //do something with the name here
            s.score.text = pM.kills + "/" + pM.deaths;
        }

    }
}
