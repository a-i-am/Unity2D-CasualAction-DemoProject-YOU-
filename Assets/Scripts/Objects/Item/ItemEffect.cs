using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract bool Execute(ItemUseContext context);
}
