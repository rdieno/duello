using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Controller : MonoBehaviour {

    private Rigidbody2D rb;
    public float movementSpeed;

    public Transform[] groundPoints;
    private float groundRadius = 0.2f;
    public LayerMask groundLayerMask;

    private bool grounded;
    private bool jumping;
    private float jumpTakeOffSpeed = 400.0f;
    private float horizontal = 0;
    private float attackColdDown;
    private bool attackFreeze;

    private Animator animator;
    private GameObject Player2;

    public GameObject playerAttacks;
    public GameObject playerColliders;
    private float highter;
    private bool isOnAir;


    public bool isHit = false;

    private float currentPlatformYPos = 0f;
    private bool isFalling = false;
    private int currentLayer = 0;

    const int playerLayer = 8;
    const int platformLayer = 9;

    private float _jumpBonusFactor = 0;
    private float _jumpBonusEnd = -1;
    private float _speedBonusFactor = 0;
    private float _speedBonusEnd = -1;

    public GameObject ballPrefab;
    public Transform ballSpawn;
    public GameObject UIRangedIndicator;
    private int rangedweapon = 1;

    private const float BALL_SPEED = 10;
    private bool isLeft = true;

    private GameObject _projectile;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Player2 = GameObject.Find("Player2");
        attackFreeze = false;

        _projectile = Resources.Load<GameObject>("Prefabs/RangedAttack/ProjectileSlow");
    }

    void Update()
    {
        // if the player is hit freeze all controls
        if (!isHit)
        {
            HandleInput();
            if (rb.velocity.y > 0)
            {
                //animator.SetBool("Jump", false);
                //animator.SetTrigger("JumpDown");
            }

            if (grounded)
            {
                highter = Player2.transform.position.y;
                //Debug.Log("High get");
            }
            else if (grounded == false && Player2.transform.position.y - highter > 0.4f)
            {
                isOnAir = true;
            }
            if (Player2.transform.position.y - highter < 0.3f && isOnAir == true)
            {
                animator.SetBool("Jump", false);
                //Debug.Log("Hittt");
                isOnAir = false;
            }

            if (Input.GetButtonDown("Fire2p2")  && rangedweapon > 0)
            {
                rangedweapon--;
                ShootBall();
                animator.SetTrigger("Throw");

                if (rangedweapon == 0)
                {
                    UIRangedIndicator.SetActive(false);
                }
            }
        }

        // bonus end
        if (_jumpBonusFactor != 0 && _jumpBonusEnd != -1 && _jumpBonusEnd < Time.realtimeSinceStartup)
            _jumpBonusFactor = 0;
        if (_speedBonusFactor != 0 && _speedBonusEnd != -1 && _speedBonusEnd < Time.realtimeSinceStartup)
            _speedBonusFactor = 0;
    }

    public void SetJumpBuff(object[] arguments)
    {
        float zeroBasedFactor = (float)arguments[0];
        float buffEndTime = (float)arguments[1];
        _jumpBonusFactor = zeroBasedFactor;
        _jumpBonusEnd = buffEndTime;
    }

    public void SetSpeedBuff(object[] arguments)
    {
        float zeroBasedFactor = (float)arguments[0];
        float buffEndTime = (float)arguments[1];
        _speedBonusFactor = zeroBasedFactor;
        _speedBonusEnd = buffEndTime;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal2");
        animator.SetFloat("Speed", horizontal * horizontal);


        // check direction and rotate projectile spawn
        if (horizontal > 0)
        {
            isLeft = false;
        }

        if (horizontal < 0)
        {
            isLeft = true;
        }

        if (!isLeft)
        {
            ballSpawn.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 1.2f, transform.position.z);
        }
        else
        {
            ballSpawn.position = new Vector3(transform.position.x - 0.5f, transform.position.y + 1.2f, transform.position.z);
        }

        // check if player is grounded
        grounded = IsGrounded();


        // once player has fallen below the platform, re-enable collisions with that platform
        if (isFalling && (gameObject.transform.position.y + (GetComponent<BoxCollider2D>().size.y / 2)) < currentPlatformYPos)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
            currentPlatformYPos = 0f;
            isFalling = false;
        }

        // do not allow the player to move while hit
        if (!isHit)
        {
            HandleMovement(horizontal);
        }

        ResetValues();
    }

    private void HandleMovement(float horizontal)
    {

        // determine movement speed based on horizontal input
        rb.velocity = new Vector2(horizontal * movementSpeed * (1 + _speedBonusFactor), rb.velocity.y);

        // check direction and rotate player & weapon hitboxes accordingly
        if (horizontal > 0)
        {
            Player2.transform.rotation = Quaternion.Euler(0, 120, 0);
            playerColliders.transform.rotation = Quaternion.Euler(0, 125, 0);
            //playerAttacks.transform.rotation = Quaternion.Euler(0, 125, 0);
            ballSpawn.transform.rotation = Quaternion.Euler(0, 0, 0);

        }
        else if (horizontal < 0)
        {
            Player2.transform.rotation = Quaternion.Euler(0, 235, 0);
            playerColliders.transform.rotation = Quaternion.Euler(0, -55, 0);
            ballSpawn.transform.rotation = Quaternion.Euler(0, 180, 0);
            //playerAttacks.transform.rotation = Quaternion.Euler(0, -55, 0);
        }

        // if player is on ground and has pressed the jump button add vertical force
        if (grounded && jumping)
        {
            grounded = false;
            rb.AddForce(new Vector2(0.0f, jumpTakeOffSpeed * (1 + _jumpBonusFactor)));
        }
        else if (Input.GetButtonUp("Jump2")) // if player is in the air and not holding the jump button
        {
            if (rb.velocity.y > 0)
            {
                animator.SetBool("Jump", false);
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);  // divide vertical velocity in half making character fall
            }
        }
    }

    private void HandleInput()
    {

        if (Input.GetButtonDown("Fire1p2") && attackFreeze == false)
        {
            attackColdDown = Time.time;
            animator.SetTrigger("Attack");
            attackFreeze = true;
            // Debug.Log("attack on");
        }
        if (Time.time - attackColdDown >= 0.6f && attackFreeze == true)
        {
            attackFreeze = false;
            // Debug.Log("attack freeze");
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Standing Melee Attack Horizontal"))
        {
        }

        if (Input.GetButtonDown("Jump2"))
        {
            animator.SetBool("Jump", true);
            jumping = true;
        }
        /*if (Input.GetButtonDown("Fire2"))
        {
            animator.SetBool("Jump", true);
=
        }

        */
        // if player presses down on keyboard/joystick, check if on a "fallthrough" platform and disable collision
        if (Input.GetAxis("Vertical2") < 0 && currentLayer == platformLayer)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
            currentPlatformYPos = groundPoints[0].transform.position.y;
            isFalling = true;
            grounded = false;
        }
    }

    // checks if groundPoints are colliding with the ground
    // groundPoints are empty game objects located at the players feet
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
                        currentLayer = colliders[i].gameObject.layer;

                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void ResetValues()
    {
        jumping = false;
    }

    // when the player is hit apply horizontal force in the direction they were attacked from
    // when isHit is true the players controls are frozen
    public IEnumerator GetHit(Vector2 knockback)
    {
        isHit = true;
        rb.AddForce(knockback, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        isHit = false;
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
        Destroy(ball, 2.0f);
    }
}
