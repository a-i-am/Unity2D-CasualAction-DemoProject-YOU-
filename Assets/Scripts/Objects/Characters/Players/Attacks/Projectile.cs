using Assets;
using UnityEngine;

public enum ProjectileType
{
    Standard,
    Lasting,
    Poisonous

}
public class Projectile : MonoBehaviour
{
    private GameObject collidedObject;
    [SerializeField] private float launchSpeed;
    [SerializeField] private ProjectileType projectileType;
    private Vector2 launchDir;
    private SpriteRenderer spriteRenderer;
    private Player player;
    private int followerLayer = 9;
    public void SetDirection(Vector2 launchDir)
    {
        this.launchDir = launchDir.normalized;
    }

    void Start()
    {
        player = FindObjectOfType<Player>();

        Transform childTransform = GetComponentInChildren<Transform>();


        Rigidbody2D rbAmmo = GetComponent<Rigidbody2D>();
        rbAmmo.velocity = launchDir * launchSpeed;

        if (player.GetComponent<SpriteRenderer>().flipX)
        {
            childTransform.localScale = new Vector3(-Mathf.Abs(childTransform.localScale.x), childTransform.localScale.y, childTransform.localScale.z);
        }
        else childTransform.localScale = new Vector3(Mathf.Abs(childTransform.localScale.x), childTransform.localScale.y, childTransform.localScale.z);
    }






    void OnCollisionEnter2D(Collision2D other)
    {
        collidedObject = other.gameObject;

        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collidedObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage();
            }
        }

        switch (projectileType)
        {
            case ProjectileType.Standard:
                Destroy(gameObject);

                break;

            case ProjectileType.Lasting:

                Destroy(gameObject);
                break;

            case ProjectileType.Poisonous:


                Destroy(gameObject);
                break;

            default: Destroy(gameObject);
                break;
        }
    }

    private void Explode()
    {

        Debug.Log("Explosion effect!");
        Destroy(gameObject);
    }

    private void ApplyPoison()
    {

        Debug.Log("Poison effect applied!");
        Destroy(gameObject);
    }
}
