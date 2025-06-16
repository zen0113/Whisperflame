using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // 불꽃의 이동 속도
    [SerializeField] private float speed = 10f;

    // 불꽃의 존재하는 시간 (초)
    [SerializeField] private float lifeTime = 3f;

    // 불꽃이 적에게 입히는 데미지
    [SerializeField] private int damage = 1;

    // Rigidbody2D 컴포넌트 참조
    private Rigidbody2D rb;

    void Awake()
    {
        // Rigidbody2D 컴포넌트 가져오기 또는 없으면 추가
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // 중력 영향을 받지 않도록 설정
        rb.gravityScale = 0f;

        // 회전 방지
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        // 불꽃을 위 방향으로 설정된 속도로 발사
        rb.linearVelocity = transform.up * speed;

        // 공격 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerAttack);
        }

        // 일정 시간이 지나면 화살 오브젝트 제거
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 적과 충돌했을 때
        if (other.CompareTag("Enemy"))
        {
            // 몬스터 컨트롤러 컴포넌트를 가져옴
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                // 몬스터에게 데미지를 줌
                monster.TakeDamage(damage);
            }
        }

        // 적과 상관없이 충돌 시 불꽃은 제거됨
        Destroy(gameObject);
    }
}
