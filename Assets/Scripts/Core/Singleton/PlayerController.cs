using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Mesh;
#region 플레이어 로직 요약
//모든 플레이어는
// 지면 판정이 존재하고,

// 애니메이션이 존재하고 // 애니메이션 유형화

// 죽고(죽지 않고, 키입력이 있으면) // 죽음/데드점프/리스폰 유형화
// 죽을 때, 접속 이상일 때
// 데드존에 걸리면 데드점프 되고 
// 리스폰 되고

// 공격/피격 판정이 있고 // 공격, 피격 유형화
// 피격 시 데미지를 입어야 하고

// 기를 모아야 하고 // 기 유형화
// 4단필(궁) 사용 판정이 있고 // 궁 유형화
// 이동, 점프&코루틴타임점프, 대시&대시쿨다운하고
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
    public float dashCooldown; // 대시 후 대시가 다시 가능해지는 시간 (초 단위)
    public float lastDashTime = 0.0f; // 마지막 대시 입력 시간을 기록하는 변수
    public float dashTime;
    public float defaultTime;
}
[Serializable]
public class CoyoteTimeJump
{
    //코요태타임점프(코루틴을 사용한 지연 점프)
    public float coyoteTime = 0.1f; // 코요태 점프 타임
    public float coyoteTimer = 0f; // 코요태 점프 타이머
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
    LayerMask groundLayer; // Ground 레이어를 가진 오브젝트와의 충돌을 감지
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    Vector2 currentVelocity;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        //Managers.Init();  // Managers 초기화 보장
    }

    void Start()
    {
        //Managers.Input.keyAction -= OnKeyboard;
        //Managers.Input.keyAction += OnKeyboard;
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
        // 플레이어의 앞 방향으로 레이캐스트를 발사하여 적을 감지합니다.

        if (inputHorizontal != 0)
        {
            Walk();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= dashTimeSet.lastDashTime + dashTimeSet.dashCooldown)
        {
            canDash = true;
            dashTimeSet.lastDashTime = Time.time; // 현재 시간을 기록
        }
        if (Input.GetButton("Jump")) //  && isGrounded && !isCastingSpell
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
            // 이동
            currentVelocity = new Vector2(inputHorizontal * playerPhysics.walkSpeed, rb.velocity.y);
            rb.velocity = currentVelocity;
        }
        else
        {
            // 입력이 없을 때는 수평 속도를 0으로 설정하여 멈추게 함
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // 방향에 맞게 스프라이트 반전
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
                Physics2D.IgnoreLayerCollision(6, 7, true); // 보스 스핀 공격 회피 가능(gush out은 시간차로 못 피함)
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
