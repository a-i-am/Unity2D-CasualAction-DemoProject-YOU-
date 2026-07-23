using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    [SerializeField] Animator transitionAnim;

    // 씬 전환(포탈로 맵 이동)
    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    IEnumerator LoadLevel(string sceneName = null)
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        if (string.IsNullOrEmpty(sceneName))
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadSceneAsync(sceneName);
        transitionAnim.SetTrigger("Start");
    }

}
