using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEditor.ShaderGraph.Internal.Texture2DShaderProperty;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Mesh;
#region 플레이어 로직 요약
















#endregion
[Serializable]
public class PlayerAsset
{
    [SerializeField] Transform launchOffsetL;
    [SerializeField] Transform launchOffsetR;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject playerAOEPrefab;
    [SerializeField] ParticleSystem CastingSpellEffect;
}
[Serializable]
public class PlayerPhysics
{
    public float walkSpeed;
    public float dashSpeed;
    public float jumpForce;
}
[Serializable]
public class DashTimeSet
{
    public float dashCooldown;
    public float lastDashTime = 0.0f;
    public float dashTime;
    public float defaultTime;
}
[Serializable]
public class CoyoteTimeJump
{

    public float coyoteTime = 0.1f;
    public float coyoteTimer = 0f;
    public bool isCoroutineActive = false;
}

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private PlayerPhysics playerPhysics;
    [SerializeField] private DashTimeSet dashTimeSet;
    [SerializeField] private CoyoteTimeJump coyoteTimeJump;
    [SerializeField] private PlayerAsset playerAsset;
    [SerializeField] ObjectPoolManager projectilePool;
    [SerializeField] Ghost ghost;

    EnemyState enemyState;


    bool isGrounded;
    bool isCastingSpell;
    bool deadWait;
    bool respawnOrDead;

    bool isDash;
    bool canDash;

    float inputHorizontal;
    LayerMask groundLayer;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    Vector2 currentVelocity;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();


    }

    void Start()
    {


    }

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Dash();
    }

    void OnKeyboard()
    {
        inputHorizontal = Input.GetAxis("Horizontal");


        if (inputHorizontal != 0)
        {
            Walk();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= dashTimeSet.lastDashTime + dashTimeSet.dashCooldown)
        {
            canDash = true;
            dashTimeSet.lastDashTime = Time.time;
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

    void Walk()
    {
        if (inputHorizontal != 0)
        {

            currentVelocity = new Vector2(inputHorizontal * playerPhysics.walkSpeed, rb.velocity.y);
            rb.velocity = currentVelocity;
        }
        else
        {

            rb.velocity = new Vector2(0f, rb.velocity.y);
        }


        spriteRenderer.flipX = inputHorizontal < 0;
    }

    void Dash()
    {
        if (canDash)
        {
            isDash = true;
        }
        if (dashTimeSet.dashTime <= 0)
        {
            ghost.makeGhost = false;
            rb.velocity = currentVelocity;
            if (isDash)
            {
                Physics2D.IgnoreLayerCollision(6, 7, true);
                dashTimeSet.dashTime = dashTimeSet.defaultTime;
            }
        }
        else
        {
            dashTimeSet.dashTime -= Time.deltaTime;

            if (spriteRenderer.flipX)
            {
                ghost.makeGhost = true;
                rb.velocity = new Vector2(playerPhysics.dashSpeed * -1, rb.velocity.y);
            }
            else
            {
                ghost.makeGhost = true;
                rb.velocity = new Vector2(playerPhysics.dashSpeed * 1, rb.velocity.y);
            }
        }

        isDash = false;
        canDash = false;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, playerPhysics.jumpForce);
    }

    void Launch()
    {

    }

    void PickUpItem()
    {

    }

    void PickUpCharacter()
    {

    }
}

