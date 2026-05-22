using Assets;
using UnityEngine;

public enum ProjectileType
{
    Standard,
    Lasting,
    Poisonous
    // 다른 유형 추가 가능
}
public class Projectile : MonoBehaviour
{
    private GameObject collidedObject;
    [SerializeField] private float launchSpeed;
    [SerializeField] private ProjectileType projectileType; // 프리팹 유형 추가
    private Vector2 launchDir;
    private SpriteRenderer spriteRenderer;
    private Player player;
    private int followerLayer = 9;    // "Follower" 레이어의 번호
    public void SetDirection(Vector2 launchDir)
    {
        this.launchDir = launchDir.normalized; // 방향 벡터 정규화
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        // 자식 오브젝트의 Transform 가져오기
        Transform childTransform = GetComponentInChildren<Transform>();

        // 발사체에 속도 적용
        Rigidbody2D rbAmmo = GetComponent<Rigidbody2D>(); // 강체 탄(발사체)
        rbAmmo.velocity = launchDir * launchSpeed;

        if (player.GetComponent<SpriteRenderer>().flipX)
        {
            childTransform.localScale = new Vector3(-Mathf.Abs(childTransform.localScale.x), childTransform.localScale.y, childTransform.localScale.z); // X축 반전
        }
        else childTransform.localScale = new Vector3(Mathf.Abs(childTransform.localScale.x), childTransform.localScale.y, childTransform.localScale.z); // 원래 방향 유지
    }

    //private void Normal()
    //{
    //    rbAmmo.velocity = Vector2.right * speed;
    //}

    void OnCollisionEnter2D(Collision2D other)
    {
        collidedObject = other.gameObject;
        // 적과 충돌했을 때
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collidedObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage();
            }
        }
                // 프리팹의 종류에 따라 다르게 처리
        switch (projectileType)
        {
            case ProjectileType.Standard:
                Destroy(gameObject);
                // 표준 발사체에 대한 처리
                break;

            case ProjectileType.Lasting:
                // 폭발성 발사체에 대한 처리
                Destroy(gameObject);
                break;

            case ProjectileType.Poisonous:
                // 독성 발사체에 대한 처리
                //ApplyPoison(); // 독성 적용 메서드 호출
                Destroy(gameObject);
                break;

            default: Destroy(gameObject);
                break;
        }
    }

    private void Explode()
    {
        // 폭발 효과 구현
        Debug.Log("Explosion effect!");
        Destroy(gameObject); // 파괴
    }

    private void ApplyPoison()
    {
        // 독성 효과 구현
        Debug.Log("Poison effect applied!");
        Destroy(gameObject); // 파괴
    }
}
