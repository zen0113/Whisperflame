using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 1;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Rigidbody2D 설정
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        // 속도 설정
        rb.linearVelocity = transform.up * speed;
        
        // 일정 시간 후 제거
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 몬스터와 충돌 시
        if (other.CompareTag("Enemy"))
        {
            // MonsterController 컴포넌트 가져오기
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                // 데미지 적용
                monster.TakeDamage(damage);
            }
        }
        
        // 충돌한 화살 제거
        Destroy(gameObject);
    }
} 