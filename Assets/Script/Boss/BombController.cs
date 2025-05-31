using UnityEngine;

public class BombController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int projectileCount = 12;
    public float explosionDelay = 2f;
    private float baseAngle = 0f;

    void Start()
    {
        Invoke(nameof(Explode), explosionDelay);
    }

    void Explode()
    {
        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = baseAngle + i * angleStep;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<BulletController>().Initialize(direction);
        }

        Destroy(gameObject); // ÆøÅº ¿ÀºêÁ§Æ® Á¦°Å
    }
}
