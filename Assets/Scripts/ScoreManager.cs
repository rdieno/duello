using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public GameObject GO_Score;
    public GameObject GO_High;
    public static int healthScore;
    public static int timerScore;
    public static int totalScore;
    private static string highScoreTag;
    private static string highScoreDateTag;
    private static int highScore;
    private static int checkHighScore = 0;
    private static string highScoreDate;
    private static Text scoreText;
    private static Text highScoreText;
    private static Text scoreText_GO;
    private static Text highScoreText_GO;
    private static int currentScore;

    private void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        highScoreText = GameObject.Find("HighScoreText").GetComponent<Text>();
        scoreText_GO = GO_Score.GetComponent<Text>();
        highScoreText_GO = GO_High.GetComponent<Text>();
    }

    void Awake()
    {
        // Reset the scores
        healthScore = 0;
        timerScore = 0;
        totalScore = 0;
    }

    public static void UpdateScore()
    {
        totalScore = healthScore + timerScore;
        UpdateCurrentScore();
        UpdateEndPanelText();
    }

    private static void UpdateCurrentScore()
    {
        currentScore = PlayerPrefs.GetInt("currentScoreDuello", 0) + totalScore;
        PlayerPrefs.SetInt("currentScoreDuello", currentScore);
        Debug.Log("Current Score [UpdateCurrentScore()]: " + currentScore);
    }

    private static void UpdateEndPanelText()
    {
        // Set the display text when the game ends
        if (healthScore != 0 && timerScore != 0)
        {
            scoreText.text = string.Format("{0,-20}{1,5}\n{2,-20}{3,7}\n{4,-20}{5,7}\n\n{6,-20}{7,7}"
                                        , "HEALTH REMAINING:"
                                        , healthScore
                                        , "TIME REMAINING:"
                                        , timerScore
                                        , "LEVEL SCORE:"
                                        , totalScore
                                        , "TOTAL SCORE:"
                                        , currentScore);
        }

        if (healthScore == 0)
        {
            UpdateHighScores(currentScore);
            currentScore -= timerScore;
            scoreText.text = string.Format("{0}\n\n{1}\n\n{2,-20}{3,7}"
                                        , "YOU DIED!!"
                                        , "GAME OVER"
                                        , "FINAL SCORE:"
                                        , currentScore);
            Debug.Log("Current Score [UpdateEndPanelText() if (healthScore == 0)]: " + currentScore);
        }

        if (timerScore == 0)
        {
            UpdateHighScores(currentScore);
            currentScore -= healthScore;
            scoreText.text = string.Format("{0}\n\n{1}\n\n{2,-20}{3,7}"
                                        , "YOU RAN OUT OF TIME!!"
                                        , "GAME OVER"
                                        , "FINAL SCORE:"
                                        , currentScore);
            Debug.Log("Current Score [UpdateEndPanelText() if (timerScore == 0)]: " + currentScore);
        }

        scoreText_GO.text = scoreText.text;

        // Display the current high score
        highScore = PlayerPrefs.GetInt("highScoreDuello1");
        highScoreDate = PlayerPrefs.GetString("highScoreDateDuello1");

        if (highScore > 0)
        {
            highScoreText.text = string.Format("{0} {1} ({2})"
                                        , "CURRENT HIGH SCORE:"
                                        , highScore
                                        , highScoreDate);
        }
        else
        {
            highScoreText.text = "NO CURRENT HIGH SCORE AVAILABLE";
        }
        highScoreText_GO.text = highScoreText.text;
    }

    public static void UpdateCurrentScoreOnRestart()
    {
        int restartCurrentScore = PlayerPrefs.GetInt("currentScoreDuello") - totalScore;
        if (restartCurrentScore > 0)
        {
            PlayerPrefs.SetInt("currentScoreDuello", restartCurrentScore);
        }
        else
        {
            PlayerPrefs.SetInt("currentScoreDuello", 0);
        }
        Debug.Log("Current Score (UpdateCurrentScoreOnRestart()): " + PlayerPrefs.GetInt("currentScoreDuello"));
    }

    private static void UpdateHighScores(int cs)
    {
        // Go through all saved high scores
        for (int i = 1; i <= 5; i++)
        {
            // Get High Score
            highScoreTag = "highScoreDuello" + i.ToString();
            checkHighScore = PlayerPrefs.GetInt(highScoreTag, 0);

            // Get High Score Date
            highScoreDateTag = "highScoreDateDuello" + i;
            highScoreDate = PlayerPrefs.GetString(highScoreDateTag, "1-Jan-2000 12:00:00 AM");

            // Check for a new high score, set if needed
            if (cs > checkHighScore)
            {
                DateTime thisDate = DateTime.Now;
                PlayerPrefs.SetString(highScoreDateTag, thisDate.ToString("d-MMM-yyyy h:mm:ss tt"));
                PlayerPrefs.SetInt(highScoreTag, cs);
                cs = checkHighScore;
            }
        }
    }
}
