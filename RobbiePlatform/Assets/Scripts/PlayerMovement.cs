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
    public float jumpForce = 6.3f;         //基本跳躍參數
    public float jumpHoldForce = 1.9f;     //長按跳躍加乘參數
    public float jumpHoldDuration = 0.1f;  //常按按鍵可按時間
    public float crouchJumpBoost = 2.5f;   //當下蹲時額外跳躍加乘
    public float hangingJumpForce = 15f;    //懸掛時的而外跳躍


    float jumpTime;


    [Header("狀態")]
    public bool isCrouch;   //下蹲狀態
    public bool isOnGround; //在地面
    public bool isJump;     //在跳躍過程中
    public bool isHeadBlocked;  //判斷頭頂
    public bool isHanging;  //判斷是否懸掛


    [Header("環境檢測")]
    public float footOffset = 0.4f;     //射線寬度
    public float headClearance = 0.5f;  //頭頂檢測距離
    public float groundDistance = 0.2f; //檢測與地面之間距離
    float playerHeight;                 //頭頂位置
    public float eyeHeight = 1.5f;      //眼睛高度射線
    public float grabDistance = 0.4f;   //距離面前牆壁
    public float reachOffset = 0.7f;

    public LayerMask groundLayer;

    float xVelocity;
    //按鍵設置
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;
    bool crouchPressed;

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


        playerHeight = coll.size.y; //玩家高度 = 剛體高度
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
        crouchPressed = Input.GetButtonDown("Crouch");

    }
    private void FixedUpdate()
    {
        PhysicsChech();
        GroundMovement();
        MidAirMovement();
    }
    private void PhysicsChech()     //物理環境判斷
    {
        /*Vector2 pos = transform.position;                   //紀錄位置
        Vector2 offset = new Vector2(-footOffset, 0f);      //左側偏移
        
        RaycastHit2D leftCheck = Physics2D.Raycast(pos + offset, Vector2.down, groundDistance, groundLayer);    //(遊戲角色+偏移位置，方向向下，距離，圖層)
        Debug.DrawRay(pos + offset, Vector2.down, Color.red, 0.2f); //規劃射線*/

        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);

        if (leftCheck || rightCheck)
            isOnGround = true;
        else isOnGround = false;

        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);

        if (headCheck)
            isHeadBlocked = true;
        else isHeadBlocked = false;

        float direction = transform.localScale.x;       //左右懸掛射線方向(但float只是一個數值)
        Vector2 grabDir = new Vector2(direction, 0f);   //將數值改變成方向

        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, groundDistance * 3, groundLayer);
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);

        if (!isOnGround && rb.velocity.y < 0f && ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector3 pos = transform.position;   //定義一個位置

            pos.x += (wallCheck.distance - 0.05f) * direction;    //新曾玩家懸掛時加上牆面的距離

            pos.y -= ledgeCheck.distance;           //減去玩家懸掛時頭部高出的距離

            transform.position = pos;

            rb.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }
    }
    void GroundMovement()
    {
        if (isHanging)  //懸掛時重複執行以免翻轉
            return;

        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch && !isHeadBlocked)    //自動起立
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
        if (isHanging)
        {
            if (jumpPressed)    //懸掛後恢復原回到地面(且按下單次跳躍)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = new Vector2(rb.velocity.x, hangingJumpForce);
                isHanging = false;
            }
            if (crouchPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;

                isHanging = false;
            }
        }

        if (jumpPressed && isOnGround && !isJump && !isHeadBlocked)
        {

            if (isCrouch)
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
        else if (xVelocity > 0)
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
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)       //(位移，方向、浮點型的變量、圖層)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green;

        Debug.DrawRay(pos + offset, rayDiraction * length, color);

        return hit;
    }


}
