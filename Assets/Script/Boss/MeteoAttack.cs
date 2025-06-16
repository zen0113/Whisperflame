using UnityEngine;

public class MeteoAttack : MonoBehaviour
{
    public GameObject meteorPrefab;                 // ������ ȭ�� ������
    public float spawnInterval = 1.5f;              // ȭ�� ���� ����
    public float meteorFallY = -5f;                 // ȭ���� ������ Y ��ġ (�������� ����)
    public Vector2 spawnXRange = new Vector2(-3f, 3f);  // ȭ���� ������ X ��ǥ ����

    void Start()
    {
        // ���� �������� SpawnMeteor �Լ� ȣ��
        InvokeRepeating("SpawnMeteor", 1f, spawnInterval);
    }


    // ��� ������ ��ġ�� �����ϴ� �Լ�
    void SpawnMeteor()
    {
        // ���� X ��ǥ ����
        float x = Random.Range(spawnXRange.x, spawnXRange.y);

        // ���� ��ġ ��� (���� ������Ʈ Y ��ġ ����)
        Vector3 spawnPos = new Vector3(x, transform.position.y, 0);

        // ȭ�� ������ ����
        Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
    }

    // ������Ʈ�� ��Ȱ��ȭ�� �� � ���� ����
    void OnDisable()
    {
        CancelInvoke(); // �ݺ� ȣ�� ����
    }
}
