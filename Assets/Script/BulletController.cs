using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 30f; // 총알이 휘어지는 정도 (도/초)
    private Vector3 currentDirection;
    public float lifetime = 5f; // 자동 파괴 타이머

    void Start()
    {
        // 일정 시간이 지나면 자동 파괴
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 direction)
    {
        currentDirection = direction.normalized;
    }

    void Update()
    {
        // 방향을 회전시킴
        currentDirection = Quaternion.Euler(0, 0, rotateSpeed * Time.deltaTime) * currentDirection;

        // 이동
        transform.position += currentDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 데미지 처리
            Destroy(gameObject);
        }
    }
}
