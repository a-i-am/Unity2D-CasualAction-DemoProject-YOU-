using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.EventSystems.EventTrigger;
public class TargetingAI : Singleton<TargetingAI>, IFollowerNumberCheck, IEnemyNumberCheck
{






    private List<Follower> _activeFollowers = new List<Follower>();



    private HashSet<Enemy> _activeEnemies = new HashSet<Enemy>();


    private HashSet<Enemy> _detectedEnemies = new HashSet<Enemy>();
    private SortedSet<(float, Enemy)> _targetCandidates = new SortedSet<(float, Enemy)>(new TargetComparer());
    private HashSet<Enemy> _targetHashSet = new HashSet<Enemy>();

    private Dictionary<Collider2D, Enemy> _enemyCache = new Dictionary<Collider2D, Enemy>();

    public bool IsFollowerRegistered(Follower follower)
    {
        return _activeFollowers.Contains(follower);
    }

    public void AddFollower(Follower follower)
    {
        if (!IsFollowerRegistered(follower))
        {
            _activeFollowers.Add(follower);
        }
    }

    public void RemoveFollower(Follower follower)
    {
        if(follower == null) return;
        _activeFollowers.Remove(follower);
    }







    public void ClearTargetHashSet()
    {
        _targetHashSet.Clear();
    }

    public void AddActiveEnemy(Enemy enemy)
    {
        _activeEnemies.Add(enemy);
    }


    public void RemoveActiveEnemy(Enemy enemy)
    {
        _activeEnemies.Remove(enemy);
        _targetHashSet.Remove(enemy);
    }

    public void EnterEnemy(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy) || _detectedEnemies.Contains(enemy)) return;
        _detectedEnemies.Add(enemy);


    }
    public void ExitEnemy(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy) || !_detectedEnemies.Contains(enemy)) return;

        _detectedEnemies.Remove(enemy);
        _targetHashSet.Remove(enemy);
    }

    private Enemy GetEnemyFromCollider(Collider2D collider)
    {
        if(collider == null) return null;

        if(_enemyCache.TryGetValue(collider, out Enemy enemy))
        {
            return enemy;
        }

        if (collider.TryGetComponent(out enemy))
        {
            _enemyCache[collider] = enemy;
            return enemy;
        }

        return null;
    }

    private void PrintDebugState()
    {
        Debug.Log($"[TargetingAI] ActiveFollowers: {_activeFollowers.Count}, ActiveEnemies: {_activeEnemies.Count}, DetectedEnemies: {_detectedEnemies.Count}");
    }

    private void OnTriggerStay2D(Collider2D other)
    {


        if (_activeFollowers.Count < 1 || _activeEnemies.Count < 1) return;

        Enemy enemy = GetEnemyFromCollider(other);

        if (_activeEnemies.Count > 0 && enemy != null && !enemy.isFainted)
            EnterEnemy(enemy);

        if (_detectedEnemies.Count == 0) return;
            CalculateTargets();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (_activeFollowers.Count < 1 || _activeEnemies.Count < 1) return;

        Enemy enemy = GetEnemyFromCollider(other);


            Debug.Log("EnterEnemy 호출");
        if (_activeEnemies.Count > 0 && enemy != null && !enemy.isFainted)
            ExitEnemy(enemy);
    }





    public void CalculateTargets()
    {
        _targetCandidates.Clear();

        foreach (Enemy enemy in _detectedEnemies)
        {
            if (enemy == null) continue;
            foreach (Follower follower in _activeFollowers)
            {
                if (follower == null) continue;
                float dist = Vector2.Distance(follower.transform.position, enemy.transform.position);
                if (!_targetHashSet.Contains(enemy) && !enemy.isFainted)
                {
                    _targetCandidates.Add((dist, enemy));
                }
            }
        }
        AssignTargets();
    }

    private void AssignTargets()
    {
        foreach (Follower follower in _activeFollowers)
        {

            if (follower == null || follower.IsDashCheck())
                continue;

            foreach (var candidate in _targetCandidates)
            {
                if (!_targetHashSet.Contains(candidate.Item2))
                {
                    follower.SetTarget(candidate.Item2);
                    _targetHashSet.Add(candidate.Item2);
                    follower.CallDashAttack();
                    break;
                }
            }
        }
    }

    private class TargetComparer : IComparer<(float, Enemy)>
    {
        public int Compare((float, Enemy) x, (float, Enemy) y)
        {

            int result = x.Item1.CompareTo(y.Item1);
            if (result == 0)
            {


                return x.Item2.GetInstanceID().CompareTo(y.Item2.GetInstanceID());
            }
            return result;
        }
    }
}
