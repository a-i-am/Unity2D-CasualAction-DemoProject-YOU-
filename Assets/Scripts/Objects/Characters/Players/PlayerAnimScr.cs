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





        #endregion
        void Update()
        {

            inputHorizontal = Input.GetAxisRaw("Horizontal");

            if (Input.GetButton("Jump") && player.isGrounded)
            {
                anim.SetBool("Jump", true);
            }
            else if (Input.GetButtonUp("Jump"))
            { anim.SetBool("Jump", false); }

            #region isJumpingComment








            #endregion
        }


        public void WalkAnimation(bool IsWalk)
        {
            if (player.isGrounded)
            {

                anim.SetBool("IsWalk", IsWalk);
            }
        }



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








        }
    }
}
