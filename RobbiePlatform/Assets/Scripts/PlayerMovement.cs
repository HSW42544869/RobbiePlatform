using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("移動參數")] public float speed = 8;     //移動數度
    public float crouchSpeedDivisor = 3f;   //下蹲時移動數度

    [Header("跳躍參數")]
    public float jumpForce = 6.3f;         //基本跳躍參數
    public float jumpHoldForce = 1.9f;     //長按跳躍加乘參數
    public float jumpHoldDuration = 0.1f;  //常按按鍵可按時間
    public float crouchJumpBoost = 5f;   //當下蹲時額外跳躍加乘
    public float hangingJumpForce = 15f;    //懸掛時的而外跳躍

    [Header("跳躍時間")]
    float jumpTime;


    [Header("狀態")]
    public bool isCrouch;       //下蹲狀態
    public bool isOnGround;     //在地面
    public bool isJump;         //在跳躍過程中
    public bool isHeadBlocked;  //判斷頭頂
    public bool isHanging;      //判斷是否懸掛


    [Header("環境檢測")]
    public float footOffset = 0.4f;     //射線寬度
    public float headClearance = 0.5f;  //頭頂檢測距離
    public float groundDistance = 0.2f; //檢測與地面之間距離
    float playerHeight;                 //頭頂位置
    public float eyeHeight = 1.5f;      //眼睛高度射線
    public float grabDistance = 0.4f;   //距離面前牆壁
    public float reachOffset = 0.7f;    //判斷頭頂上下是否有牆壁

    [Header("地面監測")]
    public LayerMask groundLayer;

    public float xVelocity;

    [Header("布林值狀態開關")]
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


        playerHeight = coll.size.y;             //(在一開始獲得玩家的高度)玩家高度 = 剛體高度
        colliderStandSize = coll.size;          //獲取站立的尺寸
        colliderStandOffset = coll.offset;      //獲取站立位置
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);        //設置下蹲剛體尺寸
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
    /// <summary>
    /// 環境物理判斷
    /// </summary>
    private void PhysicsChech()
    {
        //Vector2 pos = transform.position;                   //紀錄位置
        //Vector2 offset = new Vector2(-footOffset, 0f);      //左側偏移

        //RaycastHit2D leftCheck = Physics2D.Raycast(pos + offset, Vector2.down, groundDistance, groundLayer);    //(遊戲角色+偏移位置，方向向下，距離，圖層)
        //Debug.DrawRay(pos + offset, Vector2.down, Color.red, 0.2f);                                             //規劃射線
        //左右腳射線判斷
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);

        if (leftCheck || rightCheck)
            isOnGround = true;
        else isOnGround = false;

        //頭頂設限判斷
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);

        if (headCheck)
            isHeadBlocked = true;
        else isHeadBlocked = false;

        float direction = transform.localScale.x;       //左右懸掛射線方向(但float只是一個數值)
        Vector2 grabDir = new Vector2(direction, 0f);   //因為 direction 是 一個浮點型的數值 需要另外改一個2維的變量 (將數值改變成方向)[轉換為2為向量用法]

        // 頭頂的射線 起點 = Raycast(new Vector2(X = 玩家射線寬度 * 浮點型玩家X數值,Y = 玩家高度),玩家方向,玩家距離,判斷碰撞的牆體)
        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);
        // 眼睛的射線 牆壁確認 = Raycast(new Vector2(X = 玩家的射線高度 * 浮點型玩家X數值,Y = 玩家的眼睛高度),玩家方向,玩家距離,判斷碰撞的牆體)
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance, groundLayer);
        // 方向向下的射線 壁掛的檢測 = Raycast(new Vector2(X = 玩家的射線高度 * 浮點型玩家X數值,Y = 玩家的眼睛高度),向下的2為向量,玩家距離,判斷碰撞的牆體)
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);

        if (!isOnGround && rb.velocity.y < 0 && ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector3 pos = transform.position;             //重新定一玩家的數值  Vector3  = 現在的transform.position

            pos.x += (wallCheck.distance - 0.05f) * direction;    //新曾玩家懸掛時加上牆面的距離

            pos.y -= ledgeCheck.distance;                 //減去玩家懸掛時頭部高出的距離

            transform.position = pos;                     //將數值重新給回transform.position

            rb.bodyType = RigidbodyType2D.Static; //將玩家位置釘在牆上
            isHanging = true;                     //懸掛狀態 = true
        }
    }
    /// <summary>
    /// 懸掛
    /// </summary>
    void GroundMovement()
    {
        if (isHanging)  //懸掛時重複執行以免翻轉
            return;     //返回值  將會不斷進行懸掛

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
                //增加懸掛後跳躍上升的跳躍
                rb.AddForce(new Vector2(0f, crouchJumpBoost * 1.5f), ForceMode2D.Impulse);
            }

            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            AudioManmager.PlayJumpAudio();
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
            transform.localScale = new Vector3(-1, 1);  //玩家面相朝左
        else if (xVelocity > 0)
            transform.localScale = new Vector3(1, 1);   //玩家面相朝右
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
