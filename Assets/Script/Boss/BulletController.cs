using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 30f; // 오른쪽 회전 각도 (도/초)
    private Vector3 currentDirection;
    public float lifetime = 5f; // 최대 수명

    void Start()
    {
        // 최대 수명 동안 유지
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 direction)
    {
        currentDirection = direction.normalized;
    }

    void Update()
    {
        // 오른쪽으로 회전
        currentDirection = Quaternion.Euler(0, 0, rotateSpeed * Time.deltaTime) * currentDirection;

        // 이동
        transform.position += currentDirection * speed * Time.deltaTime;
    }

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
        Destroy(gameObject);
    }
}
