using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Follower : MonoBehaviour
{
    [Header("외부 참조")]


    private IFollowerNumberCheck followerNumChecker;
    private IFollowerTargetReceivable followerTargetReceivable;
    private IFollowerAttackable followerAttackable;


    [SerializeField] private FollowerController followerController;


    private void Awake()
    {

        followerTargetReceivable = followerController;
        followerAttackable = followerController;
        followerNumChecker = TargetingAI.Instance;
    }

    private void OnEnable()
    {
        if (followerNumChecker == null)
        {
            return;
        }


        if (!followerNumChecker.IsFollowerRegistered(this))
        {
            followerNumChecker.AddFollower(this);
        }
    }

    private void OnDisable()
    {
        followerNumChecker?.RemoveFollower(this);
    }

    public bool IsDashCheck()
    {
        return followerController.IsDashCheck();
    }


    public void SetTarget(Enemy target)
    {
        followerTargetReceivable?.SetTarget(target);
    }

    public void CallDashAttack()
    {
        followerAttackable?.DashAndReturn();
    }
}
