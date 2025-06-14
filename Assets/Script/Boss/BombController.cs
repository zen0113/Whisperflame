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
        // 폭발 효과음 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.bombExplode);
        }

        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = baseAngle + i * angleStep;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<BulletController>().Initialize(direction);
        }

        Destroy(gameObject); // 폭탄 게임오브젝트 제거
    }
}
