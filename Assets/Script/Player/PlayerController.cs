using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum MoveMode { SideScroll = 1, TopDown = 2 }
    public MoveMode currentMode = MoveMode.SideScroll;

    [Header("Components")]
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float movePower = 2f;       // 기본 이동 속도
    [SerializeField] private float jumpPower = 2f;       // 점프 힘
    [SerializeField] private float doubleJumpPower = 1f; // 2단 점프 힘

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    private bool isGrounded;
    private int jumpCount = 0;          // 점프 횟수(2단 점프까지)

    [Header("Attack Settings")]
    [SerializeField] private float attackCooltime = 0.3f;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform firePoint;       // 발사 위치
    private float curAttackTime = 0f;

    public static bool isplayerDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        SetMoveMode(currentMode);
    }

    void Update()
    {
        if (isplayerDead) return;

        if (currentMode == MoveMode.SideScroll)
        {
            CheckGrounded();
            HandleSideScrollMovement();
            HandleSideScrollJump();
            HandleSideScrollAttack();
        }
        else
        {
            HandleTopDownMovement();
        }
    }

    void HandleSideScrollMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveX * movePower, rb.linearVelocity.y);
    }

    void HandleSideScrollJump()
    {
        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            float currentJumpPower = jumpCount == 0 ? jumpPower : doubleJumpPower;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * currentJumpPower, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    void HandleSideScrollAttack()
    {
        if (curAttackTime > 0)
        {
            curAttackTime -= Time.deltaTime;
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Z)) && curAttackTime <= 0)
        {
            if (attackPrefab != null && firePoint != null)
            {
                Instantiate(attackPrefab, firePoint.position, Quaternion.identity);
                curAttackTime = attackCooltime;
            }
        }
    }

    void HandleTopDownMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized * movePower;
        rb.linearVelocity = movement;
    }

    void CheckGrounded()
    {
        isGrounded = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void SetMoveMode(MoveMode mode)
    {
        currentMode = mode;

        // 중력 및 속도 설정
        rb.gravityScale = (mode == MoveMode.TopDown) ? 0f : 2f;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 점프 리셋
        jumpCount = 0;
    }
}
