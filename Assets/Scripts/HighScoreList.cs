using System;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreList : MonoBehaviour {

    private int[] highScores = { 0, 0, 0, 0, 0 };
    private string[] highScoreDates = { "","","","","" };
    public Text highScoreText;

	// Use this for initialization
	void Start () {
		for(int i = 1; i <= 5; i++)
        {
            string highScore = "highScoreDuello" + i.ToString();
            highScores[i-1] = PlayerPrefs.GetInt(highScore, 0);

            string highScoreDate = "highScoreDateDuello" + i.ToString();
            highScoreDates[i - 1] = PlayerPrefs.GetString(highScoreDate, "1-Jan-2000 12:00:00 AM");

        }

        getHighScoreList();
	}
	
	void getHighScoreList () {
        highScoreText.text = string.Format("{0}\n\n{1:D5}{2,33}\n{3:D5}{4,33}\n{5:D5}{6,33}\n{7:D5}{8,33}\n{9:D5}{10,33}"
                                            , "CURRENT HIGH SCORES"
                                            , highScores[0]
                                            , highScoreDates[0]
                                            , highScores[1]
                                            , highScoreDates[1]
                                            , highScores[2]
                                            , highScoreDates[2]
                                            , highScores[3]
                                            , highScoreDates[3]
                                            , highScores[4]
                                            , highScoreDates[4]);
    }
}
