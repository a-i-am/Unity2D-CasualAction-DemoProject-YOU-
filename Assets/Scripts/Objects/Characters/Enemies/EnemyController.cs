using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public event Action FaintedEvent;


    [Header("몬스터 이동")]
    private int nextMove;
    [SerializeField] private float moveSpeed = 5f; // 몬스터의 이동 속도
    [SerializeField] private float chaseDistance = 8f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float chaseSpeed = 15f; // 몬스터가 플레이어를 빨리 쫓아갈 때 속도

    [Header("몬스터 스탯")]
    [SerializeField] private int health = 3; // 몬스터의 체력

    [Header("외부 참조")]
    [SerializeField] private Transform player; // 플레이어의 Transform을 저장하는 변수
    
    private int dashHitCount;
    public bool isFainted;
    private bool isHurted;
    
    private EnemyAnimScr enemyAnimScr;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rbEnemy;
    RaycastHit2D rayHit;
    [SerializeField] private ParticleSystem dashHitVFX;
    //private bool enemyIsGrounded;

    private void Awake()
    {
        //dashHitVFX = GetComponentInChildren<ParticleSystem>();
        // Player 오브젝트를 찾아서 player에 할당
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rbEnemy = GetComponent<Rigidbody2D>();
        enemyAnimScr = GetComponent<EnemyAnimScr>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(6, 6); // Enemy 끼리 충돌 방지
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        // Move
        if (!isHurted && !isFainted)
        {
            rbEnemy.velocity = new Vector2(moveSpeed * nextMove, rbEnemy.velocity.y);
            GroundCheckRay();
            SpeedUpForChasePlayer();
        }
    }

    // Enemy 녹다운
    void Faint()
    {
        isFainted = true;
        gameObject.layer = 10; // "Fainted"
        gameObject.tag = "Fainted";

        Physics2D.IgnoreLayerCollision(10, 7); // Fainted(10)과 Player(7) 충돌 무시
        Physics2D.IgnoreLayerCollision(10, 8); // Fainted(10)과 Attack(8) 충돌 무시 
        Physics2D.IgnoreLayerCollision(10, 6); // Fainted(10)과 Enemy(6)  충돌 무시

        enemyAnimScr.FaintAnimation(true);
        FaintedEvent.Invoke();
    }

    void Think()
    {
        // Set Next Active
        nextMove = Random.Range(-1, 2);
        enemyAnimScr.WalkAnimation(nextMove);

        // Flip Sprite
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        // Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void SpeedUpForChasePlayer()
    {
        if (Vector2.Distance(rbEnemy.position, player.position) < chaseDistance)
        {
            moveSpeed = chaseSpeed;

            if (Vector2.Distance(rbEnemy.position, player.position) < stopDistance)
            {
                moveSpeed = 0;
            }
        }
    }

    void GroundCheckRay()
    {
        // 앞에 땅이 있는지 체크
        Vector2 frontCheck = new Vector2(rbEnemy.position.x + nextMove, rbEnemy.position.y);
        Debug.DrawRay(frontCheck, Vector2.down, Color.red); // 레이를 시각적으로 표시
        rayHit = Physics2D.Raycast(frontCheck, Vector2.down, 1, LayerMask.GetMask("groundLayer"));

        // 앞에 땅 없으면 방향 전환
        if (rayHit.collider == null)
        {
            Turn();
        }
    }
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 2);
    }

    public void TakeDamage()
    {
        isHurted = true;
        enemyAnimScr.HurtAnimation();
        --health;

        if (health <= 0)
        {
            Faint();
        }
        else
        {
            if (transform.position.x > player.transform.position.x)
            {
                rbEnemy.velocity = Vector2.zero;
                rbEnemy.velocity = new Vector2(10f, 0);
            }
            else
            {
                rbEnemy.velocity = Vector2.zero;
                rbEnemy.velocity = new Vector2(-10f, 0);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Follower") && !isFainted
            || other.gameObject.CompareTag("Attack"))
        {
            TakeDamage();
            //dashHitVFX.Play();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Follower") && !isFainted
            || other.gameObject.CompareTag("Attack"))
        {
            TakeDamage();
            dashHitVFX.Play();
        }
    }
}
