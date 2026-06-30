using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public int slotIndex;
    private ItemInstance slotItemInstance;

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public void Bind(ItemInstance instance)
    {
        slotItemInstance = instance;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slotItemInstance == null || slotItemInstance.count <= 0)
        {
            if (iconImage != null) { iconImage.sprite = null; iconImage.color = new Color(1, 1, 1, 0); }
            if (countText != null) countText.text = "";
            return;
        }

        if (iconImage != null)
        {
            iconImage.color = new Color(1, 1, 1, 1);
            iconImage.sprite = slotItemInstance.masterData.itemImage;
        }

        if (countText != null)
        {
            countText.text = slotItemInstance.count.ToString();
        }
    }

    public void OnClickSlot()
    {
        if (slotItemInstance != null)
        {
            ItemUseHandler.UseItem(slotItemInstance);
        }
    }
}
