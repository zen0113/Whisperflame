using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    void Start()
    {
        // 타이틀 씬 시작 시 배경음악 재생 (페이드 인 효과와 함께)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.titleBGM);
        }
    }

    // 게임 시작 버튼 클릭 시 호출될 함수
    public void StartGame()
    {
        // UI 클릭 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.uiClick);
        }

        // SceneLoader를 통해 씬 전환
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneByName("PlayerScene");
        }
        else
        {
            Debug.LogError("SceneLoader not found!");
        }
    }
} 