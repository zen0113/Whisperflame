using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    public Image healthBarFill;  // HP 바 UI
    public int currentHealth;


    void Start()
    {
        currentHealth = maxHealth;
        
        // HP 바 초기화
        UpdateHealthBar();
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
        
        if (currentHealth <= 0)
        {
            Die();
        }
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