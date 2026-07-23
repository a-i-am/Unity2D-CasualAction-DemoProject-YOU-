using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPortal : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    private bool isAllowEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (string.IsNullOrEmpty(targetSceneName))
                SceneController.Instance.NextLevel();
            else
                SceneController.Instance.LoadScene(targetSceneName);
        }

        if (isAllowEnter && other.gameObject.layer == LayerMask.NameToLayer("PortalGuard"))
        {
            /*
             * 보스전 클리어 등, 포탈이 비활성화 되었다가 나타난 위치에 플레이어가 있으면
             * 예고도 없이 다음 씬으로 이동될 수 있다. 이를 방지하기 위해 Collider 범위를 포탈 영역에 맞닿게 해서
             * 플레이어가 클리어 전까지는 물리적으로 해당 포탈 위치에 접근하지 못하게 막아둔다. 
            */
            other.gameObject.SetActive(false); // 포탈 강제이동 방지용 콜라이더 개체 비활성화
        }
    }

    private void OnEnable()
    {
        isAllowEnter = true;
    }

}
