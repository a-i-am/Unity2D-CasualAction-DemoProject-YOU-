using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseHandler : MonoBehaviour
{
    public static void UseItem(ItemInstance instance)
    {
        Player player = Player.Instance;
        
        if (player == null)
        {
            return; 
        }

        foreach (var effect in instance.masterData.effects)
        {
            player.AddStat(effect.type, effect.value);
        }

        instance.count--;

        if(instance.count <= 0)
        {
            // @TODO : dot(.)이 김. 리팩토링 필요한지 검토 필요
            Inventory.Instance.Items.Remove(instance);
        }
        else
        {
            Inventory.Instance.Items.ForceUpdate();
        }
    }    
}
