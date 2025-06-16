using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public GameObject bombPrefab;         // 폭탄(유령) 프리팹
    public float spawnInterval = 3f;      // 폭탄(유령) 생성 주기
    public Vector2 spawnAreaMin;          // 생성 영역 최소 좌표
    public Vector2 spawnAreaMax;          // 생성 영역 최대 좌표

    private float timer = 0f;

    void Update()
    {
        // 시간 누적
        timer += Time.deltaTime;

        // 주기가 되면 폭탄(유령) 생성
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBomb();
        }
    }

    // 폭탄(유령) 생성 위치를 랜덤으로 지정하여 생성
    void SpawnBomb()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        Instantiate(bombPrefab, spawnPos, Quaternion.identity);
    }

    // 오브젝트 비활성화 시, Invoke 중지
    void OnDisable()
    {
        CancelInvoke();
    }
}
