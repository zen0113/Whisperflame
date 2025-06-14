using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        // 현재 씬의 배경음악 중지
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        // 씬 전환
        SceneManager.LoadScene(sceneName);
    }
}
