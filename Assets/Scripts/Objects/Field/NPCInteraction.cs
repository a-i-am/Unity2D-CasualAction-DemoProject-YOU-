using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    Animator[] animators;
    [SerializeField] GameObject barrier;
    [SerializeField] GameObject barrierVFX;

    private bool barrierCreated;
    void Start()
    {

        animators = GetComponentsInChildren<Animator>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && !barrierCreated)
        {
            barrierCreated = true;
            Instantiate(barrierVFX, null);
            Invoke("InstantiateBarrier", 1.2f);
            Invoke("ActAnimation", 1.2f);
        }
    }

    void ActAnimation()
    {

        foreach (Animator animator in animators)
        {
            animator.SetBool("IsActing", true);
        }
    }

    void InstantiateBarrier()
    {
        Instantiate(barrier, null);
    }


}
