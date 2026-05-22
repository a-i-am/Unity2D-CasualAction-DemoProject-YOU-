using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

namespace Assets
{
    public class PlayerAnimScr : MonoBehaviour
    {
        private Animator anim;
        private float inputHorizontal;
        Player player;
        void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            player = GetComponent<Player>();
        }
        #region MoveSpeedComment
        //public float MoveSpeed
        //{
        //    set => anim.SetFloat("movementSpeed", value);
        //    get => anim.GetFloat("movementSpeed");
        //}
        #endregion
        void Update()
        {
            // Walk Anim
            inputHorizontal = Input.GetAxisRaw("Horizontal");

            if (Input.GetButton("Jump") && player.isGrounded)
            {
                anim.SetBool("Jump", true);
            }
            else if (Input.GetButtonUp("Jump"))
            { anim.SetBool("Jump", false); }

            #region isJumpingComment
            //private bool isJumping = false; // 점프 모션이 실행 중인지 여부

            // SetBool("New Bool", false);
            //isJumping = true; // Jump 입력 시 점프 모션이 실행 중임을 설정
            //if (!isJumping) // 점프 모션이 실행 중이 아닐 때에만 Idle 모션으로 전환
            //{
            //    animator.SetBool("IdleAnimation", true);
            //}
            #endregion
        }


        public void WalkAnimation(bool IsWalk)
        {
            if (player.isGrounded)
            {
                //anim.SetTrigger("Walking");
                anim.SetBool("IsWalk", IsWalk);
            }
        }


        // DeadJump Anim(GameOver action 1)
        public void DeadJumpAnimation(bool isFallDead)
        {
            anim.SetBool("DeadJump", isFallDead);
        }

        public void LaunchAnimation()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                anim.SetTrigger("Launch");
            }
        }

        public void AerialLaunchAnimation()
        {
            if (!player.isGrounded)
            {
                anim.SetTrigger("AerialLaunch");
            }
        }

        public void CastingSpellAnimation(bool isCastingSpell)
        {

            anim.SetBool("CastingSpell", isCastingSpell);
            //if(!player.isAttacking && Input.GetKey(KeyCode.X))
            //{
            //    anim.SetTrigger("CastingSpell");
            //}
            //else if (Input.GetKeyUp(KeyCode.X)) 
            //{ anim.ResetTrigger("CastingSpell"); }


        }
    }
}