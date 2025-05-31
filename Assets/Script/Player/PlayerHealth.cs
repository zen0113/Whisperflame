using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public static int maxHealth = 3;
    public int currentHealth;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private Animator animator;

    public float invincibleDuration = 3f;
    private bool isInvincible = false;

    private CameraShake cameraShake; // 카메라 쉐이크용
    public Camera mainCamera;
    public float zoomInSize = 2.5f;
    public float zoomDuration = 1.0f;

    private bool isDead = false;
    private PlayerController movement;
    private PlayerAttack attack;
    public GameObject waveManager;

    public Image fadePanel;  // 검은색 UI 패널 (Canvas에 이미지로 오버레이)
    public float fadeDuration = 2.0f;

    public GameObject wave1Spawners;
    public GameObject wave2Spawners;
    public GameObject wave3Spawners;

    // Start is called before the first frame update
    void Start()
    {
        // 컴포넌트 초기화
        cameraShake = Camera.main.GetComponent<CameraShake>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerController>();
        attack = GetComponent<PlayerAttack>();

        // 체력 초기화
        currentHealth = maxHealth;

        // 페이드 패널 초기화
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0;
            fadePanel.color = c;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHearts();

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void UpdateHearts()
    {
        if (hearts == null) return;

        foreach (Image img in hearts)
        {
            if (img != null)
            {
                img.sprite = emptyHeart;
            }
        }

        for (int i = 0; i < currentHealth; i++)
        {
            if (i < hearts.Length && hearts[i] != null)
            {
                hearts[i].sprite = fullHeart;
            }
        }
    }

    public void Damaged()
    {
        if (isInvincible) return;

        currentHealth--;
        if (animator != null)
        {
            animator.SetTrigger("Damaged");
        }

        isInvincible = true;
        gameObject.layer = LayerMask.NameToLayer("Invincible");

        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(0.15f, 0.2f));
        }

        Invoke("OffDamaged", invincibleDuration);
    }

    void OffDamaged()
    {
        isInvincible = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        waveManager.GetComponent<BattleWaveManager>().enabled = false;
        wave1Spawners.GetComponent<MeteoAttack>().enabled = false;
        wave2Spawners.GetComponent<LaserAttack>().enabled = false;
        wave3Spawners.GetComponent<BombSpawner>().enabled = false;
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // 1. 플레이어 움직임과 공격 비활성화
        if (movement != null)
        {
            movement.enabled = false;
        }
        if (attack != null)
        {
            attack.enabled = false;
        }

        // 2. 죽는 애니메이션
        if (animator != null)
        {
            animator.SetBool("Dead", true);
        }


        // 4. 잠시 대기
        yield return new WaitForSeconds(1.0f);

        // 5. 페이드 아웃
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeToBlack());
        }

        // 6. 씬 전환
        //SceneManager.LoadScene("TitleScene");
    }


    IEnumerator FadeToBlack()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color startColor = fadePanel.color;
        Color targetColor = new Color(0, 0, 0, 1);

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            fadePanel.color = Color.Lerp(startColor, targetColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadePanel.color = targetColor;
    }
}
