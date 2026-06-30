using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSlot : MonoBehaviour
{
    public int slotIndex;
    private CharacterInstance slotCharacterInstance;

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI levelText;

    public void Bind(CharacterInstance instance)
    {
        slotCharacterInstance = instance;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slotCharacterInstance == null)
        {
            if (iconImage != null) { iconImage.sprite = null; iconImage.color = new Color(1, 1, 1, 0); }
            if (levelText != null) levelText.text = "";
            return;
        }

        if (iconImage != null)
        {
            iconImage.color = new Color(1, 1, 1, 1);
            iconImage.sprite = slotCharacterInstance.masterData.characterImage;
        }

        if (levelText != null)
        {
            levelText.text = "Lv." + slotCharacterInstance.level.ToString();
        }
    }
}
