using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("移動參數")]
    public float speed = 8;     //移動數度
    public float crouchSpeedDivisor = 3f;   //下蹲時移動數度

    [Header("跳躍參數")]
    private float jumpForce = 6.3f;         //基本跳躍參數
    private float jumpHoldForce = 1.9f;     //長按跳躍加乘參數
    private float jumpHoldDuration = 0.1f;  //常按按鍵可按時間
    private float crouchJumpBoost = 2.5f;   //當下蹲時額外跳躍加乘

    float jumpTime;


    [Header("狀態")]
    public bool isCrouch;   //下蹲狀態
    public bool isOnGround; //在地面
    public bool isJump;     //在跳躍過程中

    [Header("環境檢測")]
    public float footOffset = 0.4f;     //射線寬度
    public float headClearance = 0.5f;  //頭頂檢測距離
    public float groundDistance = 0.2f; //檢測與地面之間距離
    public LayerMask groundLayer;

    float xVelocity;
    //按鍵設置
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;

    //碰撞體尺寸 
    Vector2 colliderStandSize;      //站立的尺寸
    Vector2 colliderStandOffset;    //站立位置
    Vector2 colliderCrouchSize;     //下蹲的尺寸
    Vector2 colliderCrouchOffset;   //下蹲位置


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        colliderStandSize = coll.size;          //獲取站立的尺寸
        colliderStandOffset = coll.offset;      //獲取站立位置
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);    //設置下蹲剛體尺寸
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);  //設置下蹲位置
    }

    // Update is called once per frame
    void Update()
    {
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
    }
    private void FixedUpdate()
    {
        PhysicsChech();
        GroundMovement();
        MidAirMovement();
    }
    private void PhysicsChech()     //物理環境判斷
    {
        if (coll.IsTouchingLayers(groundLayer))
            isOnGround = true;
        else isOnGround = false;
    }
    void GroundMovement()
    {
        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch)    //自動起立
            StandUp();
        else if (!isOnGround && isCrouch)
            StandUp();

        xVelocity = Input.GetAxis("Horizontal");    //介於1 ~-1之間的數值:控制玩家左右鍵

        if (isCrouch)   //蹲下速度
            xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);

        FilpDirction();
    }

    void MidAirMovement()   //半空中判斷
    {
        if (jumpPressed && isOnGround && !isJump)
        {

            if (isCrouch & isOnGround)
            {
                StandUp();
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            }

            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        else if (isJump)
        {
            if (jumpHeld)
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            if (jumpTime < Time.time)
                isJump = false;
        }
    }

    void FilpDirction() //玩家面相以左右按鍵數值判斷
    {
        if (xVelocity < 0)
            transform.localScale = new Vector2(-1, 1);  //玩家面相朝左
        else
            transform.localScale = new Vector2(1, 1);   //玩家面相朝右
    }
    void Crouch()
    {
        isCrouch = true;
        //下蹲coll尺寸
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }
    void StandUp()
    {
        isCrouch = false;
        //站立coll尺寸
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffset;
    }
}
