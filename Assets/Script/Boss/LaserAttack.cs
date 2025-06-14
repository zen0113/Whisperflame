using UnityEngine;

public class LaserAttack : MonoBehaviour
{
    public GameObject laserWarningPrefab;
    public GameObject laserBeamPrefab;
    public Transform[] lanes; // Lane positions for laser
    public float warningTime = 1f;
    public float laserDuration = 1f;
    public float interval = 4f;

    void Start()
    {
        InvokeRepeating("FireLaser", 2f, interval);
    }

    void FireLaser()
    {
        Transform lane = lanes[Random.Range(0, lanes.Length)];
        StartCoroutine(SpawnLaser(lane));
    }

    System.Collections.IEnumerator SpawnLaser(Transform lane)
    {
        GameObject warn = Instantiate(laserWarningPrefab, lane.position, Quaternion.identity);
        yield return new WaitForSeconds(warningTime);
        Destroy(warn);

        // 레이저 공격 효과음 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.laserAttack);
        }

        GameObject beam = Instantiate(laserBeamPrefab, lane.position, Quaternion.identity);
        beam.AddComponent<LaserBeamCollision>();
        yield return new WaitForSeconds(laserDuration);
        Destroy(beam);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}

// 레이저 빔 충돌 처리를 위한 새로운 클래스
public class LaserBeamCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Damaged();
            }
        }
    }
}

