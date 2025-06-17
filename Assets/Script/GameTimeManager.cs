using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

// 게임 시간을 측정하고 클리어 후 자동으로 타이틀로 이동하는 매니저
public class GameTimeManager : MonoBehaviour
{
    [Header("페이드 설정")]
    public Image fadePanel;         // 페이드 아웃용 이미지 패널
    public float fadeDuration = 1f; // 페이드 지속 시간

    [Header("타이틀 이동 설정")]
    public string titleSceneName = "TitleScene"; // 타이틀 씬 이름
    public float waitTimeAfterClear = 5f;        // 게임 클리어 후 대기 시간

    // 시간 측정 관련 변수들
    private float gameStartTime;    // 게임 시작 시간
    private float gameEndTime;      // 게임 종료 시간
    private float totalGameTime;    // 총 게임 플레이 시간
    private bool gameEnded = false; // 게임 종료 여부

    // 싱글톤 패턴 구현
    public static GameTimeManager Instance { get; private set; }

    // 현재 게임 시간을 가져오는 프로퍼티
    public float CurrentGameTime
    {
        get
        {
            if (gameEnded)
                return totalGameTime;
            else
                return Time.time - gameStartTime;
        }
    }

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 게임 시작 시간 기록
        StartGameTimer();

        // 페이드 패널 초기 설정 (완전 투명)
        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 0);
            fadePanel.gameObject.SetActive(true);
        }
    }

    // 게임 타이머 시작
    public void StartGameTimer()
    {
        gameStartTime = Time.time;
        gameEnded = false;
        Debug.Log("게임 타이머 시작!");
    }

    // 게임 종료 및 자동 타이틀 이동 처리
    public void EndGame()
    {
        if (gameEnded) return; // 이미 종료된 경우 중복 실행 방지

        gameEndTime = Time.time;
        totalGameTime = gameEndTime - gameStartTime;
        gameEnded = true;

        Debug.Log($"게임 클리어! 총 플레이 시간: {FormatTime(totalGameTime)}");

        // 점수 저장
        SaveScore();

        // 지정된 시간 후 자동으로 타이틀로 이동
        StartCoroutine(AutoReturnToTitle());
    }

    // 점수를 저장하는 메서드
    void SaveScore()
    {
        // ScoreManager에 점수 저장
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(totalGameTime);
        }
        else
        {
            // ScoreManager가 없는 경우 PlayerPrefs로 직접 저장
            Debug.LogWarning("ScoreManager not found, saving directly to PlayerPrefs");
            SaveScoreToPlayerPrefs();
        }
    }

    // PlayerPrefs에 직접 점수 저장 (백업용)
    void SaveScoreToPlayerPrefs()
    {
        // 기존 점수들 불러오기
        int scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);

        // 새 점수 추가
        PlayerPrefs.SetFloat($"Score_{scoreCount}", totalGameTime);
        PlayerPrefs.SetInt("ScoreCount", scoreCount + 1);
        PlayerPrefs.Save();
    }

    // 자동으로 타이틀로 돌아가는 코루틴
    IEnumerator AutoReturnToTitle()
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(waitTimeAfterClear);

        // 페이드 아웃 시작
        yield return StartCoroutine(FadeToBlack());

        // 타이틀 씬으로 이동
        SceneManager.LoadScene(titleSceneName);
    }

    // 페이드 아웃 효과 코루틴
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

    // 시간을 MM:SS 형식으로 포맷하는 메서드
    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // 현재 게임 시간을 포맷된 문자열로 반환
    public string GetFormattedCurrentTime()
    {
        return FormatTime(CurrentGameTime);
    }

    void OnDestroy()
    {
        // 인스턴스가 파괴될 때 null로 설정
        if (Instance == this)
        {
            Instance = null;
        }
    }
}