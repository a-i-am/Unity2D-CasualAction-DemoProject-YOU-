#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[InitializeOnLoad]
public static class RuntimeCounter
{
    private const string RunCountKey = "MyGame_EditorRunCount";
    static RuntimeCounter()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            int runCount = EditorPrefs.GetInt(RunCountKey, 0);
            runCount++;
            EditorPrefs.SetInt(RunCountKey, runCount);

            Debug.Log("에디터에서의 실행 횟수: " + runCount);
        }
    }
}
#endif
