using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFighterScript : MonoBehaviour {

    private int damage = 0;
    public Collider[] attacks; //set array size in editor, can hold many attacks colliders.
    private float knockback = 0.75f; //feel free to change value


    private EnemyScript enemyScript;

    private float _attackBonusFactor = 0;
    private float _attackBonusEnd = -1;

    private bool damagedFlag;
    private float inAction;

    public EnemyController enemyController;

    public AudioClip attack1;
    private AudioSource _audioSource;

    private Animator _animator;
    // Use this for initialization
    void Start () {
        enemyScript = GetComponent<EnemyScript>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        damagedFlag = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // bonus end
        if (_attackBonusFactor != 0 && _attackBonusEnd != -1 && _attackBonusEnd < Time.realtimeSinceStartup)
            _attackBonusFactor = 0;

        if (enemyScript.attacking) {
            _audioSource.PlayOneShot(attack1);
            inAction = Time.time;
            damagedFlag = false;
        }
        if (Time.time - inAction > 0.25f && Time.time - inAction < 0.72f)
        {
            if (damagedFlag == false)
            {
                damage = 10;
                attack(attacks[0]);
            }
        }
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
        foreach (Collider c in cols) {
            if (c.transform.root == transform){
                continue;
            }
            c.SendMessageUpwards("TakeDamage", damage);
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
