using UnityEngine;

public class BattleWaveManager : MonoBehaviour
{
    public enum BattleWave
    {
        Wave1_SideScroll,
        Wave2_TopDown,
        Wave3_SideScrollFinal
    }

    public BattleWave currentWave = BattleWave.Wave1_SideScroll;

    public GameObject player;
    public GameObject bossMonster;
    public GameObject transparentWall;  // Wave2에만 나타날 투명 벽

    public float wave2Duration = 60f;

    private float wave2Timer = 0f;
    private bool isWave2Active = false;
    private bool waveCompleted = false;

    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private MonsterController bossController;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        playerHealth = player.GetComponent<PlayerHealth>();
        bossController = bossMonster.GetComponent<MonsterController>();

        StartWave(currentWave);
    }

    void Update()
    {
        if (waveCompleted) return;

        switch (currentWave)
        {
            case BattleWave.Wave1_SideScroll:
                if (bossController.currentHealth <= 0)
                {
                    Debug.Log("Wave1 클리어!");
                    waveCompleted = true;
                    NextWave();
                }
                break;

            case BattleWave.Wave2_TopDown:
                if (isWave2Active)
                {
                    wave2Timer += Time.deltaTime;
                    if (wave2Timer >= wave2Duration)
                    {
                        Debug.Log("Wave2 클리어!");
                        waveCompleted = true;
                        isWave2Active = false;
                        NextWave();
                    }
                }
                break;

            case BattleWave.Wave3_SideScrollFinal:
                if (bossController != null && bossController.currentHealth <= 0)
                {
                    Debug.Log("모든 웨이브 클리어!");
                    waveCompleted = true;
                    // 게임 클리어 처리 로직 추가
                }
                break;
        }
    }

    public void NextWave()
    {
        ResetHealth();

        switch (currentWave)
        {
            case BattleWave.Wave1_SideScroll:
                currentWave = BattleWave.Wave2_TopDown;
                break;

            case BattleWave.Wave2_TopDown:
                currentWave = BattleWave.Wave3_SideScrollFinal;
                break;

            case BattleWave.Wave3_SideScrollFinal:
                Debug.Log("게임 클리어!");
                return;
        }

        StartWave(currentWave);
    }

    void StartWave(BattleWave wave)
    {
        waveCompleted = false;
        if (playerController == null) return;

        // 모든 웨이브 시작 전에 기본적으로 벽 비활성화
        if (transparentWall != null)
            transparentWall.SetActive(false);

        switch (wave)
        {
            case BattleWave.Wave1_SideScroll:
                playerController.SetMoveMode(PlayerController.MoveMode.SideScroll);
                ActivateBoss(); // 보스 활성화
                break;

            case BattleWave.Wave2_TopDown:
                playerController.SetMoveMode(PlayerController.MoveMode.TopDown);
                bossMonster.SetActive(false); // 보스 비활성화
                wave2Timer = 0f;
                isWave2Active = true;

                if (transparentWall != null)
                    transparentWall.SetActive(true); // 투명 벽 활성화
                break;

            case BattleWave.Wave3_SideScrollFinal:
                playerController.SetMoveMode(PlayerController.MoveMode.SideScroll);
                ActivateBoss(); // 보스 활성화
                break;
        }

        Debug.Log($"Wave 시작: {wave}");
    }

    void ActivateBoss()
    {
        bossMonster.SetActive(true);
        bossController.ResetHealth(); // 체력도 초기화
    }

    void ResetHealth()
    {
        if (playerHealth != null)
        {
            playerHealth.currentHealth = PlayerHealth.maxHealth;
        }

        if (bossController != null)
        {
            bossController.ResetHealth();
        }
    }
}
