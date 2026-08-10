using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static readonly object _lock = new object();
    private static bool _isApplicationQuit = false;

    public static T Instance
    {
        get
        {
            if (_isApplicationQuit)
            {
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name + " (Singleton)");
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

    public virtual void OnDestroy()
    {
        _isApplicationQuit = true;
    }
}

