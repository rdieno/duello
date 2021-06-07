using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Different types of navigation
public enum OffMeshLinkMoveMethod
{
    NormalSpeed,
    Jump,
    LevelOneFall,
    LevelOneJump,
    LevelTwoJump,
    LevelTwoDownJump
}

public class EnemyScript : MonoBehaviour {

    public GameObject player;
    private NavMeshAgent agent;

    private BoxCollider2D collider;

    private float stoppingDistance = 0f;
    private OffMeshLinkMoveMethod method = OffMeshLinkMoveMethod.NormalSpeed;

    private float distanceModifier = 1f;

    private Animator anim;
    private Rigidbody2D rb;

    public bool attacking;

    private float attackRate = 1.5f;
    private float nextAttack = 0.0f;

    public GameObject enemyAttacks;
    public GameObject enemyColliders;

    public GameObject ground;
    private float groundYPos;

    public bool isHit = false;

    private Vector2 vKnockback = Vector2.zero;

    public Transform[] groundPoints;
    private float groundRadius = 0.2f;
    public LayerMask groundLayerMask;
    private bool grounded;

    private float _jumpBonusFactor = 0;
    private float _jumpBonusEnd = -1;
    private float _speedBonusFactor = 0;
    private float _speedBonusEnd = -1;

    public GameObject ballPrefab;
    public Transform ballSpawn;
    public GameObject UIRangedIndicator;
    private int rangedweapon = 1;

    private const float BALL_SPEED = 10;

    private GameObject _projectile;

