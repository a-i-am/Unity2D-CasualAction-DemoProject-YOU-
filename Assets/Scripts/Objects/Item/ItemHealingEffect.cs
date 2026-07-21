using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Health")]
public class ItemHealingEffect : ItemEffect
{
    public int healingPoint = 10;

    public override bool Execute(ItemUseContext context)
    {
        if (context == null || context.Health == null) return false;
        context.Health.PlayerCurrentVal += healingPoint;
        return true;
    }
}
