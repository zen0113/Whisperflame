using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;          // 최대 체력
    public Image healthBarFill;                           // 체력 바 UI 이미지
    public int currentHealth;                             // 현재 체력
    private bool isDamaged = false;                       // 데미지를 받고 있는 상태 여부

    [Header("스프라이트 관련")]
    [SerializeField] private SpriteRenderer spriteRenderer;  // 몬스터 스프라이트 렌더러
    public Sprite defaultSprite;                             // 기본 상태 스프라이트
    [SerializeField] private Sprite damagedSprite;           // 피격 상태 스프라이트

    void Start()
    {
        // 체력 초기화
        currentHealth = maxHealth;

        // 현재 스프라이트를 기본 스프라이트로 저장
        defaultSprite = spriteRenderer.sprite;

        // 체력 바 초기화
        UpdateHealthBar();
    }

    void Update()
    {
        // 필요 시 상태 확인용 업데이트 처리
    }


    // 데미지를 받을 때 호출되는 함수
    public void TakeDamage(int damage)
    {
        if (isDamaged) return; // 피격 중엔 무시 (중복 피격 방지)

        // 피격 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHit);
        }

        // 피격 스프라이트 및 흔들림 효과 코루틴 실행
        StartCoroutine(ShowDamageSprite());

        // 체력 감소
        currentHealth -= damage;

        // 체력 바 업데이트
        UpdateHealthBar();

        // 체력이 0 이하이면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }


    // 피격 시 스프라이트 변경 및 흔들림 효과 처리
    private IEnumerator ShowDamageSprite()
    {
        isDamaged = true;

        // 피격 스프라이트로 변경
        spriteRenderer.sprite = damagedSprite;

        // 흔들림 효과 변수
        Vector3 originalPos = transform.localPosition;
        float shakeDuration = 0.4f;
        float shakeMagnitude = 0.05f;
        float elapsed = 0f;

        // 일정 시간 동안 흔들림 효과 반복
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-0.2f, 0.2f) * shakeMagnitude;
            float offsetY = Random.Range(-0.2f, 0.2f) * shakeMagnitude;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원래 위치 및 기본 스프라이트로 복귀
        transform.localPosition = originalPos;
        spriteRenderer.sprite = defaultSprite;
        isDamaged = false;
    }

    // 체력 바 UI를 현재 체력 기준으로 업데이트
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    // 몬스터 죽음 처리
    private void Die()
    {

    }

    // 체력을 최대치로 복구하고 체력 바 갱신
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
}
