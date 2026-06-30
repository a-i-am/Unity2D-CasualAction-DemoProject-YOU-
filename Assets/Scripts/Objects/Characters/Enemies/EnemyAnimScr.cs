using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyAnimScr : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
















    void Update()
    {
    }
    public void WalkAnimation(int walkSpeed)
    {
        anim.SetInteger("WalkSpeed", walkSpeed);
    }

    public void HurtAnimation()
    {
        anim.SetTrigger("Hurt");
    }

    public void FaintAnimation(bool isFaint)
    {
        anim.SetBool("IsFaint", isFaint);
    }

    public void RespawnAnimation()
    {
        anim.SetTrigger("IsRespawn");
    }

}
