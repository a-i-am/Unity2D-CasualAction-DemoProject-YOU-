using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private IEnemyNumberCheck enemyNumberChecker;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private BossHelath bossHealth;

    [HideInInspector] public bool isFainted;



    public Character.CharacterData characterData;
    public void SetCharacter(Character.CharacterData character)
    {
        characterData = character;

    }
    public Character.CharacterData GetCharacter()
    {
        #region null 체크
        if (characterData == null)
        {
            Debug.LogError("GetCharacter returned null");
        }
        else if (characterData.characterPrefab == null)
        {
            Debug.LogError("GetCharacter returned a character with a null prefab");
        }
        #endregion
        return characterData;
    }
    public Follower GetCharacterPrefab()
    {
        return characterData.characterPrefab;
    }

    private void Awake()
    {
        enemyNumberChecker = TargetingAI.Instance;

        if (enemyNumberChecker != null && enemyController != null)
        {
            enemyController.FaintedEvent -= Remove;
            enemyController.FaintedEvent += Remove;
        }
        else if (bossHealth != null)
        {
            bossHealth.FaintedEvent -= Remove;
            bossHealth.FaintedEvent += Remove;
        }
    }

    private void OnEnable()
    {
        enemyNumberChecker?.AddActiveEnemy(this);
    }

    private void OnDisable()
    {
        Remove();
    }

    private void Remove()
    {
        isFainted = true;
        enemyNumberChecker?.RemoveActiveEnemy(this);
    }

}
