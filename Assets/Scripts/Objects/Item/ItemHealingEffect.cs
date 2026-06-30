using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Health")]
public class ItemHealingEffect : ItemEffect
{
    public int healingPoint = 10;
    private PlayerHPValue playerHP;

    public void Init(PlayerHPValue _playerHP)
    {
        playerHP = _playerHP;
    }

    public override bool ExecuteRole()
    {
        if (playerHP == null) return false;
        playerHP.PlayerCurrentVal += healingPoint;
        return true;
    }
}
