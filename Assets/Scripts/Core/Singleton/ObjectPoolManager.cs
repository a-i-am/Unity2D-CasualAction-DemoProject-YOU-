using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TextCore.Text;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float despawnTime = 5f;
    private GameObject pooledProjectileParent;

    private List<GameObject> pooledProjectiles;


    private bool isInitialized = false;


    void Start()
    {

        if (!isInitialized)
        {
            InitializePool();
        }
    }


    private void InitializePool()
    {

        pooledProjectiles = new List<GameObject>();
        GameObject pooledProjectileParent = new GameObject("Pooled_Projectile");



        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject pooledProjectile = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
            pooledProjectile.SetActive(false);
            pooledProjectile.transform.parent = pooledProjectileParent.transform;
            pooledProjectiles.Add(pooledProjectile);

            Destroy(pooledProjectileParent, 3.0f);
        }
        #region 몹 풀링1














        #endregion
        isInitialized = true;
    }
    #region 몹 풀링2





























    #endregion
    public GameObject GetProjectile()
    {
        foreach (GameObject pooledProjectile in pooledProjectiles)
        {
            if (!pooledProjectile.activeInHierarchy)
            {
                Debug.Log("비활성화 발사체 가져옴");
                pooledProjectile.SetActive(true);
                Invoke("DespawnProjectile", despawnTime);
                return pooledProjectile;
            }
        }

        Debug.LogWarning("풀에 비활성화된 발사체가 없습니다. 초기 풀 크기를 늘려주세요.");
        return null;
    }


    public void ReturnProjectile(GameObject projectile)
    {
        if (pooledProjectiles.Contains(projectile))
        {
            projectile.SetActive(false);
        }
        else
        {
            Debug.LogWarning("이 풀에는 해당 발사체가 없습니다.");
        }
    }


    private IEnumerator DespawnProjectile(GameObject projectile)
    {
        yield return new WaitForSeconds(despawnTime);
        ReturnProjectile(projectile);

        #region 이미 ReturnProjectile에서 처리중임













        #endregion
    }
}
