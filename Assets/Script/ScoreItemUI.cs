using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// ���� ���� �׸��� UI�� �����ϴ� Ŭ����
public class ScoreItemUI : MonoBehaviour
{
    [Header("UI ������Ʈ��")]
    public TextMeshProUGUI rankText;    // ���� �ؽ�Ʈ
    public TextMeshProUGUI timeText;    // �ð� �ؽ�Ʈ
    public TextMeshProUGUI dateText;    // ��¥ �ؽ�Ʈ

    [Header("������ ��� ����")]
    public Color firstPlaceColor = new Color(1f, 0.84f, 0f, 0.3f);    // 1��: �ݻ�
    public Color secondPlaceColor = new Color(0.75f, 0.75f, 0.75f, 0.3f); // 2��: ����
    public Color thirdPlaceColor = new Color(0.8f, 0.5f, 0.2f, 0.3f);     // 3��: ����
    public Color defaultColor = new Color(1f, 1f, 1f, 0.1f);              // �⺻: ���

    // ���� ������
    private ScoreData scoreData;

    void Awake()
    {
        // ������Ʈ �ڵ� �Ҵ� (Inspector���� �������� ���� ���)
        if (rankText == null)
            rankText = transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
        if (timeText == null)
            timeText = transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        if (dateText == null)
            dateText = transform.Find("DateText")?.GetComponent<TextMeshProUGUI>();
    }

    // ���� �׸� ����
    public void SetupScoreItem(ScoreData data, float animationDelay = 0f)
    {
        scoreData = data;

        // UI �ؽ�Ʈ ����
        SetupTexts();

    }

    // UI �ؽ�Ʈ�� ����
    void SetupTexts()
    {
        if (ScoreManager.Instance == null) return;

        // ���� �ؽ�Ʈ ����
        if (rankText != null)
        {
            rankText.text = ScoreManager.Instance.GetRankText(scoreData.rank);
            rankText.color = ScoreManager.Instance.GetRankColor(scoreData.rank);

            // 1~3���� ����ü�� ����
            if (scoreData.rank <= 3)
            {
                rankText.fontStyle = FontStyles.Bold;
            }
        }

        // �ð� �ؽ�Ʈ ����
        if (timeText != null)
        {
            timeText.text = ScoreManager.Instance.FormatTime(scoreData.time);

            // 1���� Ư���� �������� ǥ��
            if (scoreData.rank == 1)
            {
                timeText.color = Color.yellow;
                timeText.fontStyle = FontStyles.Bold;
            }
        }

        // ��¥ �ؽ�Ʈ ����
        if (dateText != null)
        {
            dateText.text = scoreData.dateTime;
        }
    }


    // ���� ������ ��ȯ
    public ScoreData GetScoreData()
    {
        return scoreData;
    }
}