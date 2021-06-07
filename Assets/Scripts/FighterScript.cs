using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterScript : MonoBehaviour {
    public AudioClip attack1;
    private AudioSource _audioSource;
    private int damage = 0;
    public Collider[] attacks; //set array size in editor, can hold many attacks colliders.

    private float _attackBonusFactor = 0;
    private float _attackBonusEnd = -1;

    private float knockback = 0.75f; //feel free to change value

    private PlayerController pc;

    //private float knockback = 2.5f; //feel free to change value
    private bool damagedFlag, pressFlag;
    private float inAction;

    private static bool _devCheated = false;

    private Animator _animator;
    // Use this for initialization
    void Start () {
        _audioSource = GetComponent<AudioSource>();
        pc = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        damagedFlag = true;
    }
	
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButton("Fire1") && Input.GetButtonDown("Fire2") || Input.GetButton("Fire2") && Input.GetButtonDown("Fire1"))
        {
            _devCheated = !_devCheated;
        }

        // bonus end
        if (_attackBonusFactor != 0 && _attackBonusEnd != -1 && _attackBonusEnd < Time.realtimeSinceStartup)
            _attackBonusFactor = 0;

        if (Input.GetButtonDown("Fire1") && !pc.isHit && !checkAttackAction())
        {
            _audioSource.PlayOneShot(attack1);
            inAction = Time.time;
            damagedFlag = false;
            //Debug.Log("Click");

        }
        if (Time.time - inAction > 0.25f && Time.time - inAction < 0.72f)
        {
            if (damagedFlag == false)
            {
                //Time.timeScale = 0;
                damage = Mathf.RoundToInt(10 * (1 + _attackBonusFactor));
                if (_devCheated)
                    damage += 20;
                attack(attacks[0]);
            }
        }else if (Time.deltaTime - inAction > 0.8f)
        {
            pressFlag = true;
        }

        /*if (Input.GetButtonDown("Fire1") && !pc.isHit) 
        {
            damage = Mathf.RoundToInt(10 * (1 + _attackBonusFactor));
            _audioSource.PlayOneShot(attack1);
            attack(attacks[0]);
        }*/
    }

    public void SetAttackBuff(object[] arguments)
    {
        float zeroBasedFactor = (float)arguments[0];
        float buffEndTime = (float)arguments[1];
        _attackBonusFactor = zeroBasedFactor;
        _attackBonusEnd = buffEndTime;
    }

    //attack method. Checks for any colliders on the "Hitbox" layer that overlap the selected attacks collider, and sends method calls.
    void attack(Collider col) {
        Collider[] cols = Physics.OverlapBox(col.bounds.center,col.bounds.extents,col.transform.rotation,LayerMask.GetMask("Hitbox"));
        foreach(Collider c in cols) {
            if (c.transform.root == transform){
                continue;
            }
            c.SendMessageUpwards("TakeDamage", damage);
            // Debug.Log("TakeDamage");
            damagedFlag = true;
            c.SendMessageUpwards("TakeKnockback", knockback);
        }
    }
    private bool checkAttackAction()
    {
        if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
            return true;
        else return false;
    }
}
