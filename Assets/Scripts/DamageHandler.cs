using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour {

    public AudioClip damageSound;
    private AudioSource _audioSource;
    public GameObject Opponent;
    private Rigidbody2D body;
    private Rigidbody2D oppponent;
    private int health = 100;

    private PlayerController pc;

    private float _resistanceFactor = 0;
    private float _resistanceEnd = -1;


    private bool isHit = false, Hitted=false;
    public Renderer bodyRend;
    private float damageHitTime, newTime;

    private Animator animator;

    // Use this for initialization
    void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        oppponent = Opponent.GetComponent<Rigidbody2D>();

        pc = GetComponent<PlayerController>();
        _audioSource = GetComponent<AudioSource>();

        animator = GetComponent<Animator>();
        //bodyRend = GetComponent<Renderer>();

    }
	
	// Update is called once per frame
	void Update () {

        // resistance bonus end
        if (_resistanceFactor != 0 && _resistanceEnd != -1 && _resistanceEnd < Time.realtimeSinceStartup)
            _resistanceFactor = 0;

        if (isHit == true)
        {
            // Debug.Log(" 111");
            damageHitTime = Time.time;
            isHit = false;
            Hitted = true;
        }
        if (Hitted == true)
        {
            newTime = Time.time - damageHitTime;
            // Debug.Log("Time:" + damageHitTime);
            // Debug.Log("newTime:" + newTime);
            /*if (newTime < 0.1f || (newTime > 0.3f && newTime < 0.5f))
            {
                bodyRend.material.SetColor("_Specular", Color.red); //.material.SetColor("_Color", Color.red);
            }
            else
            {
                bodyRend.material.SetColor("_Specular", Color.cyan);
                if (newTime > 0.7f)
                {
                    isHit = false;
                    Hitted = false;
                }
            }*/
        }
    }

    public void SetResistanceBuff(object[] arguments)
    {
        float zeroBasedFactor = (float)arguments[0];
        float buffEndTime = (float)arguments[1];
        _resistanceFactor = zeroBasedFactor;
        _resistanceEnd = buffEndTime;
    }

    //public access for the health variable
    public int getHealth() { return health; }

    public void RestoreHealth(int amount)
    {
        int healthAfter = health + amount;
        if (healthAfter > 100) healthAfter = 100;
        health = healthAfter;
    }

    //Health updater from an attack.
    public void TakeDamage(int damage) {


        health -= Mathf.FloorToInt((float)damage * (1 - _resistanceFactor));
        _audioSource.PlayOneShot(damageSound);

        isHit = true;
        //animator.ResetTrigger("Attack");
        animator.SetTrigger("isHit");        

        if (health <= 0) {
            health = 0;
            TimerScript.timerIsActive = false;
            TimerScriptMultiplayer.timerIsActive = false;
        }
    }

    //Kockback effect upon getting hit.
    public void TakeKnockback(float knockback) {
        //Debug.Log("Body position: " + body.position);
        //Debug.Log("Opponent position: " + oppponent.position);

        Vector2 direction = oppponent.position - body.position;
        direction.Normalize();
        pc.StartCoroutine("GetHit", direction * -knockback);
    }
}
