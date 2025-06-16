using UnityEngine;

public class MeteoAttack : MonoBehaviour
{
    public GameObject meteorPrefab;                 // 생성할 화살 프리팹
    public float spawnInterval = 1.5f;              // 화살 생성 간격
    public float meteorFallY = -5f;                 // 화살이 떨어질 Y 위치 (사용되지는 않음)
    public Vector2 spawnXRange = new Vector2(-3f, 3f);  // 화살이 생성될 X 좌표 범위

    void Start()
    {
        // 일정 간격으로 SpawnMeteor 함수 호출
        InvokeRepeating("SpawnMeteor", 1f, spawnInterval);
    }


    // 운석을 랜덤한 위치에 생성하는 함수
    void SpawnMeteor()
    {
        // 랜덤 X 좌표 생성
        float x = Random.Range(spawnXRange.x, spawnXRange.y);

        // 생성 위치 계산 (현재 오브젝트 Y 위치 기준)
        Vector3 spawnPos = new Vector3(x, transform.position.y, 0);

        // 화살 프리팹 생성
        Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
    }

    // 오브젝트가 비활성화될 때 운석 생성 중지
    void OnDisable()
    {
        CancelInvoke(); // 반복 호출 중지
    }
}
