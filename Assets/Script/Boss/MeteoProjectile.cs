using UnityEngine;

public class MeteoProjectile : MonoBehaviour
{
    public float fallSpeed = 5f;  // 운석 낙하 속도

    void Update()
    {
        // 매 프레임마다 아래로 이동
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    // 충돌 처리
    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 충돌 시
        if (other.CompareTag("Player"))
        {
            // PlayerHealth 컴포넌트 가져와서 데미지 처리
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Damaged();
            }
        }

        // 화살 제거
        Destroy(gameObject);
    }
}
