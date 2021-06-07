using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour {

    public GameObject scorePanel;
    public GameObject failPanel;
    public GameObject Player;
    public GameObject Enemy;
    public int timer;
    public Text timerText;
    // public Text playerHealthText;
    // public Text enemyHealthText;
    public static bool timerIsActive = true;
    private DamageHandler player;
   // private EnemyScript enemy;
    private EnemyDamageHandler enemy;
    private bool victory = true;

    // Use this for initialization
    void Start () {
        timer = 90;
        player = Player.GetComponent<DamageHandler>();
        enemy = Enemy.GetComponent<EnemyDamageHandler>();
        StartCoroutine("LoseTime");
	}
	
	// Update is called once per frame
	void Update () {
        if(timer <= 20)
        {
            timerText.color = Color.red;
        }

        timerText.text = timer.ToString();

        //Health text updates;
        // updateHealthDisplays();
        // What happens when the timer is stopped
        if(!timerIsActive || timer == 0)
        {
            StopCoroutine("LoseTime");
            
            if(timer > 0 && timer <= 90)
            {
                // Tell ScoreManager the score
                ScoreManager.timerScore = timer * 100;
                ScoreManager.healthScore = player.getHealth() * 200;
                victory = true;
            }

            if (timer <= 0 )
            {
                // Losing End Game Logic Here
                ScoreManager.timerScore = 0;
                victory = false;
            }

            if (player.getHealth() <= 0)
            {
                // Losing End Game Logic Here
                ScoreManager.healthScore = 0;
                victory = false;
            }

            ScoreManager.UpdateScore();

            // Show End Game Panel
            if (victory == true)
            {
                scorePanel.SetActive(true);
            } else {
                failPanel.SetActive(true);
            }
            
            enabled = false;
        }
        else
        {
            scorePanel.GetComponent<Image>().material.SetFloat("_Size", 0);
            failPanel.GetComponent<Image>().material.SetFloat("_Size", 0);
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

    //sub method that sorts out the health display logic
    /*
    void updateHealthDisplays() {
        //Player health
        playerHealthText.text = "Player Health: " + player.getHealth() + "%";
        if (player.getHealth() <= 20)
        {
            playerHealthText.text = "Player Health: <color=red>" + player.getHealth() + "%</color>";
        }

        //enemy health
        enemyHealthText.text = "Enemy Health: " + enemy.getHealth() + "%";
        if (enemy.getHealth() <= 20)
        {
            enemyHealthText.text = "Enemy Health: <color=red>" + enemy.getHealth() + "%</color>";
        }
    }*/
}
