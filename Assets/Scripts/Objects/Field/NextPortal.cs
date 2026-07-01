using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPortal : MonoBehaviour
{
    private bool isAllowEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            SceneController.Instance.NextLevel();
        }

        if (isAllowEnter && other.gameObject.layer == LayerMask.NameToLayer("PortalGuard"))
        {





            other.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        isAllowEnter = true;
    }

}
