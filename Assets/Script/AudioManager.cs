using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("배경음악")]
    public AudioClip titleBGM;    // 타이틀 배경음악
    public AudioClip gameBGM;     // 게임 씬 배경음악

    [Header("효과음")]
    public AudioClip uiClick;       // UI 클릭 효과음
    public AudioClip playerDeath;     // 플레이어 사망 효과음
    public AudioClip playerHurt;      // 플레이어 체력 감소 효과음
    public AudioClip playerAttack;    // 플레이어 공격 효과음
    public AudioClip enemyHit;        // 적 데미지 효과음
    public AudioClip stageClear;      // 다음 단계로 넘어갈 때 효과음
    public AudioClip laserAttack;     // 레이저 공격 효과음
    public AudioClip bombExplode;     // 폭탄 터지는 효과음

    private AudioSource bgmSource;    // 배경음악용 AudioSource
    private AudioSource sfxSource;    // 효과음용 AudioSource

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            SceneManager.sceneLoaded += OnSceneLoaded;  // 씬 로드 이벤트 구독
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;  // 씬 로드 이벤트 구독 해제
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때마다 해당 씬에 맞는 배경음악 재생
        switch (scene.name)
        {
            case "TitleScene":
                PlayBGM(titleBGM);
                break;
            case "PlayerScene":
                PlayBGM(gameBGM);
                break;
        }
    }

    private void InitializeAudioSources()
    {
        // 배경음악용 AudioSource 설정
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = 0.3f;

        // 효과음용 AudioSource 설정
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = 0.7f;
    }

    // 배경음악 재생
    public void PlayBGM(AudioClip bgm)
    {
        if (bgmSource.clip == bgm) return;

        bgmSource.clip = bgm;
        bgmSource.volume = 0.3f;
        bgmSource.Play();
    }

    // 효과음 재생
    public void PlaySFX(AudioClip sfx)
    {
        if (sfx != null)
        {
            sfxSource.PlayOneShot(sfx);
        }
    }

    // 배경음악 끄기
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}