using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Assets;

public class Player : Singleton<Player>
{
    public float HP;
    public float MaxHP;
    public float MP;
    public float MaxMP;
    public float Atk;

    public ParticleSystem CastingSpellEffect;
    public ObjectPoolManager projectilePool;

    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private PlayerAnimScr playerAnimScr;
    private SpriteRenderer spriteRenderer;
    private LayerMask groundLayer;
    private float inputHorizontal;
    private bool isDash;


    public PlayerHPValue health;

    public GameObject projectilePrefab;
    public GameObject playerAOEPrefab;
    public bool isUseAOE = false;

    [SerializeField] private Transform launchOffsetL;
    [SerializeField] private Transform launchOffsetR;

    [SerializeField] private float dashTime;
    [SerializeField] private float defaultTime;
    [SerializeField] private float walkSpeed = 13f;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float jumpForce = 45f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float coyoteTimer = 0f;

    private float lastDashTime = 0.0f;
    private bool isLaunch = false;
    private bool isDamaged;
    private bool deadWait;
    private bool respawnOrDead;
    private bool canLaunch = true;
    private bool canDash;

    internal bool isCastingSpell;
    internal bool isGrounded;
    internal bool isAttacking;

    public Ghost ghost;
    public float dashCooldown;


    private bool isCoroutineActive = false;

    void Start()
    {


        GameManager.Instance.gameOverDele += OnDeath;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimScr = GetComponent<PlayerAnimScr>();
        CastingSpellEffect = transform.GetChild(0).GetComponent<ParticleSystem>();

        Physics2D.IgnoreLayerCollision(7, 8);
        Physics2D.IgnoreLayerCollision(8, 8);
        Physics2D.IgnoreLayerCollision(9, 8);
    }

    void Awake()
    {
        health.PlayerHPInitialize();
    }

    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        Jump();
        Launch();
        ResetLaunch();
        CheckGrounded();


