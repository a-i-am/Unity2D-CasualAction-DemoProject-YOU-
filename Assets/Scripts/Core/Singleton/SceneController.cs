using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    [SerializeField] private Animator transitionAnim;

    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger("End");
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger("Start");
        }
    }
}

