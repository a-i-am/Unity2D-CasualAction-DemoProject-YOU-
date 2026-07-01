using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DeathZoneScr : MonoBehaviour
{
    void Start(){


    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")){

            GameManager.Instance.OnDeath();
            Debug.Log("데스존 게임오버!");
        }









        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Debug.Log("적이 삭제되었습니다!");
        }
    }
}


