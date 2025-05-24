using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 30f; // �Ѿ��� �־����� ���� (��/��)
    private Vector3 currentDirection;
    public float lifetime = 5f; // �ڵ� �ı� Ÿ�̸�

    void Start()
    {
        // ���� �ð��� ������ �ڵ� �ı�
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 direction)
    {
        currentDirection = direction.normalized;
    }

    void Update()
    {
        // ������ ȸ����Ŵ
        currentDirection = Quaternion.Euler(0, 0, rotateSpeed * Time.deltaTime) * currentDirection;

        // �̵�
        transform.position += currentDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ������ ó��
            Destroy(gameObject);
        }
    }
}
