using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Character
{
    [System.Serializable]
    public class CharacterData
    {
        public string tabName, type, name, explain, number;
        public bool isUsing;
        public int slotIndex;
        [JsonIgnore] public Sprite characterImage;
        [JsonIgnore] public Follower characterPrefab;
        [JsonIgnore] public List<FollowerEffect> efts;
        public CharacterData(string _type, string _name, string _explain, string _number, bool _isUsing, Sprite _characterImage, Follower _characterPrefab, string _tabName = "Character")
        {
            tabName = _tabName;
            type = _type;
            name = _name;
            explain = _explain;
            number = _number;
            characterImage = _characterImage;
            isUsing = _isUsing;
            characterPrefab = _characterPrefab;
        }
        public bool UseCharacter()
        {
            isUsing = false;

            foreach (FollowerEffect eft in efts)
            {
                if (eft == null) continue;
                isUsing = eft.ExecuteRole();
            }
            return isUsing;
        }

        public void Reset()
        {
            tabName = "Character";
            type = string.Empty;
            name = string.Empty;
            explain = string.Empty;
            number = string.Empty;
            isUsing = false;
            slotIndex = -1;

            characterImage = null;
            characterPrefab = null;

            if (efts != null)
                efts.Clear();
        }
    }
}