        if (Input.GetKeyDown(KeyCode.C) && Time.time >= lastDashTime + dashCooldown)
        {
            canDash = true;
            lastDashTime = Time.time;
        }
        Walk();
    }
    void FixedUpdate()
    {

        Walk();
        Dash();
        UpdateCoyoteTimer();
        CastingSpell();
        UseAOESkill();
    }
    public SpriteRenderer SpriteRenderer
    {
        get { return spriteRenderer; }
    }

    public void AddStat(EffectType type, float amount)
    {
        switch (type)
        {
            case EffectType.Heal_HP: HP = Mathf.Clamp(HP + amount, 0, MaxHP); break;
            case EffectType.Heal_MP: MP = Mathf.Clamp(MP + amount, 0, MaxMP); break;
            case EffectType.Atk_Up: Atk += amount; break;
        }
    }

    void Walk()
    {
        currentVelocity = new Vector2(inputHorizontal * walkSpeed, rb.velocity.y);


        if (!isCastingSpell && !isAttacking && inputHorizontal < 0 && !respawnOrDead)
        {
            playerAnimScr.WalkAnimation(true);
            rb.velocity = currentVelocity;
            spriteRenderer.flipX = true;

        }
        else if (!isCastingSpell && !isAttacking && inputHorizontal > 0 && !respawnOrDead)
        {
            playerAnimScr.WalkAnimation(true);
            rb.velocity = currentVelocity;
            spriteRenderer.flipX = false;

        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            playerAnimScr.WalkAnimation(false);

        }
    }
    void Jump()
    {

        if (Input.GetButton("Jump") && isGrounded && !isCastingSpell)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Dash()
    {
        if (canDash)
        {
            isDash = true;
        }
        if (dashTime <= 0)
        {
            ghost.makeGhost = false;
            rb.velocity = currentVelocity;
            if (isDash)
            {
                Physics2D.IgnoreLayerCollision(6, 7, true);
                dashTime = defaultTime;
            }
        }
        else
        {
            dashTime -= Time.deltaTime;

            if (spriteRenderer.flipX)
            {
                ghost.makeGhost = true;
                rb.velocity = new Vector2(dashSpeed * -1, rb.velocity.y);
            }
            else
            {
                ghost.makeGhost = true;
                rb.velocity = new Vector2(dashSpeed * 1, rb.velocity.y);
            }
        }

        isDash = false;
        canDash = false;


    }
    void Launch()
    {


        if (Input.GetKeyDown(KeyCode.Z) && !Input.GetKey(KeyCode.X) && inputHorizontal == 0 && !isAttacking && canLaunch)
        {
            isLaunch = true;
            if (!isGrounded)
            {
                playerAnimScr.AerialLaunchAnimation();
                Invoke("InstantiateProjectile", 0.4f);
            }
            else
            {
                playerAnimScr.LaunchAnimation();
                Invoke("InstantiateProjectile", 0.2f);
            }

            canLaunch = false;
            isAttacking = true;


            Invoke("LaunchExit", 0.7f);

            Invoke("ResetLaunch", 1.0f);
        }
        else isLaunch = false;
    }
    void InstantiateProjectile()
    {
        GameObject projectile;

        if (SpriteRenderer.flipX)
        {
            projectile = Instantiate(projectilePrefab, launchOffsetL.position, transform.rotation);
            projectile.GetComponent<Projectile>().SetDirection(Vector2.left);
        }
        else
        {
            projectile = Instantiate(projectilePrefab, launchOffsetR.position, transform.rotation);
            projectile.GetComponent<Projectile>().SetDirection(Vector2.right);
        }


        Destroy(projectile, 3.0f);
    }
    void LaunchExit()
    {
        isAttacking = false;
    }
    void ResetLaunch()
    {
        canLaunch = true;
    }
    void UseAOESkill()
    {
        if (playerAOEPrefab != null && !isUseAOE)
        {
            isUseAOE = true;
            Instantiate(playerAOEPrefab, transform.position, transform.rotation);
        }
    }
    void CastingSpell()
    {
        if (Input.GetKey(KeyCode.X) && inputHorizontal == 0)
        {
            if (!CastingSpellEffect.isPlaying && isGrounded)
            {
                playerAnimScr.CastingSpellAnimation(true);
                CastingSpellEffect.Play();
                isCastingSpell = true;
            }
        }
        else if (CastingSpellEffect.isPlaying)
        {
            playerAnimScr.CastingSpellAnimation(false);
            CastingSpellEffect.Stop();
            isCastingSpell = false;
        }
    }

    IEnumerator CoyoteTimeJump()
    {
        isCoroutineActive = true;
        yield return new WaitForSeconds(coyoteTime);
        Jump();
        isCoroutineActive = false;

        if (!isCoroutineActive && coyoteTimer > 0f)
        {
            StartCoroutine(CoyoteTimeJump());
        }
    }
    internal void CheckGrounded()
    {

        Vector2 groundRay = new Vector2(transform.position.x, GetComponent<Collider2D>().bounds.center.y);
        RaycastHit2D groundHit = Physics2D.Raycast(groundRay, Vector2.down, 1f, LayerMask.GetMask("groundLayer"));
        Debug.DrawRay(groundRay, Vector2.down * 1f, Color.green);

        if (groundHit.collider != null)
        {



            isGrounded = true;
        }
        else
        {


            isGrounded = false;
        }



    }










    void UpdateCoyoteTimer()
    {
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else if (coyoteTimer > 0f)
        {
            coyoteTimer -= Time.deltaTime;
        }
    }



    void OffDamaged()
    {
        Physics2D.IgnoreLayerCollision(6, 7, false);
        spriteRenderer.color = new Color(1, 1, 1, 1);
        isDamaged = false;

    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (isDamaged) return;

        int bumpForceDirc = transform.position.x - other.transform.position.x > 0 ? 1 : -1;

        if (other.gameObject.tag == "Enemy")
        {
            health.PlayerCurrentVal -= 10;
            isDamaged = true;

            rb.AddForce(new Vector2(bumpForceDirc, 0) * 50, ForceMode2D.Impulse);
            rb.velocity = new Vector2(bumpForceDirc * 40, 20);

            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            Physics2D.IgnoreLayerCollision(6, 7, true);
            Invoke("OffDamaged", 3f);
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (isDamaged) return;

        if (other.gameObject.CompareTag("Attack") || other.gameObject.CompareTag("Enemy"))
        {

            health.PlayerCurrentVal -= 10;
            isDamaged = true;

            int bumpForceDirc = transform.position.x - other.transform.position.x > 0 ? 1 : -1;
            rb.velocity = new Vector2(bumpForceDirc * 40, 20);

            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            Physics2D.IgnoreLayerCollision(6, 7, true);
            Invoke("OffDamaged", 3f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDamaged) return;

        if (other.gameObject.CompareTag("Attack") || other.gameObject.CompareTag("Enemy"))
        {

            health.PlayerCurrentVal -= 10;
            isDamaged = true;

            int bumpForceDirc = transform.position.x - other.transform.position.x > 0 ? 1 : -1;
            rb.velocity = new Vector2(bumpForceDirc * 40, 20);

            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            Physics2D.IgnoreLayerCollision(6, 7, true);
            Invoke("OffDamaged", 3f);
        }
    }





    public void OnDeath()
    {

        StartCoroutine(DeadJump());
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;


    }

    IEnumerator DeadJump()
    {
        respawnOrDead = true;


        Debug.Log("DeadJumpStart");
        playerAnimScr.DeadJumpAnimation(true);
        yield return new WaitForSeconds(1f);
        deadWait = true;
        Debug.Log("DeadJumpEnd");

        if (deadWait)
        {
            rb.AddForce(new Vector2(0, 1500f));
            rb.gravityScale = 8;
            gameObject.GetComponent<Collider2D>().enabled = false;

        }
    }
}
