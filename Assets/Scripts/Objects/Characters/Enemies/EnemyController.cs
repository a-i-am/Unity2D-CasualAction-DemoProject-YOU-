using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEditor.TerrainTools;

public class EnemyController : MonoBehaviour
{
    public event Action FaintedEvent;


    [Header("몬스터 이동")]
    private int nextMove;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float chaseDistance = 8f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float chaseSpeed = 15f;

    [Header("몬스터 스탯")]
    [SerializeField] private int health = 3;

    [Header("외부 참조")]
    [SerializeField] private Transform player;

    private int dashHitCount;
    public bool isFainted;
    private bool isHurted;

    private EnemyAnimScr enemyAnimScr;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rbEnemy;
    RaycastHit2D rayHit;
    [SerializeField] private ParticleSystem dashHitVFX;


    private void Awake()
    {


        player = GameObject.FindGameObjectWithTag("Player").transform;
        rbEnemy = GetComponent<Rigidbody2D>();
        enemyAnimScr = GetComponent<EnemyAnimScr>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(6, 6);
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {

        if (!isHurted && !isFainted)
        {
            rbEnemy.velocity = new Vector2(moveSpeed * nextMove, rbEnemy.velocity.y);
            GroundCheckRay();
            SpeedUpForChasePlayer();
        }
    }


    void Faint()
    {
        isFainted = true;
        gameObject.layer = 10;
        gameObject.tag = "Fainted";

        Physics2D.IgnoreLayerCollision(10, 7);
        Physics2D.IgnoreLayerCollision(10, 8);
        Physics2D.IgnoreLayerCollision(10, 6);

        enemyAnimScr.FaintAnimation(true);
        FaintedEvent.Invoke();
    }

    void Think()
    {

        nextMove = Random.Range(-1, 2);
        enemyAnimScr.WalkAnimation(nextMove);


        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;


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

        Vector2 frontCheck = new Vector2(rbEnemy.position.x + nextMove, rbEnemy.position.y);
        Debug.DrawRay(frontCheck, Vector2.down, Color.red);
        rayHit = Physics2D.Raycast(frontCheck, Vector2.down, 1, LayerMask.GetMask("groundLayer"));


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
