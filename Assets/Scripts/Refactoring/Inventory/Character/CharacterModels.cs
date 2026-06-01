using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterMasterData
{
    public int characterID;
    public string characterName;
    public string type;
    public Sprite characterImage;
    public GameObject characterPrefab;
}

[Serializable]
public class CharacterInstance
{
    public CharacterMasterData masterData;

    public int level;
    public int exp;
    public bool isSummoned;

    public CharacterInstance(CharacterMasterData master)
    {
        this.masterData = master;
        this.level = 1;
        this.exp = 0;
        this.isSummoned = false;
    }
}