using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Managers : MonoBehaviour
{
    static Managers s_Instance;
    public static Managers Instance { get { return s_Instance; } }

    InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    public static void Init()
    {
        if (Instance == null)
        {
            GameObject go = GameObject.FindWithTag("Managers");
            if ((go == null))
            {
                go = new GameObject { name = "Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            s_Instance = go.GetComponent<Managers>();
        }
    }

    void Update()
    {

    }
}
