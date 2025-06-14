using UnityEngine;
using System.Collections;

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
    public GameObject key1;
    public GameObject key2;
    public GameObject transparentWall;  // Wave2에만 나타날 투명 벽

    public Transform bossDestinationWave1And3; // 웨이브 1,3 보스 도착지
    public Transform bossDestinationWave2;      // 웨이브 2 보스 도착지

    public float wave2Duration = 60f;
    public float bossMoveDuration = 2f; // 보스 이동 시간

    private float wave2Timer = 0f;
    private bool isWave2Active = false;
    private bool waveCompleted = false;
    private bool gameCleared = false;  // 게임 클리어 상태 추가

    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private MonsterController bossController;
    private SpriteRenderer bossSpriteRenderer;

    // 웨이브별 보스 기본 스프라이트 (인스펙터에 등록)
    public Sprite bossSpriteWave1;
    public Sprite bossSpriteWave2;
    public Sprite bossSpriteWave3;

    public GameObject wave1Spawners;
    public GameObject wave2Spawners;
    public GameObject wave3Spawners;

    // 웨이브별 보스 스케일 (예: 0.7 또는 1.0)
    public Vector3 bossScaleSmall = Vector3.one * 0.7f;
    public Vector3 bossScaleNormal = Vector3.one;

    public GameObject gameClearText;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        playerHealth = player.GetComponent<PlayerHealth>();
        bossController = bossMonster.GetComponent<MonsterController>();
        bossSpriteRenderer = bossMonster.GetComponent<SpriteRenderer>();

        // 게임 시작 시 배경음악 재생 (페이드 인 효과와 함께)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.gameBGM);
        }

        StartWave(currentWave);
    }

    void Update()
    {
        if (waveCompleted || gameCleared) return;  // 게임 클리어 상태도 확인

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
                    gameCleared = true;  // 게임 클리어 상태 설정
                    StartCoroutine(HandleGameClear());  // 바로 게임 클리어 처리
                }
                break;
        }
    }

    public void NextWave()
    {
        if (gameCleared) return;  // 게임이 클리어되었으면 더 이상 진행하지 않음

        // 다음 단계 효과음 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.stageClear);
        }

        ResetHealth();

        switch (currentWave)
        {
            case BattleWave.Wave1_SideScroll:
                currentWave = BattleWave.Wave2_TopDown;
                StartWave(currentWave);
                break;

            case BattleWave.Wave2_TopDown:
                currentWave = BattleWave.Wave3_SideScrollFinal;
                StartWave(currentWave);
                break;

            case BattleWave.Wave3_SideScrollFinal:
                // 웨이브 3은 Update()에서 처리하므로 여기서는 아무것도 하지 않음
                break;
        }
    }

    void StartWave(BattleWave wave)
    {
        GameObject[] monsterAttacks = GameObject.FindGameObjectsWithTag("MonsterAttack");
        foreach (GameObject obj in monsterAttacks)
        {
            Destroy(obj);
        }

        waveCompleted = false;
        if (playerController == null) return;

        if (transparentWall != null)
            transparentWall.SetActive(false);

        // 스포너/패턴 초기화
        DisableAllSpawnersAndPatterns();

        switch (wave)
        {
            case BattleWave.Wave1_SideScroll:
                playerController.SetMoveMode(PlayerController.MoveMode.SideScroll);
                bossMonster.SetActive(true);
                bossController.ResetHealth();
                SetBossSprite(bossSpriteWave1);
                key1.gameObject.SetActive(true);
                key2.gameObject.SetActive(false);
                StartCoroutine(MoveBoss(bossDestinationWave1And3.position, bossScaleNormal));

                // 웨이브 1 전용 스포너/공격 패턴 활성화
                EnableWave1SpawnersAndPatterns();
                break;

            case BattleWave.Wave2_TopDown:
                playerController.SetMoveMode(PlayerController.MoveMode.TopDown);
                bossMonster.SetActive(true);
                bossController.ResetHealth();
                wave2Timer = 0f;
                isWave2Active = true;
                if (transparentWall != null)
                    transparentWall.SetActive(true);
                SetBossSprite(bossSpriteWave2);
                key1.gameObject.SetActive(false);
                key2.gameObject.SetActive(true);
                StartCoroutine(MoveBoss(bossDestinationWave2.position, bossScaleSmall));

                // 웨이브 2 전용 스포너/공격 패턴 활성화
                EnableWave2SpawnersAndPatterns();
                break;

            case BattleWave.Wave3_SideScrollFinal:
                playerController.SetMoveMode(PlayerController.MoveMode.SideScroll);
                bossMonster.SetActive(true);
                bossController.ResetHealth();
                SetBossSprite(bossSpriteWave3);
                key1.gameObject.SetActive(true);
                key2.gameObject.SetActive(false);
                StartCoroutine(MoveBoss(bossDestinationWave1And3.position, bossScaleNormal));

                // 웨이브 3 전용 스포너/공격 패턴 활성화
                EnableWave3SpawnersAndPatterns();
                break;
        }

        Debug.Log($"Wave 시작: {wave}");
    }

    void SetBossSprite(Sprite sprite)
    {
        if (bossSpriteRenderer != null && sprite != null)
            bossSpriteRenderer.sprite = sprite;
        bossController.defaultSprite = sprite;
    }

    IEnumerator MoveBoss(Vector3 targetPosition, Vector3 targetScale)
    {
        Vector3 startPos = bossMonster.transform.position;
        Vector3 startScale = bossMonster.transform.localScale;
        float elapsed = 0f;

        while (elapsed < bossMoveDuration)
        {
            float t = elapsed / bossMoveDuration;
            bossMonster.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            bossMonster.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        bossMonster.transform.position = targetPosition;
        bossMonster.transform.localScale = targetScale;
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

    void DisableAllSpawnersAndPatterns()
    {
        if (wave1Spawners) wave1Spawners.SetActive(false);
        if (wave2Spawners) wave2Spawners.SetActive(false);
        if (wave3Spawners) wave3Spawners.SetActive(false);
    }

    void EnableWave1SpawnersAndPatterns()
    {
        if (wave1Spawners) wave1Spawners.SetActive(true);

    }

    void EnableWave2SpawnersAndPatterns()
    {
        if (wave2Spawners) wave2Spawners.SetActive(true);
    }

    void EnableWave3SpawnersAndPatterns()
    {
        if (wave3Spawners) wave3Spawners.SetActive(true);
    }

    IEnumerator HandleGameClear()
    {
        DisableAllSpawnersAndPatterns();

        foreach (var spawner in wave3Spawners.GetComponentsInChildren<BombSpawner>())
        {
            spawner.enabled = false;
        }

        GameObject[] monsterAttacks = GameObject.FindGameObjectsWithTag("MonsterAttack");
        foreach (GameObject obj in monsterAttacks)
        {
            Destroy(obj);
        }

        // 보스 천천히 아래로 이동하며 사라짐
        float duration = 1.8f;
        float elapsed = 0f;
        Vector3 startPos = bossMonster.transform.position;
        Vector3 endPos = startPos + Vector3.down * 0.56f;

        float shakeMagnitude = 0.05f; // 진동 세기

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // 기본 위치 보간
            Vector3 lerpedPos = Vector3.Lerp(startPos, endPos, t);

            // 진동 추가 (X, Y 방향으로 랜덤 오프셋)
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude),
                0f
            );

            bossMonster.transform.position = lerpedPos + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        bossMonster.SetActive(false); // 완전히 사라지게

        // 게임 클리어 텍스트 표시
        if (gameClearText != null)
        {
            gameClearText.SetActive(true);
        }

        Debug.Log("Game Clear 처리 완료!");
    }
}