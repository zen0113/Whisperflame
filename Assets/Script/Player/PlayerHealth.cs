using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // 최대 체력 (static으로 다른 스크립트에서도 접근 가능)
    public static int maxHealth = 3;
    public int currentHealth;

    // UI에 표시될 하트 이미지 및 스프라이트
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // 이펙트 오브젝트 (피격 시 비활성화 등)
    public GameObject effectObject;

    // 애니메이터 컴포넌트
    private Animator animator;

    // 무적 관련 설정
    public float invincibleDuration = 3f;
    private bool isInvincible = false;

    // 카메라 흔들림 연출
    private CameraShake cameraShake;
    public Camera mainCamera;

    // 줌 인 설정
    public float zoomInSize = 2.5f;
    public float zoomDuration = 1.0f;

    // 사망 처리 플래그
    private bool isDead = false;

    // 플레이어 조작 관련 참조
    private PlayerController movement;
    private PlayerAttack attack;

    // 웨이브 관리자 오브젝트
    public GameObject waveManager;

    // 검은색 페이드 패널 (사망 시 화면 전환)
    public Image fadePanel;
    public float fadeDuration = 2.0f;

    // 웨이브 스포너들
    public GameObject wave1Spawners;
    public GameObject wave2Spawners;
    public GameObject wave3Spawners;

    // 초기화
    void Start()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerController>();
        attack = GetComponent<PlayerAttack>();

        currentHealth = maxHealth;

        // 시작 시 페이드 패널의 알파값을 0으로 설정
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0;
            fadePanel.color = c;
        }
    }

    // 매 프레임 호출됨
    void Update()
    {
        UpdateHearts(); // 하트 UI 업데이트

        // 체력이 0 이하이고 아직 죽지 않았다면 사망 처리
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // 하트 UI를 현재 체력에 맞게 갱신
    void UpdateHearts()
    {
        if (hearts == null) return;

        // 모든 하트를 빈 하트로 설정
        foreach (Image img in hearts)
        {
            if (img != null)
            {
                img.sprite = emptyHeart;
            }
        }

        // 현재 체력만큼 하트를 꽉 찬 하트로 표시
        for (int i = 0; i < currentHealth; i++)
        {
            if (i < hearts.Length && hearts[i] != null)
            {
                hearts[i].sprite = fullHeart;
            }
        }
    }

    // 플레이어가 데미지를 입었을 때 호출
    public void Damaged()
    {
        if (isInvincible) return; // 무적 상태이면 무시

        currentHealth--;

        // 애니메이션 재생
        if (animator != null)
        {
            animator.SetTrigger("Damaged");
        }

        // 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHurt);
            // AudioManager.Instance.TemporarilyLowerBGMVolume(1f, 0.2f); // 배경음 일시 감소
        }

        // 무적 상태 설정
        isInvincible = true;
        gameObject.layer = LayerMask.NameToLayer("Invincible");

        // 카메라 흔들림 연출
        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(0.15f, 0.2f));
        }

        // 무적 깜빡임 연출
        StartCoroutine(FlashWhileInvincible());

        // 일정 시간 후 무적 해제
        Invoke("OffDamaged", invincibleDuration);
    }

    // 무적 해제
    void OffDamaged()
    {
        isInvincible = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    // 무적 시간 동안 스프라이트 깜빡이기
    private IEnumerator FlashWhileInvincible()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        float flashInterval = 0.1f;

        while (elapsed < invincibleDuration)
        {
            if (sr != null)
                sr.enabled = !sr.enabled;

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (sr != null)
            sr.enabled = true;
    }

    // 사망 처리
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        effectObject.gameObject.SetActive(false); // 이펙트 끄기

        // 사망 사운드 재생 및 배경음 제거
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeath);
            // AudioManager.Instance.StopBGM(true); 
        }

        // 각종 웨이브 이벤트 중지
        waveManager.GetComponent<BattleWaveManager>().enabled = false;
        wave1Spawners.GetComponent<MeteoAttack>().enabled = false;
        wave2Spawners.GetComponent<LaserAttack>().enabled = false;
        wave3Spawners.GetComponent<BombSpawner>().enabled = false;

        StartCoroutine(DeathSequence()); // 사망 시퀀스 시작
    }

    // 사망 연출 시퀀스
    IEnumerator DeathSequence()
    {
        // 조작 및 공격 비활성화
        if (movement != null) movement.enabled = false;
        if (attack != null) attack.enabled = false;

        // 죽는 애니메이션
        if (animator != null)
        {
            animator.SetBool("Dead", true);
        }

        yield return new WaitForSeconds(1.0f); // 잠시 대기

        // 페이드 아웃
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeToBlack());
        }

        // 타이틀 씬으로 이동
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

    // 화면 검게 페이드 아웃
    IEnumerator FadeToBlack()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color startColor = fadePanel.color;
        Color targetColor = new Color(0, 0, 0, 1); // 완전한 검정

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
