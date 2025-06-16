using UnityEngine;

public class MeteoProjectile : MonoBehaviour
{
    public float fallSpeed = 5f;  // � ���� �ӵ�

    void Update()
    {
        // �� �����Ӹ��� �Ʒ��� �̵�
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    // �浹 ó��
    void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾�� �浹 ��
        if (other.CompareTag("Player"))
        {
            // PlayerHealth ������Ʈ �����ͼ� ������ ó��
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Damaged();
            }
        }

        // ȭ�� ����
        Destroy(gameObject);
    }
}
