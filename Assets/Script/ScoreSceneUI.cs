using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

// ���� ȭ���� UI�� �����ϴ� Ŭ����
public class ScoreSceneUI : MonoBehaviour
{
    [Header("UI ��ҵ�")]
    public Transform scoreListParent;           // ���� ����Ʈ�� �� �θ� Transform
    public GameObject scoreItemPrefab;          // ���� �׸� ������
    public Button backButton;                   // �ڷΰ��� ��ư
    public Button clearScoresButton;            // ���� �ʱ�ȭ ��ư
    public TextMeshProUGUI titleText;           // ���� �ؽ�Ʈ
    public TextMeshProUGUI noScoreText;         // ����� ���� �� ǥ���� �ؽ�Ʈ

    [Header("�� ����")]
    public string titleSceneName = "TitleScene"; // Ÿ��Ʋ �� �̸�

    [Header("�ִϸ��̼� ����")]
    public float itemAnimationDelay = 0.1f;     // �� �׸� �� �ִϸ��̼� ������

    // ������ ���� �׸���� �����ϴ� ����Ʈ
    private List<GameObject> scoreItems = new List<GameObject>();

    void Start()
    {
        // ��ư �̺�Ʈ ����
        if (backButton != null)
            backButton.onClick.AddListener(BackToTitle);

        if (clearScoresButton != null)
            clearScoresButton.onClick.AddListener(ClearAllScores);

        // ���� ����Ʈ ǥ��
        DisplayScores();

        // ������� ���
        PlayBGM();
    }

    // ���� ����Ʈ�� ȭ�鿡 ǥ��
    void DisplayScores()
    {
        // ���� �׸�� ����
        ClearScoreItems();

        // ScoreManager���� ���� ������ ��������
        if (ScoreManager.Instance == null)
        {
            ShowNoScoreMessage("No Score Manager!");
            return;
        }

        List<ScoreData> scores = ScoreManager.Instance.Scores;

        // ������ ���� ���
        if (scores.Count == 0)
        {
            ShowNoScoreMessage("No Score!");
            return;
        }

        // ����� ���� �� �޽��� �����
        if (noScoreText != null)
            noScoreText.gameObject.SetActive(false);

        // �� ������ ���� UI �׸� ����
        for (int i = 0; i < scores.Count; i++)
        {
            CreateScoreItem(scores[i], i);
        }
    }

    // ���� ���� �׸� UI ����
    void CreateScoreItem(ScoreData scoreData, int index)
    {
        if (scoreItemPrefab == null || scoreListParent == null) return;

        GameObject item = Instantiate(scoreItemPrefab, scoreListParent);
        scoreItems.Add(item);

        // ��ġ ���� (������ �ΰ� �Ʒ��� ��ġ)
        RectTransform itemRect = item.GetComponent<RectTransform>();
        float itemHeight = 30f; // ������ ���� + ����
        itemRect.anchoredPosition = new Vector2(0, -index * itemHeight);

        // UI ����
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

    // ScoreItemUI ������Ʈ�� ���� ��� ���� UI ����
    void SetupScoreItemDirect(GameObject item, ScoreData scoreData)
    {
        // ���� �ؽ�Ʈ ����
        TextMeshProUGUI rankText = item.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
        if (rankText != null)
        {
            rankText.text = ScoreManager.Instance.GetRankText(scoreData.rank);
            rankText.color = ScoreManager.Instance.GetRankColor(scoreData.rank);
        }

        // �ð� �ؽ�Ʈ ����
        TextMeshProUGUI timeText = item.transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        if (timeText != null)
        {
            timeText.text = ScoreManager.Instance.FormatTime(scoreData.time);
        }

        // ��¥ �ؽ�Ʈ ����
        TextMeshProUGUI dateText = item.transform.Find("DateText")?.GetComponent<TextMeshProUGUI>();
        if (dateText != null)
        {
            dateText.text = scoreData.dateTime;
        }
    }

    // ���� ���� �׸�� ����
    void ClearScoreItems()
    {
        foreach (GameObject item in scoreItems)
        {
            if (item != null)
                Destroy(item);
        }
        scoreItems.Clear();
    }

    // ����� ���� �� �޽��� ǥ��
    void ShowNoScoreMessage(string message)
    {
        if (noScoreText != null)
        {
            noScoreText.text = message;
            noScoreText.gameObject.SetActive(true);
        }
    }

    // Ÿ��Ʋ�� ���ư���
    public void BackToTitle()
    {
        // ��ư Ŭ�� ȿ���� ���
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.uiClick);
        }

        SceneManager.LoadScene(titleSceneName);
    }

    // ��� ���� �ʱ�ȭ
    public void ClearAllScores()
    {
        // Ȯ�� ���� �ٷ� ���� (�ʿ�� Ȯ�� ��ȭ���� �߰� ����)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ClearAllScores();
            DisplayScores(); // ȭ�� ����

            // ȿ���� ���
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.uiClick);
            }
        }
    }

    // ������� ���
    void PlayBGM()
    {
        if (AudioManager.Instance != null)
        {
            // Ÿ��Ʋ BGM�̳� ������ ���� ȭ�� BGM ���
            AudioManager.Instance.PlayBGM(AudioManager.Instance.titleBGM);
        }
    }

    // ���� ���ΰ�ħ (�ܺο��� ȣ�� ����)
    public void RefreshScores()
    {
        DisplayScores();
    }
}