using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : Singleton<GameManager>
{
    private const string GameOverPanelName = "GameOverPanel";

    // 델리게이트
    // 플레이어 이벤트
    public delegate void GameOverHandler(); // = delegate void = Action 
    public event GameOverHandler gameOverDele;
    private bool isGameOver = false;
    private GameObject gameOverUI;
    private Button replayButton;

    // 키 인풋
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

    // 게임 오버 & 리플레이 로직(델리게이트)
    public void GameOverDeath()
    {
        SetGameOverUI(true);
    }
    public void OnDeath()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverDele?.Invoke();
        GameOverDeath();
    }
    void Replay()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        SetGameOverUI(false);

        if (PlayerScr.Instance != null)
        {
            PlayerScr.Instance.RespawnNearSafePosition();
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // = SceneManager.LoadScene("2D Scene");
    }

    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(Replay);
    }
    void Start()
    {
        CacheGameOverUI();
        SetGameOverUI(false);
    }

    private void CacheGameOverUI()
    {
        if (gameOverUI == null)
        {
            gameOverUI = FindSceneObject(GameOverPanelName);
        }

        if (gameOverUI == null)
        {
            CreateGameOverUI();
        }

        if (gameOverUI == null) return;

        Button button = gameOverUI.GetComponentInChildren<Button>(true);
        if (button != null && button != replayButton)
        {
            replayButton = button;
            replayButton.onClick.RemoveListener(Replay);
            replayButton.onClick.AddListener(Replay);
        }
    }

    private void SetGameOverUI(bool active)
    {
        CacheGameOverUI();
        if (gameOverUI != null) gameOverUI.SetActive(active);
    }

    private static GameObject FindSceneObject(string objectName)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.name == objectName && go.scene.IsValid())
            {
                return go;
            }
        }

        return null;
    }

    private void CreateGameOverUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>(true);
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("GameOver Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;

        GameObject panel = new GameObject(GameOverPanelName, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.65f);

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Text title = CreateText("Title", panel.transform, "GAME OVER", font, 64);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0f, 70f);
        titleRect.sizeDelta = new Vector2(600f, 90f);

        GameObject buttonObj = new GameObject("RestartButton", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObj.transform.SetParent(panel.transform, false);
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0f, -45f);
        buttonRect.sizeDelta = new Vector2(260f, 70f);
        buttonObj.GetComponent<Image>().color = new Color(1f, 0.42f, 0.08f, 0.95f);

        Text buttonText = CreateText("Text", buttonObj.transform, "RESTART", font, 34);
        RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        gameOverUI = panel;
    }

    private static Text CreateText(string name, Transform parent, string text, Font font, int size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        Text label = obj.GetComponent<Text>();
        label.text = text;
        label.font = font;
        label.fontSize = size;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        return label;
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


//    public void TabClick(string tabName)
//    {
//    }

//    public void SendMessageToChat(string text)
//    {
//        if (messageList.Count >= maxMessages)
//            messageList.Remove(messageList[0]);

//        Message newMessage = new Message();

//        newMessage.text = text;

//        messageList.Add(newMessage);
//    }

//    [System.Serializable]
//    public class Message
//    {
//        public string text;
//    }
