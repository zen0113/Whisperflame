using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 5f;              // 이동 속도
    public float rotateSpeed = 30f;       // 회전 속도 (도/초)
    private Vector3 currentDirection;     // 현재 이동 방향
    public float lifetime = 5f;           // 수명 (초)

    void Start()
    {
        // 일정 시간이 지나면 자동으로 제거
        Destroy(gameObject, lifetime);
    }

    // 외부에서 방향 초기화
    public void Initialize(Vector3 direction)
    {
        currentDirection = direction.normalized;
    }

    void Update()
    {
        // 일정 속도로 방향 회전
        currentDirection = Quaternion.Euler(0, 0, rotateSpeed * Time.deltaTime) * currentDirection;

        // 이동
        transform.position += currentDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 충돌 시 데미지 적용
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Damaged();
            }
        }

        // 충돌 후 투사체 제거
        Destroy(gameObject);
    }
}
