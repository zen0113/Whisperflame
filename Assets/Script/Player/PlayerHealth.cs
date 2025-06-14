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
    public GameObject effectObject;

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

        // 데미지 효과음 재생 및 배경음악 볼륨 일시적 감소
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHurt);
            //AudioManager.Instance.TemporarilyLowerBGMVolume(1f, 0.2f);  // 1초 동안 볼륨 20%로 감소
        }

        isInvincible = true;
        gameObject.layer = LayerMask.NameToLayer("Invincible");

        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(0.15f, 0.2f));
        }

        // 깜빡임 시작
        StartCoroutine(FlashWhileInvincible());

        // 일정 시간 후 무적 해제
        Invoke("OffDamaged", invincibleDuration);
    }

    void OffDamaged()
    {
        isInvincible = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private IEnumerator FlashWhileInvincible()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        float flashInterval = 0.1f;

        while (elapsed < invincibleDuration)
        {
            if (sr != null)
                sr.enabled = !sr.enabled; // 깜빡이기 ON/OFF

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (sr != null)
            sr.enabled = true; // 보이도록 복원
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        effectObject.gameObject.SetActive(false);

        // 사망 효과음 재생 및 배경음악 끄기
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeath);
            //AudioManager.Instance.StopBGM(true);  // 페이드 아웃 효과와 함께 배경음악 끄기
        }

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

        // 6. SceneLoader를 통해 씬 전환
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneByName("TitleScene");
        }
        else
        {
            Debug.LogError("SceneLoader not found!");
            SceneManager.LoadScene("TitleScene");
        }
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
