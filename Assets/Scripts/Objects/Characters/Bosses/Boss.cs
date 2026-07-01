using System;
using UnityEngine;
public class Boss : MonoBehaviour
{

    [SerializeField] private GameObject nextPortal;
    [SerializeField] private ParticleSystem gushOutEffect;
    [SerializeField] private float speed;
    [SerializeField] private float followDistance;
    [SerializeField] private float gushoutDistance;
    [SerializeField] private float gushOutTimer;
    [SerializeField] private float chompTimer;
    [SerializeField] private float spinTimer;
    [SerializeField] private float turnTimer;

    [SerializeField] private float spinSpeed;
    [SerializeField] private GameObject gushOutEffectObj;
    [SerializeField] private bool isFlipped = false;
    private bool isFainted = false;
    private bool isDamaged = false;
    private bool isSpinning = false;
    private bool isSpinDirectionSet = false;
    private float followDirection;
    private float spinDirection;

    Animator anim;
    Transform player;
    Rigidbody2D rbBoss;

    public void SetFaint(bool faintState)
    {
        isFainted = faintState;

        if (isFainted)
        {

            anim.ResetTrigger("Crawl");
            anim.SetBool("GushOut", false);
            anim.SetBool("Chomp", false);
            anim.SetBool("Spin", false);


            rbBoss.velocity = Vector2.zero;

            gushOutEffect.Stop();
            gushOutEffectObj.SetActive(false);

            if(nextPortal != null)
                nextPortal.SetActive(true);
        }

    }


    private void ChangePositionToSleep()
    {

        transform.position = new Vector3(transform.position.x, transform.position.y - 2.2f, transform.position.z);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rbBoss = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gushOutEffect = transform.GetChild(0).GetComponent<ParticleSystem>();
    }


    private void Update()
    {
        if (isFainted || isDamaged)
        {

            return;
        }

        if (!isSpinning)
        {
            gushOutTimer += Time.deltaTime;
            chompTimer += Time.deltaTime;
        }
        else if (gushOutTimer >= 15f && chompTimer >= 15f)
        {
            spinTimer += Time.deltaTime;
        }

    }

    private void FixedUpdate()
    {
        if (isFainted)
        {

            return;
        }

        LookAtPlayer();
        followDirection = player.position.x < transform.position.x ? -1f : 1f;

        if (gushOutTimer >= 15f && chompTimer >= 15f)
        {
            gushOutEffect.Stop();
            gushOutEffectObj.gameObject.SetActive(false);
            if (!isFainted) Spin();
        }

        if (!isFainted && !isSpinning && Vector3.Distance(player.position, rbBoss.position) >= followDistance)
        {
            speed = 8f;
            anim.SetBool("GushOut", false);
            Chomp();
            if (!isSpinning)
                Follow();
        }
        else if (!isSpinning && Vector3.Distance(player.position, rbBoss.position) <= followDistance &&
            Vector3.Distance(player.position, rbBoss.position) >= gushoutDistance)
        {
            anim.SetBool("Chomp", false);
            if (!isFainted) GushOut();
        }


    }

    private void StopMoving()
    {
        isDamaged = true;

    }
    private void ReStartMoving()
    {
        isDamaged = false;
    }

    private void Spin()
    {
        if (isFainted || isDamaged) return;

        isSpinning = true;
        anim.SetBool("Spin", true);
        if (!isSpinDirectionSet)
        {
            spinDirection = player.position.x < transform.position.x ? -1f : 1f;
            isSpinDirectionSet = true;
        }

        rbBoss.velocity = new Vector2(spinDirection * spinSpeed, rbBoss.position.y);


        if (rbBoss.velocity.magnitude > spinSpeed)
            rbBoss.velocity = rbBoss.velocity.normalized * spinSpeed;

        if (spinTimer >= 10f)
        {
            anim.SetBool("Spin", false);
            isSpinning = false;


            gushOutTimer = 0f;
            chompTimer = 0f;
            spinTimer = 0f;


            isSpinDirectionSet = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("SpinDirectionReset")) spinDirection *= -1;
    }

    private void Follow()
    {
        if (isFainted || isDamaged) return;

        rbBoss.velocity = new Vector2(followDirection * speed, rbBoss.velocity.y);
        anim.SetTrigger("Crawl");
    }
    private void GushOut()
    {
        if (isFainted || isDamaged) return;

        if (!gushOutEffect.isPlaying && gushOutTimer < 15f)
        {

            rbBoss.AddForce(new Vector2(followDirection * 200f, 0f), ForceMode2D.Impulse);

            if (rbBoss.velocity.magnitude > 30f)
            {
                Debug.Log("Boss GushOut!");
                rbBoss.velocity = rbBoss.velocity.normalized * 30f;
            }

            anim.SetBool("GushOut", true);
            gushOutEffectObj.gameObject.SetActive(true);
            gushOutEffect.Play();
        }

    }

    private void Chomp()
    {
        if (isFainted || isDamaged) return;

        if (chompTimer < 15f)
        {
            gushOutEffect.Stop();
            gushOutEffectObj.gameObject.SetActive(false);
            anim.SetBool("Chomp", true);
        }
    }

    private void LookAtPlayer()
    {
        if (isFainted) return;

        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }




}
