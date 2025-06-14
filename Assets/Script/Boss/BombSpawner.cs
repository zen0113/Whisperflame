using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public GameObject bombPrefab;
    public float spawnInterval = 3f;
    public Vector2 spawnAreaMin;  // ��: (-8, -4)
    public Vector2 spawnAreaMax;  // ��: (8, 4)

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBomb();
        }
    }

    void SpawnBomb()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        Instantiate(bombPrefab, spawnPos, Quaternion.identity);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
