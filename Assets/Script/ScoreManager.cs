using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// ������ �����ϰ� ������ �ű�� �Ŵ��� Ŭ����
[System.Serializable]
public class ScoreData
{
    public float time;           // Ŭ���� �ð� (��)
    public string dateTime;      // Ŭ������ ��¥�� �ð�
    public int rank;             // ����

    public ScoreData(float time)
    {
        this.time = time;
        this.dateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
    }
}

public class ScoreManager : MonoBehaviour
{
    [Header("���� ����")]
    public int maxScoreCount = 10; // ������ �ִ� ���� ����

    // ���� ����Ʈ
    private List<ScoreData> scores = new List<ScoreData>();

    // �̱��� ���� ����
    public static ScoreManager Instance { get; private set; }

    // ���� ����Ʈ�� �б� �������� ����
    public List<ScoreData> Scores => scores.ToList();

    void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ����
            LoadScores(); // ����� ���� �ҷ�����
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ���ο� ���� �߰�
    public void AddScore(float clearTime)
    {
        // ���ο� ���� ������ ����
        ScoreData newScore = new ScoreData(clearTime);
        scores.Add(newScore);

        // �ð� �������� �������� ���� (ª�� �ð��� �� ���� ����)
        scores = scores.OrderBy(s => s.time).ToList();

        // ���� ������Ʈ
        UpdateRanks();

        // �ִ� ������ �ʰ��ϸ� ���� ���� ���� ����
        if (scores.Count > maxScoreCount)
        {
            scores.RemoveRange(maxScoreCount, scores.Count - maxScoreCount);
        }

        // ���� ����
        SaveScores();

        Debug.Log($"�� ���� �߰�: {FormatTime(clearTime)} - ����: {GetScoreRank(clearTime)}");
    }

    // Ư�� ������ ���� ��ȯ
    public int GetScoreRank(float time)
    {
        var scoreData = scores.FirstOrDefault(s => s.time == time);
        return scoreData?.rank ?? -1;
    }

    // �ְ� ���(1��) ��ȯ
    public ScoreData GetBestScore()
    {
        return scores.FirstOrDefault();
    }

    // ���� ������Ʈ
    void UpdateRanks()
    {
        for (int i = 0; i < scores.Count; i++)
        {
            scores[i].rank = i + 1;
        }
    }

    // ������ PlayerPrefs�� ����
    void SaveScores()
    {
        // ���� ���� ����
        PlayerPrefs.SetInt("ScoreCount", scores.Count);

        // �� ���� ������ ����
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetFloat($"Score_Time_{i}", scores[i].time);
            PlayerPrefs.SetString($"Score_DateTime_{i}", scores[i].dateTime);
            PlayerPrefs.SetInt($"Score_Rank_{i}", scores[i].rank);
        }

        PlayerPrefs.Save();
        Debug.Log($"���� ���� �Ϸ�: {scores.Count}��");
    }

    // PlayerPrefs���� ���� �ҷ�����
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

        // Ȥ�� ������ �߸��Ǿ��� ��츦 ����� �ٽ� ���� �� ���� ������Ʈ
        scores = scores.OrderBy(s => s.time).ToList();
        UpdateRanks();

        Debug.Log($"���� �ҷ����� �Ϸ�: {scores.Count}��");
    }

    // ��� ���� ����
    public void ClearAllScores()
    {
        scores.Clear();

        // PlayerPrefs������ ����
        int scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);
        for (int i = 0; i < scoreCount; i++)
        {
            PlayerPrefs.DeleteKey($"Score_Time_{i}");
            PlayerPrefs.DeleteKey($"Score_DateTime_{i}");
            PlayerPrefs.DeleteKey($"Score_Rank_{i}");
        }

        PlayerPrefs.SetInt("ScoreCount", 0);
        PlayerPrefs.Save();

        Debug.Log("��� ���� ���� �Ϸ�");
    }

    // �ð��� MM:SS.ff �������� ����
    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int centiseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, centiseconds);
    }

    // ������ ���� ��ȯ (UI���� ���)
    public Color GetRankColor(int rank)
    {
        switch (rank)
        {
            case 1: return Color.yellow;      // 1��: �ݻ�
            case 2: return Color.gray;        // 2��: ����
            case 3: return new Color(0.8f, 0.5f, 0.2f); // 3��: ����
            default: return Color.white;      // �� ��: ���
        }
    }

    // ������ �ؽ�Ʈ ��ȯ
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
        // �ν��Ͻ��� �ı��� �� null�� ����
        if (Instance == this)
        {
            Instance = null;
        }
    }
}