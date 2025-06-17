using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// 개별 점수 항목의 UI를 관리하는 클래스
public class ScoreItemUI : MonoBehaviour
{
    [Header("UI 컴포넌트들")]
    public TextMeshProUGUI rankText;    // 순위 텍스트
    public TextMeshProUGUI timeText;    // 시간 텍스트
    public TextMeshProUGUI dateText;    // 날짜 텍스트

    [Header("순위별 배경 색상")]
    public Color firstPlaceColor = new Color(1f, 0.84f, 0f, 0.3f);    // 1위: 금색
    public Color secondPlaceColor = new Color(0.75f, 0.75f, 0.75f, 0.3f); // 2위: 은색
    public Color thirdPlaceColor = new Color(0.8f, 0.5f, 0.2f, 0.3f);     // 3위: 동색
    public Color defaultColor = new Color(1f, 1f, 1f, 0.1f);              // 기본: 흰색

    // 점수 데이터
    private ScoreData scoreData;

    void Awake()
    {
        // 컴포넌트 자동 할당 (Inspector에서 설정하지 않은 경우)
        if (rankText == null)
            rankText = transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
        if (timeText == null)
            timeText = transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        if (dateText == null)
            dateText = transform.Find("DateText")?.GetComponent<TextMeshProUGUI>();
    }

    // 점수 항목 설정
    public void SetupScoreItem(ScoreData data, float animationDelay = 0f)
    {
        scoreData = data;

        // UI 텍스트 설정
        SetupTexts();

    }

    // UI 텍스트들 설정
    void SetupTexts()
    {
        if (ScoreManager.Instance == null) return;

        // 순위 텍스트 설정
        if (rankText != null)
        {
            rankText.text = ScoreManager.Instance.GetRankText(scoreData.rank);
            rankText.color = ScoreManager.Instance.GetRankColor(scoreData.rank);

            // 1~3위는 볼드체로 설정
            if (scoreData.rank <= 3)
            {
                rankText.fontStyle = FontStyles.Bold;
            }
        }

        // 시간 텍스트 설정
        if (timeText != null)
        {
            timeText.text = ScoreManager.Instance.FormatTime(scoreData.time);

            // 1위는 특별한 색상으로 표시
            if (scoreData.rank == 1)
            {
                timeText.color = Color.yellow;
                timeText.fontStyle = FontStyles.Bold;
            }
        }

        // 날짜 텍스트 설정
        if (dateText != null)
        {
            dateText.text = scoreData.dateTime;
        }
    }


    // 점수 데이터 반환
    public ScoreData GetScoreData()
    {
        return scoreData;
    }
}