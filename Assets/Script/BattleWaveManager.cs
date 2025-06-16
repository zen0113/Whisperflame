using UnityEngine;
using System.Collections;

// 배틀 웨이브를 관리하는 메인 매니저 클래스
// 3개의 웨이브(사이드스크롤 -> 탑다운 -> 최종 사이드스크롤)를 순차적으로 진행
public class BattleWaveManager : MonoBehaviour
{
    // 배틀 웨이브 종류를 정의하는 열거형
    public enum BattleWave
    {
        Wave1_SideScroll,      // 웨이브 1: 사이드스크롤 방식
        Wave2_TopDown,         // 웨이브 2: 탑다운 방식
        Wave3_SideScrollFinal  // 웨이브 3: 최종 사이드스크롤 방식
    }

    [Header("웨이브 설정")]
    public BattleWave currentWave = BattleWave.Wave1_SideScroll; // 현재 진행 중인 웨이브

    [Header("게임 오브젝트 참조")]
    public GameObject player;        // 플레이어 게임오브젝트
    public GameObject bossMonster;   // 보스 몬스터 게임오브젝트
    public GameObject key1;          // 첫 번째 키 UI (웨이브 1, 3에서 사용)
    public GameObject key2;          // 두 번째 키 UI (웨이브 2에서 사용)
    public GameObject transparentWall; // 투명 벽 (웨이브 2에서만 활성화)

    [Header("보스 위치 설정")]
    public Transform bossDestinationWave1And3; // 웨이브 1, 3에서 보스가 이동할 목표 위치
    public Transform bossDestinationWave2;     // 웨이브 2에서 보스가 이동할 목표 위치

    [Header("타이밍 설정")]
    public float wave2Duration = 60f;    // 웨이브 2의 지속 시간 (초)
    public float bossMoveDuration = 2f;  // 보스가 목표 위치로 이동하는 데 걸리는 시간

    // 웨이브 2 관련 타이머 및 상태 변수들
    private float wave2Timer = 0f;       // 웨이브 2 경과 시간 측정용 타이머
    private bool isWave2Active = false;  // 웨이브 2가 현재 활성화 상태인지 확인
    private bool waveCompleted = false;  // 현재 웨이브가 완료되었는지 확인
    private bool gameCleared = false;    // 전체 게임이 클리어되었는지 확인

    // 컴포넌트 참조 변수들
    private PlayerController playerController; // 플레이어 컨트롤러 컴포넌트
    private PlayerHealth playerHealth;         // 플레이어 체력 관리 컴포넌트
    private MonsterController bossController;  // 보스 몬스터 컨트롤러 컴포넌트
    private SpriteRenderer bossSpriteRenderer; // 보스 스프라이트 렌더러 컴포넌트

    [Header("웨이브별 보스 스프라이트")]
    public Sprite bossSpriteWave1; // 웨이브 1용 보스 스프라이트
    public Sprite bossSpriteWave2; // 웨이브 2용 보스 스프라이트
    public Sprite bossSpriteWave3; // 웨이브 3용 보스 스프라이트

    [Header("웨이브별 스포너 그룹")]
    public GameObject wave1Spawners; // 웨이브 1에서 활성화할 스포너들의 부모 오브젝트
    public GameObject wave2Spawners; // 웨이브 2에서 활성화할 스포너들의 부모 오브젝트
    public GameObject wave3Spawners; // 웨이브 3에서 활성화할 스포너들의 부모 오브젝트

    [Header("보스 크기 설정")]
    public Vector3 bossScaleSmall = Vector3.one * 0.7f; // 작은 크기 (웨이브 2용)
    public Vector3 bossScaleNormal = Vector3.one;       // 일반 크기 (웨이브 1, 3용)

    [Header("UI 요소")]
    public GameObject gameClearText; // 게임 클리어 시 표시할 텍스트 UI