    // Use this for initialization
    IEnumerator Start ()
    {
        _projectile = Resources.Load<GameObject>("Prefabs/RangedAttack/ProjectileSlow");

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        BoxCollider2D enemyCollider = GetComponent<BoxCollider2D>();
        BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
        attacking = false;

        // determines minimum space to leave between AI and player
        stoppingDistance = (playerCollider.size.x / 2) + (playerCollider.size.x / 2) + 0.01f;
        agent.stoppingDistance = stoppingDistance;

        // turns automatic off mesh link traveral off, we are going to handle it manually
        agent.autoTraverseOffMeshLink = false;

        // get the ground floor y position
        groundYPos = ground.transform.position.y + (ground.GetComponent<BoxCollider2D>().size.y / 2);

        while (true)
        {
            OffMeshLinkData omld = agent.currentOffMeshLinkData;
            // if we are not currently on an off mesh link the NavAgent controls movement automatically
            if (omld.offMeshLink != null)
            {
                // determine what type of Off Mesh Link we are currently on 
                switch (omld.offMeshLink.area)
                {

                    case 2:
                        method = OffMeshLinkMoveMethod.Jump;
                        break;
                    case 3:
                        method = OffMeshLinkMoveMethod.LevelOneJump;
                        break;
                    case 4:
                        method = OffMeshLinkMoveMethod.LevelOneFall;
                        break;
                    case 5:
                        method = OffMeshLinkMoveMethod.LevelTwoJump;
                        break;
                    case 6:
                        method = OffMeshLinkMoveMethod.LevelTwoDownJump;
                        break;
                    default:
                        method = OffMeshLinkMoveMethod.NormalSpeed;
                        break;
                }

            }

            // handle each off mesh link appropriately
            if (agent.isOnOffMeshLink)
            {
                if (method == OffMeshLinkMoveMethod.NormalSpeed)
                    yield return StartCoroutine(NormalSpeed(agent));
                if (method == OffMeshLinkMoveMethod.Jump)
                    yield return StartCoroutine(Parabola(agent, 1.5f, 0.75f));
                if (method == OffMeshLinkMoveMethod.LevelOneJump)
                    yield return StartCoroutine(Parabola(agent, 1.5f, 0.75f));
                if (method == OffMeshLinkMoveMethod.LevelOneFall)
                    yield return StartCoroutine(Fall(agent, 0.475f));
                if (method == OffMeshLinkMoveMethod.LevelTwoJump)
                    yield return StartCoroutine(Parabola(agent, 1.75f, 1.0f));
                if (method == OffMeshLinkMoveMethod.LevelTwoDownJump)
                    yield return StartCoroutine(Parabola(agent, 1.5f, 1.0f));

                if(agent.enabled)
                    agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update () {
        // bonus end
        if (_jumpBonusFactor != 0 && _jumpBonusEnd != -1 && _jumpBonusEnd < Time.realtimeSinceStartup)
            _jumpBonusFactor = 0;
        if (_speedBonusFactor != 0 && _speedBonusEnd != -1 && _speedBonusEnd < Time.realtimeSinceStartup)
            _speedBonusFactor = 0;

        // check if we are on a platform
        grounded = IsGrounded();

        if (agent.enabled == true)  // disable pathfinding when agent has been disabled
            agent.destination = player.transform.position;

        // freeze rotation if hit
        if (isHit) anim.SetLayerWeight(1, 0.1f);
        if (!isHit)
        {
            anim.SetLayerWeight(1, 1f);
            transform.rotation = GetDirectionAngle();
        }

        if (agent.velocity.x != 0f)
        {
            anim.SetFloat("Speed", 1);
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }


        attacking = false; // set this to false to avoid multiple attacks in a short period of time


        if (GetComponent<EnemyDamageHandler>().getHealth() < 21 && rangedweapon > 0 && !isHit)
        {
            //wait until it has been long enough since the last attack
            if (Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate; // reset attack cooldown
                rangedweapon--;
                ShootBall();
                anim.SetTrigger("Throw");

                if (rangedweapon == 0)
                {
                    UIRangedIndicator.SetActive(false);
                }
            }
        }

        // if within striking distance of player, attack
        if (Vector3.Distance(agent.destination, agent.transform.position) <= stoppingDistance + 0.1f && !isHit)
        {
            //wait until it has been long enough since the last attack
            if (Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate; // reset attack cooldown
                attacking = true;
                anim.SetTrigger("Attack");
            }
        }

  
        // if we are hit, the enemy will be in a stunned state, disable NavAgent functionality and enable rigidbody to handle physics
        if (isHit)
        {
            agent.enabled = false;
            rb.isKinematic = false;
            rb.mass = 10000f; // high mass so the player cannot push us
            rb.drag = 1.25f;
            rb.AddForce(vKnockback, ForceMode2D.Impulse);

            anim.SetFloat("Speed", 0);
        }
        else
        {
            // if not currently stunned and on the ground disable rigidbody effects and re-enable NavAgent
            // allow the AI to fall if not on top of a platform
            if (grounded)
            {
                rb.drag = 0f;
                rb.mass = 1f;
                rb.isKinematic = true;
                agent.enabled = true;
                vKnockback = Vector2.zero;
            }
        }
    }

    public void SetSpeedBuff(object[] arguments)
    {
        float zeroBasedFactor = (float)arguments[0];
        float buffEndTime = (float)arguments[1];
        _speedBonusFactor = zeroBasedFactor;
        _speedBonusEnd = buffEndTime;

    }

    public void SetJumpBuff(object[] arguments)
    {
        float zeroBasedFactor = (float)arguments[0];
        float buffEndTime = (float)arguments[1];
        _jumpBonusFactor = zeroBasedFactor;
        _jumpBonusEnd = buffEndTime;
    }

    Quaternion GetDirectionAngle()
    {
        if (Vector3.Distance(agent.destination, agent.transform.position) <= stoppingDistance + 0.1f)
        {
            if (agent.transform.position.x > player.transform.position.x)
            {
                enemyColliders.transform.rotation = Quaternion.Euler(0, -125, 0);
                //enemyAttacks.transform.rotation = Quaternion.Euler(0, -125, 0);
                ballSpawn.rotation = Quaternion.Euler(0, 180, 0);
                ballSpawn.position = new Vector3(transform.position.x - 0.5f, transform.position.y + 1.2f, transform.position.z);
                return Quaternion.Euler(0, 272f, 0);
            }
            else
            {
                enemyColliders.transform.rotation = Quaternion.Euler(0, 55, 0);
                //enemyAttacks.transform.rotation = Quaternion.Euler(0, 55, 0);
                ballSpawn.rotation = Quaternion.Euler(0, 0, 0);
                ballSpawn.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 1.2f, transform.position.z);
                return Quaternion.Euler(0, 125f, 0);
            }
        }
        else
        {
            if (agent.desiredVelocity.x < 0)
            {
                enemyColliders.transform.rotation = Quaternion.Euler(0, -125, 0);
                //enemyAttacks.transform.rotation = Quaternion.Euler(0, -125, 0);
                ballSpawn.rotation = Quaternion.Euler(0, 180, 0);
                ballSpawn.position = new Vector3(transform.position.x - 0.5f, transform.position.y + 1.2f, transform.position.z);
                return Quaternion.Euler(0, 272f, 0);
            }
            else
            {
                enemyColliders.transform.rotation = Quaternion.Euler(0, 55, 0);
                //enemyAttacks.transform.rotation = Quaternion.Euler(0, 55, 0);
                ballSpawn.rotation = Quaternion.Euler(0, 0, 0);
                ballSpawn.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 1.2f, transform.position.z);
                return Quaternion.Euler(0, 125f, 0);
            }
        }
    }

    Vector2 GetKnockbackDirection()
    {
        Vector2 direction = player.transform.position - rb.transform.position;
        direction.Normalize();
        return direction;
    }

    // Moves NavAgent manually at normal speed to the current off mesh link in its navigation path
    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }

    // Moves NavAgent manually straight downwards
    IEnumerator Fall(NavMeshAgent agent, float duration)
    {

        float height = 0.001f;
        //float duration = 0.2f;

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.startPos;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }

        data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;

        endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }

    // Moves NavAgent manually along a parabola path
    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;

        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        //anim.SetBool("Jump", true);
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

    }

    // gets called when we have been hit
    public IEnumerator GetHit(Vector2 knockback)
    {
        // when we are hit save the knockback force amount
        vKnockback = knockback * 1000; // knockback is increased due to large mass
        isHit = true; // triggers stun effect in update methos
        //anim.SetTrigger("Hit");
        yield return new WaitForSeconds(1f); 
        isHit = false;
        yield return null;
    }

    // checks if groundPoints are colliding with the ground
    // groundPoints are empty game objects located at the enemy's feet
    private bool IsGrounded()
    {
        if (rb.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, groundLayerMask);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    private void ShootBall()
    {
        // Create the Bullet from the Bullet Prefab
        var ball = (GameObject)Instantiate(
            _projectile,
            ballSpawn.position,
            ballSpawn.rotation);

        ball.GetComponentInChildren<ProjectileDamage>().isPlayer = false;

        // Add velocity to the bullet
        ball.GetComponent<Rigidbody2D>().velocity = transform.forward * BALL_SPEED;

        // Destroy the bullet after 2 seconds
        Destroy(ball, 10.0f);
    }

}
