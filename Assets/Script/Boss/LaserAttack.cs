using UnityEngine;

public class LaserAttack : MonoBehaviour
{
    public GameObject laserWarningPrefab;  // 레이저 공격 전 경고 이펙트 프리팹
    public GameObject laserBeamPrefab;     // 실제 레이저 빔 프리팹
    public Transform[] lanes;              // 레이저가 나갈 수 있는 라인들 (위치들)
    public float warningTime = 1f;         // 경고 이펙트 유지 시간
    public float laserDuration = 1f;       // 레이저 지속 시간
    public float interval = 4f;            // 레이저 공격 간격

    void Start()
    {
        // 일정 주기마다 레이저 발사
        InvokeRepeating("FireLaser", 2f, interval);
    }

    // 랜덤한 레인에서 레이저 공격 실행
    void FireLaser()
    {
        // 무작위 레인 선택
        Transform lane = lanes[Random.Range(0, lanes.Length)];
        StartCoroutine(SpawnLaser(lane));
    }

    // 경고 → 레이저 → 제거 순서의 코루틴
    System.Collections.IEnumerator SpawnLaser(Transform lane)
    {
        // 경고 이펙트 생성
        GameObject warn = Instantiate(laserWarningPrefab, lane.position, Quaternion.identity);

        // 경고 시간 동안 대기
        yield return new WaitForSeconds(warningTime);

        // 경고 제거
        Destroy(warn);

        // 레이저 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.laserAttack);
        }

        // 레이저 빔 생성
        GameObject beam = Instantiate(laserBeamPrefab, lane.position, Quaternion.identity);

        // 충돌 처리를 위한 컴포넌트 추가
        beam.AddComponent<LaserBeamCollision>();

        // 레이저 지속 시간만큼 대기
        yield return new WaitForSeconds(laserDuration);

        // 레이저 제거
        Destroy(beam);
    }

    // 오브젝트가 비활성화되면 레이저 반복 호출 중지
    void OnDisable()
    {
        CancelInvoke();
    }
}


// 레이저 빔 충돌 처리를 위한 클래스
public class LaserBeamCollision : MonoBehaviour
{
    // 플레이어와 충돌 시 데미지 적용
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Damaged(); // 플레이어에게 데미지
            }
        }
    }
}

