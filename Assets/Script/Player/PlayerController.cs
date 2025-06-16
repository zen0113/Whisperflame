using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 이동 모드 열거형 정의: 횡스크롤(SideScroll) 또는 탑다운(TopDown)
    public enum MoveMode { SideScroll = 1, TopDown = 2 }
    public MoveMode currentMode = MoveMode.SideScroll;

    [Header("Components")]
    private Rigidbody2D rb;  // Rigidbody2D 컴포넌트 참조

    [Header("Movement Settings")]
    [SerializeField] private float movePower = 2f;       // 이동 속도
    [SerializeField] private float jumpPower = 2f;       // 점프 힘 (1단 점프)
    [SerializeField] private float doubleJumpPower = 1f; // 2단 점프 힘

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;      // 바닥 판별을 위한 레이어
    [SerializeField] private Transform groundCheck;      // 바닥 체크 위치
    [SerializeField] private float groundCheckRadius = 0.1f; // 바닥 체크 반경
    private bool isGrounded;       // 바닥에 닿았는지 여부
    private int jumpCount = 0;     // 점프 횟수 (최대 2회)

    [Header("Attack Settings")]
    [SerializeField] private float attackCooltime = 0.3f;        // 공격 쿨타임
    [SerializeField] private GameObject attackPrefab;            // 공격 프리팹
    [SerializeField] private Transform firePoint;                // 공격 발사 위치
    private float curAttackTime = 0f;                            // 현재 공격 쿨타임 타이머

    public static bool isplayerDead = false; // 플레이어 사망 여부

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 캐싱
    }

    void Start()
    {
        SetMoveMode(currentMode); // 현재 모드에 맞게 초기 설정
    }

    void Update()
    {
        if (isplayerDead) return; // 사망 시 조작 불가

        if (currentMode == MoveMode.SideScroll)
        {
            CheckGrounded();               // 바닥 상태 확인
            HandleSideScrollMovement();   // 좌우 이동 처리
            HandleSideScrollJump();       // 점프 처리
            HandleSideScrollAttack();     // 공격 처리
        }
        else
        {
            HandleTopDownMovement();      // 탑다운 방식 이동 처리
        }
    }

    // 횡스크롤 방식 이동 처리
    void HandleSideScrollMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveX * movePower, rb.linearVelocity.y);
    }

    // 횡스크롤 방식 점프 처리 (2단 점프 포함)
    void HandleSideScrollJump()
    {
        if (isGrounded)
        {
            jumpCount = 0; // 착지 시 점프 카운트 초기화
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            float currentJumpPower = (jumpCount == 0) ? jumpPower : doubleJumpPower;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // 수직 속도 초기화
            rb.AddForce(Vector2.up * currentJumpPower, ForceMode2D.Impulse);
            jumpCount++; // 점프 카운트 증가
        }
    }

    // 횡스크롤 방식 공격 처리
    void HandleSideScrollAttack()
    {
        if (curAttackTime > 0)
        {
            curAttackTime -= Time.deltaTime; // 쿨타임 감소
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Z)) && curAttackTime <= 0)
        {
            if (attackPrefab != null && firePoint != null)
            {
                Instantiate(attackPrefab, firePoint.position, Quaternion.identity); // 공격 프리팹 생성
                curAttackTime = attackCooltime; // 쿨타임 초기화
            }
        }
    }

    // 탑다운 방식 이동 처리
    void HandleTopDownMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized * movePower;
        rb.linearVelocity = movement;
    }

    // 바닥에 닿았는지 검사 (OverlapCircle 사용)
    void CheckGrounded()
    {
        isGrounded = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // 이동 모드 설정 (중력/속도/점프 초기화 포함)
    public void SetMoveMode(MoveMode mode)
    {
        currentMode = mode;

        // 중력 설정: 탑다운이면 중력 제거
        rb.gravityScale = (mode == MoveMode.TopDown) ? 0f : 2f;

        // 이동 및 회전 초기화
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 점프 횟수 초기화
        jumpCount = 0;
    }
}
