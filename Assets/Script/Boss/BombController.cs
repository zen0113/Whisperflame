using UnityEngine;

public class BombController : MonoBehaviour
{
    public GameObject projectilePrefab;     // 폭발 시 발사될 투사체 프리팹
    public int projectileCount = 12;        // 발사할 투사체 개수
    public float explosionDelay = 2f;       // 폭발까지의 대기 시간
    private float baseAngle = 0f;           // 시작 각도 (회전 기초)

    void Start()
    {
        // 일정 시간 후 폭발
        Invoke(nameof(Explode), explosionDelay);
    }

    // 폭탄(유령) 폭발 로직
    void Explode()
    {
        // 폭발 효과음 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.bombExplode);
        }

        float angleStep = 360f / projectileCount;

        // 원형으로 투사체 생성
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = baseAngle + i * angleStep;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<BulletController>().Initialize(direction);
        }

        // 폭탄(유령) 오브젝트 제거
        Destroy(gameObject);
    }
}
