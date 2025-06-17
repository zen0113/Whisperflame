using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

// 점수 화면의 UI를 관리하는 클래스
public class ScoreSceneUI : MonoBehaviour
{
    [Header("UI 요소들")]
    public Transform scoreListParent;           // 점수 리스트가 들어갈 부모 Transform
    public GameObject scoreItemPrefab;          // 점수 항목 프리팹
    public Button backButton;                   // 뒤로가기 버튼
    public Button clearScoresButton;            // 점수 초기화 버튼
    public TextMeshProUGUI titleText;           // 제목 텍스트
    public TextMeshProUGUI noScoreText;         // 기록이 없을 때 표시할 텍스트

    [Header("씬 설정")]
    public string titleSceneName = "TitleScene"; // 타이틀 씬 이름

    [Header("애니메이션 설정")]
    public float itemAnimationDelay = 0.1f;     // 각 항목 간 애니메이션 딜레이

    // 생성된 점수 항목들을 저장하는 리스트
    private List<GameObject> scoreItems = new List<GameObject>();

    void Start()
    {
        // 버튼 이벤트 연결
        if (backButton != null)
            backButton.onClick.AddListener(BackToTitle);

        if (clearScoresButton != null)
            clearScoresButton.onClick.AddListener(ClearAllScores);

        // 점수 리스트 표시
        DisplayScores();

        // 배경음악 재생
        PlayBGM();
    }

    // 점수 리스트를 화면에 표시
    void DisplayScores()
    {
        // 기존 항목들 제거
        ClearScoreItems();

        // ScoreManager에서 점수 데이터 가져오기
        if (ScoreManager.Instance == null)
        {
            ShowNoScoreMessage("No Score Manager!");
            return;
        }

        List<ScoreData> scores = ScoreManager.Instance.Scores;

        // 점수가 없는 경우
        if (scores.Count == 0)
        {
            ShowNoScoreMessage("No Score!");
            return;
        }

        // 기록이 없을 때 메시지 숨기기
        if (noScoreText != null)
            noScoreText.gameObject.SetActive(false);

        // 각 점수에 대해 UI 항목 생성
        for (int i = 0; i < scores.Count; i++)
        {
            CreateScoreItem(scores[i], i);
        }
    }

    // 개별 점수 항목 UI 생성
    void CreateScoreItem(ScoreData scoreData, int index)
    {
        if (scoreItemPrefab == null || scoreListParent == null) return;

        GameObject item = Instantiate(scoreItemPrefab, scoreListParent);
        scoreItems.Add(item);

        // 위치 조정 (간격을 두고 아래로 배치)
        RectTransform itemRect = item.GetComponent<RectTransform>();
        float itemHeight = 30f; // 프리팹 높이 + 간격
        itemRect.anchoredPosition = new Vector2(0, -index * itemHeight);

        // UI 설정
        ScoreItemUI scoreItemUI = item.GetComponent<ScoreItemUI>();
        if (scoreItemUI != null)
        {
            scoreItemUI.SetupScoreItem(scoreData, index * itemAnimationDelay);
        }
        else
        {
            SetupScoreItemDirect(item, scoreData);
        }
    }

    // ScoreItemUI 컴포넌트가 없는 경우 직접 UI 설정
    void SetupScoreItemDirect(GameObject item, ScoreData scoreData)
    {
        // 순위 텍스트 설정
        TextMeshProUGUI rankText = item.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
        if (rankText != null)
        {
            rankText.text = ScoreManager.Instance.GetRankText(scoreData.rank);
            rankText.color = ScoreManager.Instance.GetRankColor(scoreData.rank);
        }

        // 시간 텍스트 설정
        TextMeshProUGUI timeText = item.transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        if (timeText != null)
        {
            timeText.text = ScoreManager.Instance.FormatTime(scoreData.time);
        }

        // 날짜 텍스트 설정
        TextMeshProUGUI dateText = item.transform.Find("DateText")?.GetComponent<TextMeshProUGUI>();
        if (dateText != null)
        {
            dateText.text = scoreData.dateTime;
        }
    }

    // 기존 점수 항목들 제거
    void ClearScoreItems()
    {
        foreach (GameObject item in scoreItems)
        {
            if (item != null)
                Destroy(item);
        }
        scoreItems.Clear();
    }

    // 기록이 없을 때 메시지 표시
    void ShowNoScoreMessage(string message)
    {
        if (noScoreText != null)
        {
            noScoreText.text = message;
            noScoreText.gameObject.SetActive(true);
        }
    }

    // 타이틀로 돌아가기
    public void BackToTitle()
    {
        // 버튼 클릭 효과음 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.uiClick);
        }

        SceneManager.LoadScene(titleSceneName);
    }

    // 모든 점수 초기화
    public void ClearAllScores()
    {
        // 확인 없이 바로 삭제 (필요시 확인 대화상자 추가 가능)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ClearAllScores();
            DisplayScores(); // 화면 갱신

            // 효과음 재생
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.uiClick);
            }
        }
    }

    // 배경음악 재생
    void PlayBGM()
    {
        if (AudioManager.Instance != null)
        {
            // 타이틀 BGM이나 별도의 점수 화면 BGM 재생
            AudioManager.Instance.PlayBGM(AudioManager.Instance.titleBGM);
        }
    }

    // 점수 새로고침 (외부에서 호출 가능)
    public void RefreshScores()
    {
        DisplayScores();
    }
}