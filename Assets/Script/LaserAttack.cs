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

        GameObject beam = Instantiate(laserBeamPrefab, lane.position, Quaternion.identity);
        yield return new WaitForSeconds(laserDuration);
        Destroy(beam);
    }
}
