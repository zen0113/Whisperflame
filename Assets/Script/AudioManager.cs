using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }  // 싱글톤 인스턴스

    [Header("배경음악")]
    public AudioClip titleBGM;    // 타이틀 씬 BGM
    public AudioClip gameBGM;     // 게임 씬 BGM

    [Header("효과음")]
    public AudioClip uiClick;
    public AudioClip playerDeath;
    public AudioClip playerHurt;
    public AudioClip playerAttack;
    public AudioClip enemyHit;
    public AudioClip stageClear;
    public AudioClip laserAttack;
    public AudioClip bombExplode;

    private AudioSource bgmSource;  // 배경음악용 AudioSource
    private AudioSource sfxSource;  // 효과음용 AudioSource

    private void Awake()
    {
        // 싱글톤 설정 및 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
            InitializeAudioSources();      // 오디오 소스 초기화
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 시 콜백 등록
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // 씬 로드 시 자동으로 맞는 배경음악 재생
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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

    // 오디오 소스 초기화
    private void InitializeAudioSources()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = 0.3f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = 0.7f;
    }

    // 배경음악 재생
    public void PlayBGM(AudioClip bgm)
    {
        if (bgmSource.clip == bgm) return; // 같은 BGM이면 재생하지 않음

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

    // 배경음악 정지
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
