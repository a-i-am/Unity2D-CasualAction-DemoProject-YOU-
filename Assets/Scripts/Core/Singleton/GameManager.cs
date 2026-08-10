using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public delegate void OnGameOverSignature();
    public event OnGameOverSignature OnGameOver;

    private bool isGameOver = false;

    private List<IUpdatable> updatables = new List<IUpdatable>();
    private List<IFixedUpdatable> fixedUpdatables = new List<IFixedUpdatable>();

    public Action keyAction = null;

    public void OnUpdate()
    {
        if (Input.anyKey == false) return;

        if (keyAction != null)
        {
            keyAction.Invoke();
        }
    }

    public void RegisterUpdatable(IUpdatable updatable)
    {
        updatables.Add(updatable);
    }

    public void RegisterFixedUpdatable(IFixedUpdatable fixedUpdatable)
    {
        fixedUpdatables.Add(fixedUpdatable);
    }

    public void GameOverDeath()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }

    public void OnDeath()
    {
        OnGameOver?.Invoke();
    }

    private void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        isGameOver = false;
    }

    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(Replay);
        }
    }

    private void Start()
    {
        if (OnGameOver != null)
        {
            isGameOver = true;
        }
    }

    private void Update()
    {
        foreach (var updatable in updatables)
        {
            updatable.OnUpdate();
        }
    }

    private void FixedUpdate()
    {
        foreach (var fixedUpdatable in fixedUpdatables)
        {
            fixedUpdatable.OnFixedUpdate();
        }
    }
}

