using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// 점수를 관리하고 순위를 매기는 매니저 클래스
[System.Serializable]
public class ScoreData
{
    public float time;           // 클리어 시간 (초)
    public string dateTime;      // 클리어한 날짜와 시간
    public int rank;             // 순위

    public ScoreData(float time)
    {
        this.time = time;
        this.dateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
    }
}

public class ScoreManager : MonoBehaviour
{
    [Header("점수 설정")]
    public int maxScoreCount = 10; // 저장할 최대 점수 개수

    // 점수 리스트
    private List<ScoreData> scores = new List<ScoreData>();

    // 싱글톤 패턴 구현
    public static ScoreManager Instance { get; private set; }

    // 점수 리스트를 읽기 전용으로 제공
    public List<ScoreData> Scores => scores.ToList();

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
            LoadScores(); // 저장된 점수 불러오기
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // 새로운 점수 추가
    public void AddScore(float clearTime)
    {
        // 새로운 점수 데이터 생성
        ScoreData newScore = new ScoreData(clearTime);
        scores.Add(newScore);

        // 시간 기준으로 오름차순 정렬 (짧은 시간이 더 좋은 점수)
        scores = scores.OrderBy(s => s.time).ToList();

        // 순위 업데이트
        UpdateRanks();

        // 최대 개수를 초과하면 가장 낮은 점수 제거
        if (scores.Count > maxScoreCount)
        {
            scores.RemoveRange(maxScoreCount, scores.Count - maxScoreCount);
        }

        // 점수 저장
        SaveScores();

        Debug.Log($"새 점수 추가: {FormatTime(clearTime)} - 순위: {GetScoreRank(clearTime)}");
    }

    // 특정 점수의 순위 반환
    public int GetScoreRank(float time)
    {
        var scoreData = scores.FirstOrDefault(s => s.time == time);
        return scoreData?.rank ?? -1;
    }

    // 최고 기록(1위) 반환
    public ScoreData GetBestScore()
    {
        return scores.FirstOrDefault();
    }

    // 순위 업데이트
    void UpdateRanks()
    {
        for (int i = 0; i < scores.Count; i++)
        {
            scores[i].rank = i + 1;
        }
    }

    // 점수를 PlayerPrefs에 저장
    void SaveScores()
    {
        // 점수 개수 저장
        PlayerPrefs.SetInt("ScoreCount", scores.Count);

        // 각 점수 데이터 저장
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetFloat($"Score_Time_{i}", scores[i].time);
            PlayerPrefs.SetString($"Score_DateTime_{i}", scores[i].dateTime);
            PlayerPrefs.SetInt($"Score_Rank_{i}", scores[i].rank);
        }

        PlayerPrefs.Save();
        Debug.Log($"점수 저장 완료: {scores.Count}개");
    }

    // PlayerPrefs에서 점수 불러오기
    void LoadScores()
    {
        scores.Clear();

        int scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);

        for (int i = 0; i < scoreCount; i++)
        {
            if (PlayerPrefs.HasKey($"Score_Time_{i}"))
            {
                ScoreData scoreData = new ScoreData(PlayerPrefs.GetFloat($"Score_Time_{i}"));
                scoreData.dateTime = PlayerPrefs.GetString($"Score_DateTime_{i}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
                scoreData.rank = PlayerPrefs.GetInt($"Score_Rank_{i}", i + 1);

                scores.Add(scoreData);
            }
        }

        // 혹시 순위가 잘못되었을 경우를 대비해 다시 정렬 및 순위 업데이트
        scores = scores.OrderBy(s => s.time).ToList();
        UpdateRanks();

        Debug.Log($"점수 불러오기 완료: {scores.Count}개");
    }

    // 모든 점수 삭제
    public void ClearAllScores()
    {
        scores.Clear();

        // PlayerPrefs에서도 삭제
        int scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);
        for (int i = 0; i < scoreCount; i++)
        {
            PlayerPrefs.DeleteKey($"Score_Time_{i}");
            PlayerPrefs.DeleteKey($"Score_DateTime_{i}");
            PlayerPrefs.DeleteKey($"Score_Rank_{i}");
        }

        PlayerPrefs.SetInt("ScoreCount", 0);
        PlayerPrefs.Save();

        Debug.Log("모든 점수 삭제 완료");
    }

    // 시간을 MM:SS.ff 형식으로 포맷
    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int centiseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, centiseconds);
    }

    // 순위별 색상 반환 (UI에서 사용)
    public Color GetRankColor(int rank)
    {
        switch (rank)
        {
            case 1: return Color.yellow;      // 1위: 금색
            case 2: return Color.gray;        // 2위: 은색
            case 3: return new Color(0.8f, 0.5f, 0.2f); // 3위: 동색
            default: return Color.white;      // 그 외: 흰색
        }
    }

    // 순위별 텍스트 반환
    public string GetRankText(int rank)
    {
        switch (rank)
        {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
            default: return $"{rank}th";
        }
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