    void Start()
    {
        // 필요한 컴포넌트들을 미리 캐싱하여 성능 최적화
        playerController = player.GetComponent<PlayerController>();
        playerHealth = player.GetComponent<PlayerHealth>();
        bossController = bossMonster.GetComponent<MonsterController>();
        bossSpriteRenderer = bossMonster.GetComponent<SpriteRenderer>();

        // 게임 시작 시 배경음악 재생 (AudioManager가 존재하는 경우)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.gameBGM);
        }

        // 첫 번째 웨이브 시작
        StartWave(currentWave);
    }

    void Update()
    {
        // 웨이브가 완료되었거나 게임이 클리어된 경우 더 이상 진행하지 않음
        if (waveCompleted || gameCleared) return;

        // 현재 웨이브에 따라 다른 클리어 조건 체크
        switch (currentWave)
        {
            case BattleWave.Wave1_SideScroll:
                // 웨이브 1: 보스의 체력이 0 이하가 되면 클리어
                if (bossController.currentHealth <= 0)
                {
                    Debug.Log("Wave1 클리어!");
                    waveCompleted = true;
                    NextWave(); // 다음 웨이브로 진행
                }
                break;

            case BattleWave.Wave2_TopDown:
                // 웨이브 2: 설정된 시간(60초)이 지나면 클리어
                if (isWave2Active)
                {
                    wave2Timer += Time.deltaTime; // 타이머 증가
                    if (wave2Timer >= wave2Duration)
                    {
                        Debug.Log("Wave2 클리어!");
                        waveCompleted = true;
                        isWave2Active = false;
                        NextWave(); // 다음 웨이브로 진행
                    }
                }
                break;

            case BattleWave.Wave3_SideScrollFinal:
                // 웨이브 3 (최종): 보스의 체력이 0 이하가 되면 게임 완전 클리어
                if (bossController != null && bossController.currentHealth <= 0)
                {
                    Debug.Log("모든 웨이브 클리어!");
                    waveCompleted = true;
                    gameCleared = true; // 게임 완전 클리어 상태로 설정
                    StartCoroutine(HandleGameClear()); // 게임 클리어 연출 시작
                }
                break;
        }
    }

    // 다음 웨이브로 진행하는 메서드
    // 웨이브 전환 시 필요한 초기화 작업과 다음 웨이브 시작
    public void NextWave()
    {
        // 게임이 이미 클리어된 경우 더 이상 진행하지 않음
        if (gameCleared) return;

        // 웨이브 클리어 효과음 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.stageClear);
        }

        // 플레이어와 보스의 체력을 초기화
        ResetHealth();

        // 현재 웨이브에 따라 다음 웨이브 설정 및 시작
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
                // 웨이브 3은 Update()에서 게임 클리어 처리를 하므로 여기서는 아무것도 하지 않음
                break;
        }
    }

    // 지정된 웨이브를 시작하는 메서드
    // 각 웨이브에 맞는 설정(이동 모드, 보스 위치/크기, UI 요소 등)을 적용
    void StartWave(BattleWave wave)
    {
        // 이전 웨이브에서 남은 몬스터 공격 오브젝트들을 모두 제거
        GameObject[] monsterAttacks = GameObject.FindGameObjectsWithTag("MonsterAttack");
        foreach (GameObject obj in monsterAttacks)
        {
            Destroy(obj);
        }

        // 웨이브 완료 상태를 초기화
        waveCompleted = false;

        // 플레이어 컨트롤러가 없으면 진행하지 않음
        if (playerController == null) return;

        // 투명 벽을 기본적으로 비활성화
        if (transparentWall != null)
            transparentWall.SetActive(false);

        // 모든 스포너와 공격 패턴을 비활성화
        DisableAllSpawnersAndPatterns();

        // 웨이브별 설정 적용
        switch (wave)
        {
            case BattleWave.Wave1_SideScroll:
                // 플레이어를 사이드스크롤 모드로 설정
                playerController.SetMoveMode(PlayerController.MoveMode.SideScroll);

                // 보스 활성화 및 체력 초기화
                bossMonster.SetActive(true);
                bossController.ResetHealth();

                // 웨이브 1용 보스 스프라이트 설정
                SetBossSprite(bossSpriteWave1);

                // 키 UI 설정 (key1 활성화, key2 비활성화)
                key1.gameObject.SetActive(true);
                key2.gameObject.SetActive(false);

                // 보스를 웨이브 1&3용 위치로 이동시키고 일반 크기로 설정
                StartCoroutine(MoveBoss(bossDestinationWave1And3.position, bossScaleNormal));

                // 웨이브 1 전용 스포너와 공격 패턴 활성화
                EnableWave1SpawnersAndPatterns();
                break;

            case BattleWave.Wave2_TopDown:
                // 플레이어를 탑다운 모드로 설정
                playerController.SetMoveMode(PlayerController.MoveMode.TopDown);

                // 보스 활성화 및 체력 초기화
                bossMonster.SetActive(true);
                bossController.ResetHealth();

                // 웨이브 2 타이머 초기화 및 활성화
                wave2Timer = 0f;
                isWave2Active = true;

                // 웨이브 2에서만 투명 벽 활성화
                if (transparentWall != null)
                    transparentWall.SetActive(true);

                // 웨이브 2용 보스 스프라이트 설정
                SetBossSprite(bossSpriteWave2);

                // 키 UI 설정 (key1 비활성화, key2 활성화)
                key1.gameObject.SetActive(false);
                key2.gameObject.SetActive(true);

                // 보스를 웨이브 2용 위치로 이동시키고 작은 크기로 설정
                StartCoroutine(MoveBoss(bossDestinationWave2.position, bossScaleSmall));

                // 웨이브 2 전용 스포너와 공격 패턴 활성화
                EnableWave2SpawnersAndPatterns();
                break;

            case BattleWave.Wave3_SideScrollFinal:
                // 플레이어를 사이드스크롤 모드로 설정
                playerController.SetMoveMode(PlayerController.MoveMode.SideScroll);

                // 보스 활성화 및 체력 초기화
                bossMonster.SetActive(true);
                bossController.ResetHealth();

                // 웨이브 3용 보스 스프라이트 설정
                SetBossSprite(bossSpriteWave3);

                // 키 UI 설정 (key1 활성화, key2 비활성화)
                key1.gameObject.SetActive(true);
                key2.gameObject.SetActive(false);

                // 보스를 웨이브 1&3용 위치로 이동시키고 일반 크기로 설정
                StartCoroutine(MoveBoss(bossDestinationWave1And3.position, bossScaleNormal));

                // 웨이브 3 전용 스포너와 공격 패턴 활성화
                EnableWave3SpawnersAndPatterns();
                break;
        }

        Debug.Log($"Wave 시작: {wave}");
    }

    // 보스의 스프라이트를 변경하는 메서드
    void SetBossSprite(Sprite sprite)
    {
        // 스프라이트 렌더러와 스프라이트가 모두 유효한 경우에만 변경
        if (bossSpriteRenderer != null && sprite != null)
            bossSpriteRenderer.sprite = sprite;

        // 보스 컨트롤러의 기본 스프라이트도 함께 업데이트
        bossController.defaultSprite = sprite;
    }

    // 보스를 지정된 위치와 크기로 부드럽게 이동시키는 코루틴
    IEnumerator MoveBoss(Vector3 targetPosition, Vector3 targetScale)
    {
        // 시작 위치와 크기 저장
        Vector3 startPos = bossMonster.transform.position;
        Vector3 startScale = bossMonster.transform.localScale;
        float elapsed = 0f; // 경과 시간

        // 설정된 이동 시간 동안 보간하여 부드럽게 이동
        while (elapsed < bossMoveDuration)
        {
            float t = elapsed / bossMoveDuration; // 0~1 사이의 보간 비율 계산

            // 위치와 크기를 선형 보간으로 부드럽게 변경
            bossMonster.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            bossMonster.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 최종 위치와 크기를 정확히 설정 (부동소수점 오차 방지)
        bossMonster.transform.position = targetPosition;
        bossMonster.transform.localScale = targetScale;
    }

    // 플레이어와 보스의 체력을 초기화하는 메서드
    // 웨이브 전환 시 호출됨
    void ResetHealth()
    {
        // 플레이어 체력을 최대 체력으로 회복
        if (playerHealth != null)
        {
            playerHealth.currentHealth = PlayerHealth.maxHealth;
        }

        // 보스 체력을 초기 상태로 리셋
        if (bossController != null)
        {
            bossController.ResetHealth();
        }
    }

    // 모든 웨이브의 스포너와 공격 패턴을 비활성화하는 메서드
    // 웨이브 전환 시 이전 웨이브의 요소들을 정리하기 위해 사용
    void DisableAllSpawnersAndPatterns()
    {
        if (wave1Spawners) wave1Spawners.SetActive(false);
        if (wave2Spawners) wave2Spawners.SetActive(false);
        if (wave3Spawners) wave3Spawners.SetActive(false);
    }

    // 웨이브 1 전용 스포너와 공격 패턴을 활성화하는 메서드
    void EnableWave1SpawnersAndPatterns()
    {
        if (wave1Spawners) wave1Spawners.SetActive(true);
    }

    // 웨이브 2 전용 스포너와 공격 패턴을 활성화하는 메서드
    void EnableWave2SpawnersAndPatterns()
    {
        if (wave2Spawners) wave2Spawners.SetActive(true);
    }

    // 웨이브 3 전용 스포너와 공격 패턴을 활성화하는 메서드
    void EnableWave3SpawnersAndPatterns()
    {
        if (wave3Spawners) wave3Spawners.SetActive(true);
    }

    // 게임 클리어 시 실행되는 코루틴
    // 보스 사라지는 연출과 게임 클리어 UI 표시를 담당
    IEnumerator HandleGameClear()
    {
        // 모든 스포너와 공격 패턴 비활성화
        DisableAllSpawnersAndPatterns();

        // 웨이브 3의 모든 BombSpawner 컴포넌트를 비활성화
        foreach (var spawner in wave3Spawners.GetComponentsInChildren<BombSpawner>())
        {
            spawner.enabled = false;
        }

        // 남은 몬스터 공격 오브젝트들을 모두 제거
        GameObject[] monsterAttacks = GameObject.FindGameObjectsWithTag("MonsterAttack");
        foreach (GameObject obj in monsterAttacks)
        {
            Destroy(obj);
        }

        // 보스가 천천히 아래로 이동하며 사라지는 연출
        float duration = 1.8f; // 연출 지속 시간
        float elapsed = 0f;    // 경과 시간
        Vector3 startPos = bossMonster.transform.position;         // 시작 위치
        Vector3 endPos = startPos + Vector3.down * 0.56f;         // 최종 위치 (아래로 0.56 유닛)

        float shakeMagnitude = 0.05f; // 진동 효과의 세기

        // 설정된 시간 동안 보스를 이동시키며 진동 효과 적용
        while (elapsed < duration)
        {
            float t = elapsed / duration; // 0~1 사이의 보간 비율

            // 시작 위치에서 최종 위치로 선형 보간
            Vector3 lerpedPos = Vector3.Lerp(startPos, endPos, t);

            // 진동 효과를 위한 랜덤 오프셋 생성 (X, Y 방향)
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude),
                0f // Z축은 2D이므로 0으로 고정
            );

            // 최종 위치 = 보간된 위치 + 진동 오프셋
            bossMonster.transform.position = lerpedPos + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 보스를 완전히 비활성화하여 사라지게 함
        bossMonster.SetActive(false);

        // 게임 클리어 텍스트 UI 표시
        if (gameClearText != null)
        {
            gameClearText.SetActive(true);
        }

        Debug.Log("Game Clear 처리 완료!");
    }
}