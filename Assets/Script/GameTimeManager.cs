using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

// ���� �ð��� �����ϰ� Ŭ���� �� �ڵ����� Ÿ��Ʋ�� �̵��ϴ� �Ŵ���
public class GameTimeManager : MonoBehaviour
{
    [Header("���̵� ����")]
    public Image fadePanel;         // ���̵� �ƿ��� �̹��� �г�
    public float fadeDuration = 1f; // ���̵� ���� �ð�

    [Header("Ÿ��Ʋ �̵� ����")]
    public string titleSceneName = "TitleScene"; // Ÿ��Ʋ �� �̸�
    public float waitTimeAfterClear = 5f;        // ���� Ŭ���� �� ��� �ð�

    // �ð� ���� ���� ������
    private float gameStartTime;    // ���� ���� �ð�
    private float gameEndTime;      // ���� ���� �ð�
    private float totalGameTime;    // �� ���� �÷��� �ð�
    private bool gameEnded = false; // ���� ���� ����

    // �̱��� ���� ����
    public static GameTimeManager Instance { get; private set; }

    // ���� ���� �ð��� �������� ������Ƽ
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
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ����
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // ���� ���� �ð� ���
        StartGameTimer();

        // ���̵� �г� �ʱ� ���� (���� ����)
        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 0);
            fadePanel.gameObject.SetActive(true);
        }
    }

    // ���� Ÿ�̸� ����
    public void StartGameTimer()
    {
        gameStartTime = Time.time;
        gameEnded = false;
        Debug.Log("���� Ÿ�̸� ����!");
    }

    // ���� ���� �� �ڵ� Ÿ��Ʋ �̵� ó��
    public void EndGame()
    {
        if (gameEnded) return; // �̹� ����� ��� �ߺ� ���� ����

        gameEndTime = Time.time;
        totalGameTime = gameEndTime - gameStartTime;
        gameEnded = true;

        Debug.Log($"���� Ŭ����! �� �÷��� �ð�: {FormatTime(totalGameTime)}");

        // ���� ����
        SaveScore();

        // ������ �ð� �� �ڵ����� Ÿ��Ʋ�� �̵�
        StartCoroutine(AutoReturnToTitle());
    }

    // ������ �����ϴ� �޼���
    void SaveScore()
    {
        // ScoreManager�� ���� ����
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(totalGameTime);
        }
        else
        {
            // ScoreManager�� ���� ��� PlayerPrefs�� ���� ����
            Debug.LogWarning("ScoreManager not found, saving directly to PlayerPrefs");
            SaveScoreToPlayerPrefs();
        }
    }

    // PlayerPrefs�� ���� ���� ���� (�����)
    void SaveScoreToPlayerPrefs()
    {
        // ���� ������ �ҷ�����
        int scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);

        // �� ���� �߰�
        PlayerPrefs.SetFloat($"Score_{scoreCount}", totalGameTime);
        PlayerPrefs.SetInt("ScoreCount", scoreCount + 1);
        PlayerPrefs.Save();
    }

    // �ڵ����� Ÿ��Ʋ�� ���ư��� �ڷ�ƾ
    IEnumerator AutoReturnToTitle()
    {
        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(waitTimeAfterClear);

        // ���̵� �ƿ� ����
        yield return StartCoroutine(FadeToBlack());

        // Ÿ��Ʋ ������ �̵�
        SceneManager.LoadScene(titleSceneName);
    }

    // ���̵� �ƿ� ȿ�� �ڷ�ƾ
    IEnumerator FadeToBlack()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color startColor = fadePanel.color;
        Color targetColor = new Color(0, 0, 0, 1); // ������ ����

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            fadePanel.color = Color.Lerp(startColor, targetColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadePanel.color = targetColor;
    }

    // �ð��� MM:SS �������� �����ϴ� �޼���
    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // ���� ���� �ð��� ���˵� ���ڿ��� ��ȯ
    public string GetFormattedCurrentTime()
    {
        return FormatTime(CurrentGameTime);
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