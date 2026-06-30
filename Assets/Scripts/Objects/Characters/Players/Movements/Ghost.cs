using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{

    [SerializeField] private float ghostDelay;
    private float ghostDelaySeconds;
    [SerializeField] private float destroyTime;
    public GameObject ghost;
    public bool makeGhost = false;

    private SpriteRenderer spriteRenderer;
    void Start()
    {
        ghostDelaySeconds = ghostDelay;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        if (makeGhost)
        {
            if (ghostDelaySeconds > 0)
            {
                ghostDelaySeconds -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;


                currentGhost.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;

                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                ghostDelaySeconds = ghostDelay;
                Destroy(currentGhost, destroyTime);
                Invoke("IgnoreDamage", 0.5f);
            }
        }
    }

    void IgnoreDamage()
    {
        Physics2D.IgnoreLayerCollision(6, 7, false);
    }
}
