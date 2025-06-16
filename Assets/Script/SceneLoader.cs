using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }  // 싱글톤 인스턴스

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject);  // 중복된 인스턴스 제거
        }
    }

    // 씬 이름으로 씬을 로드하며 배경음악을 정지함
    public void LoadSceneByName(string sceneName)
    {
        // 현재 재생 중인 배경음악 정지
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        // 씬 로드
        SceneManager.LoadScene(sceneName);
    }
}
