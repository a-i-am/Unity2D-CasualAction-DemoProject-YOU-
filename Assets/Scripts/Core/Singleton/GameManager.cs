using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : Singleton<GameManager>
{


    public delegate void GameOverHandler();
    public event GameOverHandler gameOverDele;
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



        gameOverDele?.Invoke();





    }
    void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        isGameOver = false;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Replay);
    }
    void Start()
    {
        if (gameOverDele != null)
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
