using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAsset
{
    [SerializeField] private Transform launchOffsetL;
    [SerializeField] private Transform launchOffsetR;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject playerAOEPrefab;
    [SerializeField] private ParticleSystem castingSpellEffect;
}

[Serializable]
public class PlayerPhysics
{
    [SerializeField] private float walkSpeed = 13f;
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float jumpForce = 45f;

    public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    public float DashSpeed { get => dashSpeed; set => dashSpeed = value; }
    public float JumpForce { get => jumpForce; set => jumpForce = value; }
}

[Serializable]
public class DashTimeSet
{
    [SerializeField] private float dashCooldown = 1.0f;
    [SerializeField] private float lastDashTime = 0.0f;
    [SerializeField] private float dashTime;
    [SerializeField] private float defaultTime = 0.2f;

    public float DashCooldown { get => dashCooldown; set => dashCooldown = value; }
    public float LastDashTime { get => lastDashTime; set => lastDashTime = value; }
    public float DashTime { get => dashTime; set => dashTime = value; }
    public float DefaultTime { get => defaultTime; set => defaultTime = value; }
}

[Serializable]
public class CoyoteTimeJump
{
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float coyoteTimer = 0f;
    [SerializeField] private bool isCoroutineActive = false;

    public float CoyoteTime { get => coyoteTime; set => coyoteTime = value; }
    public float CoyoteTimer { get => coyoteTimer; set => coyoteTimer = value; }
    public bool IsCoroutineActive { get => isCoroutineActive; set => isCoroutineActive = value; }
}

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private PlayerPhysics playerPhysics;
    [SerializeField] private DashTimeSet dashTimeSet;
    [SerializeField] private CoyoteTimeJump coyoteTimeJump;
    [SerializeField] private PlayerAsset playerAsset;
    [SerializeField] private ObjectPoolManager projectilePool;
    [SerializeField] private Ghost ghost;

    private EnemyState enemyState;

    private bool isGrounded;
    private bool isCastingSpell;
    private bool deadWait;
    private bool respawnOrDead;

    private bool isDash;
    private bool canDash;

    private float inputHorizontal;
    private LayerMask groundLayer;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private Vector2 currentVelocity;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
    }

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Dash();
    }

    private void OnKeyboard()
    {
        inputHorizontal = Input.GetAxis("Horizontal");

        if (inputHorizontal != 0)
        {
            Walk();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= dashTimeSet.LastDashTime + dashTimeSet.DashCooldown)
        {
            canDash = true;
            dashTimeSet.LastDashTime = Time.time;
        }
        if (Input.GetButton("Jump"))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Launch();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PickUpItem();
        }
        if (Input.GetKeyDown(KeyCode.C) && enemyState != EnemyState.Fainted)
        {
            PickUpCharacter();
        }
    }

    private void Walk()
    {
        if (inputHorizontal != 0)
        {
            currentVelocity = new Vector2(inputHorizontal * playerPhysics.WalkSpeed, rb.velocity.y);
            rb.velocity = currentVelocity;
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        spriteRenderer.flipX = inputHorizontal < 0;
    }

    private void Dash()
    {
        if (canDash)
        {
            isDash = true;
        }
        if (dashTimeSet.DashTime <= 0)
        {
            ghost.makeGhost = false;
            rb.velocity = currentVelocity;
            if (isDash)
            {
                Physics2D.IgnoreLayerCollision(6, 7, true);
                dashTimeSet.DashTime = dashTimeSet.DefaultTime;
            }
        }
        else
        {
            dashTimeSet.DashTime -= Time.deltaTime;

            if (spriteRenderer.flipX)
            {
                ghost.makeGhost = true;
                rb.velocity = new Vector2(playerPhysics.DashSpeed * -1, rb.velocity.y);
            }
            else
            {
                ghost.makeGhost = true;
                rb.velocity = new Vector2(playerPhysics.DashSpeed * 1, rb.velocity.y);
            }
        }

        isDash = false;
        canDash = false;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, playerPhysics.JumpForce);
    }

    private void Launch()
    {
    }

    private void PickUpItem()
    {
    }

    private void PickUpCharacter()
    {
    }
}


