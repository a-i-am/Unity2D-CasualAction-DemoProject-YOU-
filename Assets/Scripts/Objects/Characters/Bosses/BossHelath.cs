using System;
using UnityEngine;

public class BossHelath : MonoBehaviour
{
    public event Action FaintedEvent;

    [SerializeField] private BossHPValue bossHealth;

    private Boss boss;
    private bool bossIsHurted = false;
    private bool bossIsFainted;
    Transform player;
    Rigidbody2D rbBoss;

    Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        boss = GetComponent<Boss>();
        rbBoss = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Awake()
    {
        bossHealth.BossHPInitialize();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!bossIsFainted && !bossIsHurted && collision.gameObject.tag == "Attack")
        {
            TakeDamage();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!bossIsFainted && !bossIsHurted && other.gameObject.CompareTag("Attack"))
        {
            TakeDamage();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
       if (other.gameObject.CompareTag("Follower") && other.gameObject.layer == LayerMask.NameToLayer("Follower"))
       {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        Invoke("OffDamagedState", 0.5f);
        bossIsHurted = true;
        anim.SetTrigger("Hurt");

        bossHealth.BossCurrentVal -= 50;
        if (bossHealth.BossCurrentVal <= 0)
        {
            Faint();
        }
        else
        {
            rbBoss.velocity = Vector2.zero;
            if (transform.position.x > player.transform.position.x)
            {
                rbBoss.velocity = new Vector2(10f, 0);
            }
            else
            {
                rbBoss.velocity = new Vector2(-10f, 0);
            }
        }
    }
    void OffDamagedState()
    {
        bossIsHurted = false;
    }


    void Faint()
    {
        FaintedEvent.Invoke();
        boss.SetFaint(true);


        gameObject.layer = 10;
        Physics2D.IgnoreLayerCollision(10, 7);
        Physics2D.IgnoreLayerCollision(10, 8);
        Physics2D.IgnoreLayerCollision(10, 6);
        Physics2D.IgnoreLayerCollision(10, 9);

        bossIsFainted = true;
        anim.SetTrigger("Faint");
        anim.SetTrigger("Sleep");
        rbBoss.constraints = RigidbodyConstraints2D.FreezeAll;
        Debug.Log("Enemy Knock Down-!!");
    }
}
