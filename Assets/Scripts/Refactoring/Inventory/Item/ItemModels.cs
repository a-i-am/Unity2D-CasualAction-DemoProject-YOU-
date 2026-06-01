using System;
using System.Collections.Generic;
using UnityEngine;
public enum EffectType { Heal_HP, Heal_MP, Atk_Up, None };

[Serializable]
public class EffectData
{
    public EffectType type;
    public float value;
}

[Serializable]
public class ItemMasterData 
{
    public int itemID;
    public string type;
    public string itemName;
    public string explain;
    public int maxStack;

    [System.NonSerialized]
    public Sprite itemImage;

    public List<EffectData> effects = new List<EffectData>();

}

[Serializable]
public class ItemInstance
{
    public ItemMasterData masterData { get; private set; }
    public int count;                                     
    public int durability;                            

    public ItemInstance(ItemMasterData master, int amount)
    {
        this.masterData = master;
        this.count = amount;
        this.durability = 100;
    }
}
