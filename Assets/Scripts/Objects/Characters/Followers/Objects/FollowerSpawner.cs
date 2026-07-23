using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerSpawner : MonoBehaviour
{
    [Header("외부 참조")]
    [SerializeField] private FollowerGroupMoving followerGroupMoving;

    private Follower follower;
    [Header("팔로워 자리")]
    [SerializeField] private List<Transform> followerPositions;
    [SerializeField] private Queue<Transform> emptySpawnQueue = new Queue<Transform>(); // 팔로워 공석 체크
    private Transform spawnPos;

    private void Start()
    {
        InitializeEmptyPos();
        RestoreFollowers();
    }
    private void InitializeEmptyPos()
    {
        emptySpawnQueue.Clear();

        foreach (Transform pos in followerPositions)
        {
            if (pos.childCount != 0 || !pos.gameObject.activeSelf) continue;
            
            emptySpawnQueue.Enqueue(pos);
            pos.gameObject.SetActive(false);
        }
    }

    public bool SpawnFollower(Character.CharacterData characterData)
    {
        if (emptySpawnQueue.Count == 0 || characterData == null || characterData.characterPrefab == null) return false;

        spawnPos = emptySpawnQueue.Dequeue();
        spawnPos.gameObject.SetActive(true);

        follower = Instantiate(characterData.characterPrefab, spawnPos.position, Quaternion.identity, spawnPos);
        follower.gameObject.tag = "Follower";
        follower.gameObject.layer = LayerMask.NameToLayer("Follower");
        foreach (Collider2D followerCollider in follower.GetComponentsInChildren<Collider2D>())
            followerCollider.isTrigger = true;
        Debug.Log("팔로워 생성");

        if (!followerGroupMoving.enabled && follower != null)
        {
            followerGroupMoving.enabled = true;
        }

        return true;
    }

    private void EnqueueSpawnPos(Follower follower)
    {
        if (spawnPos != null)
        {
            spawnPos.gameObject.SetActive(false);
            emptySpawnQueue.Enqueue(spawnPos);
        }
        spawnPos = null;
    }

    private void RestoreFollowers()
    {
        Inventory inventory = Inventory.Instance;
        if (inventory == null) return;

        foreach (Character.CharacterData character in inventory.SavedFollowers)
            SpawnFollower(character);
    }
}
