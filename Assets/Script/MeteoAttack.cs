using UnityEngine;

public class MeteoAttack : MonoBehaviour
{
    public GameObject meteorPrefab;
    public float spawnInterval = 1.5f;
    public float meteorFallY = -5f;
    public Vector2 spawnXRange = new Vector2(-3f, 3f);

    void Start()
    {
        InvokeRepeating("SpawnMeteor", 1f, spawnInterval);
    }

    void SpawnMeteor()
    {
        float x = Random.Range(spawnXRange.x, spawnXRange.y);
        Vector3 spawnPos = new Vector3(x, transform.position.y, 0);
        Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
    }
}
