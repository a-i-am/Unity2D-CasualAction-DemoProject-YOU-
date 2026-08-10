using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

namespace Assets
{
    public class PlayerAnimScr : MonoBehaviour
    {
        private Animator anim;
        private float inputHorizontal;
        private PlayerScr player;

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            player = GetComponent<PlayerScr>();
        }

        private void Update()
        {
            inputHorizontal = Input.GetAxisRaw("Horizontal");

            if (Input.GetButton("Jump") && player.isGrounded)
            {
                anim.SetBool("Jump", true);
            }
            else if (Input.GetButtonUp("Jump"))
            {
                anim.SetBool("Jump", false);
            }
        }

        public void WalkAnimation(bool isWalk)
        {
            if (player.isGrounded)
            {
                anim.SetBool("IsWalk", isWalk);
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