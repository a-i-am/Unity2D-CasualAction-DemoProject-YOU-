using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class FollowerController : MonoBehaviour, IFollowerTargetReceivable, IFollowerAttackable
{
    [Header("외부 참조")]
    [SerializeField] private FollowerGroupMoving followerGroupMoving;


    [Header("팔로잉")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 followPos;
    [SerializeField] private int followDelay;
    private Transform parent;
    private Queue<Vector3> parentPos;
    private Tween followTween;

    [SerializeField] private Ghost ghost;

    [Header("대시와 리턴")]
    private Vector3 originalPos;
    private int dashCount = 0;
    private float startY;
    private bool isDashing = false;
    [SerializeField] private float detectionRange;
    [SerializeField] private float dashDuration; // dash 속도 조절

    // 타겟팅
    private Enemy currentTarget;
    public Enemy CurrentTarget => currentTarget;
    private Vector2 targetPos;
    public Vector2 TargetPosition => currentTarget != null ? currentTarget.transform.position : Vector2.zero;
   
    public void SetTarget(Enemy target)
    {
        currentTarget = target;
    }

    // 렌더링
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        parentPos = new Queue<Vector3>();
    }
    private void Start()
    {
        if (playerTransform != null) return;
            playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void OnEnable()
    {
        if(parent == null)
        {
            parent = transform.parent;
        }
    }
    
    private void FixedUpdate()
    {
        // 스프라이트 기본 방향 : 오른쪽(기본 FlipX true 상태)
        spriteRenderer.flipX = (transform.localPosition.x < playerTransform.localPosition.x) ? true : false;
        
        if (!isDashing)
        {
            Watch();
            Follow();
        }
    }

    private void Watch()
    {
        // Input Pos
        if (!parentPos.Contains(parent.position))
        {
            parentPos.Enqueue(parent.position);
        }

        // Output Pos
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)
            followPos = parent.position;
    }

    private void Follow()
    {
        //transform.position = followPos;
        if(followTween != null && followTween.IsActive())
            followTween.Kill();

        followTween = transform.DOMove(followPos, 0.1f).SetEase(Ease.Linear);

    }

    public bool IsDashCheck()
    {
        return isDashing;
    }
    public void DashAndReturn()
    {
        if (isDashing || currentTarget == null || currentTarget.isFainted) return;
        #region 디버깅(null 체크)
        if (currentTarget == null)
        {
            Debug.LogWarning("DashAndReturn - target이 null!");
            return;
        }
        if (followerGroupMoving == null)
        {
            Debug.LogWarning("DashAndReturn - followerGroupMoving이 null!");
            return;
        }
        #endregion
        isDashing = true; // 동작 종료 전 재실행 불가

        // 타겟팅 정보 설정
        targetPos = TargetPosition;

        // 팔로워 위치 & 움직임 조정
        startY = followerGroupMoving.startY;
        followerGroupMoving.isSineActive = false;

        Sequence seq = DOTween.Sequence();
        ghost.makeGhost = true;
        seq.Append(transform.DOMove(targetPos, dashDuration))
           .AppendInterval(0.5f)
           .Append(transform.DOMove(followPos, dashDuration))
           .OnComplete(() =>
           {
               currentTarget = null;
               followerGroupMoving.isSineActive = true;
               isDashing = false;
               TargetingAI.Instance.ClearTargetHashSet();
               ghost.makeGhost = false;
           })
           .Play();

        ++dashCount;

    }

}
