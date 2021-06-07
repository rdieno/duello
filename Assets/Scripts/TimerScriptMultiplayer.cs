using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerScriptMultiplayer : MonoBehaviour {

    public GameObject scorePanel;
    public GameObject p1;
    public GameObject p2;
    public int timer;
    public static bool timerIsActive = true;
    private DamageHandler player;
    private DamageHandler player2;
    private int p1Wins;
    private int p2Wins;
    public Text timerText;
    public Text winnerText;
    public Text p1WinsText;
    public Text p2WinsText;
    private string winner;

    // Use this for initialization
    void Start () {
        timer = 90;
        player = p1.GetComponent<DamageHandler>();
        player2 = p2.GetComponent<DamageHandler>();
        p1Wins = PlayerPrefs.GetInt("p1WinsDuello", 0);
        p2Wins = PlayerPrefs.GetInt("p2WinsDuello", 0);
        StartCoroutine("LoseTime");
	}
	
	// Update is called once per frame
	void Update () {
        if(timer <= 20)
        {
            timerText.color = Color.red;
        }

        timerText.text = timer.ToString();

        // What happens when the timer is stopped
        if(!timerIsActive || timer == 0)
        {
            StopCoroutine("LoseTime");
            
            if(timer > 0 && timer <= 90)
            {
                // Check which player wins
                if(player.getHealth() <= 0)
                {
                    winner = "PLAYER 2 WINS!";
                    p2Wins++;
                } else
                {
                    winner = "PLAYER 1 WINS!";
                    p1Wins++;
                }
            }

            if (timer <= 0 )
            {
                winner = "YOU RAN OUT OF TIME!!";
            }

            PlayerPrefs.SetInt("p1WinsDuello", p1Wins);
            PlayerPrefs.SetInt("p2WinsDuello", p2Wins);
            UpdateEndPanelText();

            scorePanel.SetActive(true);
            enabled = false;
        }
        else
        {
            scorePanel.GetComponent<Image>().material.SetFloat("_Size", 0);
        }
	}

    // Reduce the timer by 1 each second passed
    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timer--;
        }
    }

    private void UpdateEndPanelText()
    {
        winnerText.text = winner;

        // Display the current wins total
        p1WinsText.text = "PLAYER 1: " + PlayerPrefs.GetInt("p1WinsDuello").ToString();
        p2WinsText.text = "PLAYER 2: " + PlayerPrefs.GetInt("p2WinsDuello").ToString();
    }
}
