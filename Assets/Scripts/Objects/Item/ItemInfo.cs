
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public string category;
    public string name;
    public string description;
    public int maxStack;
    public bool isUsable;
    public string spritePath;
    public string spriteName;

    [JsonIgnore]
    public Sprite itemSprite;
}
