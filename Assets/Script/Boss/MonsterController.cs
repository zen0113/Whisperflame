using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    public Image healthBarFill;  // HP 바 UI
    public int currentHealth;
    private bool isDamaged = false;

    [Header("스프라이트 관련")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    [SerializeField] private Sprite damagedSprite;

    void Start()
    {
        currentHealth = maxHealth;
        defaultSprite = spriteRenderer.sprite;

        // HP 바 초기화
        UpdateHealthBar();
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (isDamaged) return; // 이미 데미지 상태면 무시

        // 데미지 효과음 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHit);
        }

        StartCoroutine(ShowDamageSprite());

        currentHealth -= damage;
        UpdateHealthBar();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator ShowDamageSprite()
    {
        isDamaged = true;
        spriteRenderer.sprite = damagedSprite;

        Vector3 originalPos = transform.localPosition;
        float shakeDuration = 0.4f;
        float shakeMagnitude = 0.05f;
        float elapsed = 0f;

        // 진동 + 대기
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-0.2f, 0.2f) * shakeMagnitude;
            float offsetY = Random.Range(-0.2f, 0.2f) * shakeMagnitude;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원래 위치 복구
        transform.localPosition = originalPos;
        spriteRenderer.sprite = defaultSprite;
        isDamaged = false;
    }


    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        // 여기에 사망 효과나 아이템 드롭 로직 추가
        // Destroy(gameObject);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
} 