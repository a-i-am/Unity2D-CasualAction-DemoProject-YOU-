using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static object _lock = new object();
    private static bool _isApplicationQuit = false;

    public static T Instance
    {
        get
        {
            // 고스트 객체 생성 방지용
            // leak 방지
            if (_isApplicationQuit)
            {
                return null;
            }

            // thread-safe
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).ToString() + " (Singleton)");
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(_instance);
                    }
                }
                return _instance;
            }


        }

    }

    protected virtual void OnApplicationQuit()
    {
        _isApplicationQuit = true;
    }

    // 객체가 파괴될 때 호출
    public virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}
