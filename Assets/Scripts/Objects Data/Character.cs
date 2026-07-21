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
        public string tabName, type, name, explain, number; // string이어야 JSON 파싱 시 잘 된다고 함
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
        public CharacterData CreateInstance()
        {
            CharacterData instance = (CharacterData)MemberwiseClone();
            instance.efts = efts == null ? null : new List<FollowerEffect>(efts);
            instance.slotIndex = -1;
            return instance;
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
                efts.Clear();  // 효과 리스트는 비우기만 (필요시 null 대입도 가능)
        }
    }
}
    //public void Initialize(Character.CharacterData data)
    //{
    //    characterData = data;
    //}
