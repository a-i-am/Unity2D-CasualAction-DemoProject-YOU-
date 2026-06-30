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
    [SerializeField] private Queue<Transform> emptySpawnQueue = new Queue<Transform>();
    private Transform spawnPos;

    private void Start()
    {
        InitializeEmptyPos();
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

    public void SpawnFollower(Character.CharacterData characterData)
    {
        if (emptySpawnQueue.Count == 0 || characterData == null || characterData.characterPrefab == null) return;

        spawnPos = emptySpawnQueue.Dequeue();
        spawnPos.gameObject.SetActive(true);

        follower = Instantiate(characterData.characterPrefab, spawnPos.position, Quaternion.identity, spawnPos);
        Debug.Log("팔로워 생성");

        if (!followerGroupMoving.enabled && follower != null)
        {
            followerGroupMoving.enabled = true;
        }
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
}
