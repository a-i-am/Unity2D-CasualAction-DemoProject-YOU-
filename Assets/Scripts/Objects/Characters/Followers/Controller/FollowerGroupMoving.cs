using System.Collections.Generic;
using UnityEngine;

public class FollowerGroupMoving : MonoBehaviour
{

    [SerializeField] private float moveDistance;
    [SerializeField] private float moveSpeed;
    private Transform player;
    public float startY;

    private Animator anim;
    private LayerMask groundLayer;



    [SerializeField] private float amplitude = 2f;
    [SerializeField] private float frequency = 1.0f;
    public bool isSineActive = true;
    private Collider2D lastGroundCollider;

    void Awake()
    {
        anim = GetComponent<Animator>();
        groundLayer = LayerMask.GetMask("groundLayer");
        Physics2D.IgnoreLayerCollision(7, 9);
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        FollowPlayer();
    }
    private void FixedUpdate()
    {
        Sine();
        ResetStartY();
    }

    private void FollowPlayer()
    {
        if (Mathf.Abs(transform.position.x - player.position.x) <= moveDistance) return;

        float direction = (player.position.x - transform.position.x) > 0 ? 1 : -1;
        transform.Translate(new Vector2(direction, 0) * Time.deltaTime * moveSpeed);
    }


    void Sine()
    {
        if (isSineActive)
        {
            float sineY = startY + Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = new Vector2(transform.position.x, sineY);
        }
    }

    void ResetStartY()
    {
        if (!isSineActive) return;

        Vector2 raycastStart = new Vector2(player.transform.position.x, player.transform.position.y - 2f);
        RaycastHit2D hit = Physics2D.Raycast(raycastStart, Vector2.down, 0.2f, LayerMask.GetMask("groundLayer"));
        Debug.DrawRay(raycastStart, Vector2.down * 0.2f, Color.magenta);

        if (hit.collider != null)
        {
            if (hit.collider != lastGroundCollider)
            {
                lastGroundCollider = hit.collider;
                startY = hit.point.y + 5f;
            }
        }
    }


}
