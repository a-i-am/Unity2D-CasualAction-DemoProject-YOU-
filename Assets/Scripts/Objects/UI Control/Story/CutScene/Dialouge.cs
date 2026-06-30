using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;

[System.Serializable]
public class DialougeArea
{
    [TextArea]
    public string dialogue;
    public Sprite actorIcon;

    public string actorName;
    public string title;
}

public class Dialouge : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    [SerializeField] private Image image_ActorIcon;
    [SerializeField] private Image image_IconBack;
    [SerializeField] private Image image_DialougeBox;
    [SerializeField] private TextMeshProUGUI txt_TitleText;
    [SerializeField] private TextMeshProUGUI txt_ActorName;
    [SerializeField] private TextMeshProUGUI txt_Dialogue;
    private bool isDialogue = false;
    private int count = 0;

    [SerializeField] private DialougeArea[] dialougeArea;

    public void ShowDialogue()
    {
        OnOff(true);
        count = 0;
        isDialogue = true;
        NextDialogue();
    }

    private void OnOff(bool _flag)
    {
        image_DialougeBox.gameObject.SetActive(_flag);
        image_ActorIcon.gameObject.SetActive(_flag);
        image_IconBack.gameObject.SetActive(_flag);
        txt_Dialogue.gameObject.SetActive(_flag);
        txt_TitleText.gameObject.SetActive(_flag);
        txt_ActorName.gameObject.SetActive(_flag);
        isDialogue = _flag;
    }


    private void NextDialogue()
    {
        txt_Dialogue.text = dialougeArea[count].dialogue;
        txt_TitleText.text = dialougeArea[count].title;
        txt_ActorName.text = dialougeArea[count].actorName;

        if (string.IsNullOrEmpty(dialougeArea[count].title))
        {
            image_ActorIcon.sprite = dialougeArea[count].actorIcon;
            image_IconBack.gameObject.SetActive(true);
            image_ActorIcon.gameObject.SetActive(true);
        }
        else
        {
            image_ActorIcon.gameObject.SetActive(false);
            image_IconBack.gameObject.SetActive(false);
        }
        count++;
    }

    void Start()
    {
        OnOff(false);
    }

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    public void StartTimeline()
    {
        director.time = director.time;
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }
    public void StopTimeline()
    {
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    void Update()
    {
        if(isDialogue)
        {
            if(Input.GetKeyDown(KeyCode.D))
            {
                if (count < dialougeArea.Length)
                    NextDialogue();
                else
                    OnOff(false);

            }
        }
    }
}
