using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("���ʰѼ�")]
    public float speed = 8;     //���ʼƫ�
    public float crouchSpeedDivisor = 3f;   //�U�ۮɲ��ʼƫ�

    [Header("���D�Ѽ�")]
    private float jumpForce = 6.3f;         //�򥻸��D�Ѽ�
    private float jumpHoldForce = 1.9f;     //�������D�[���Ѽ�
    private float jumpHoldDuration = 0.1f;  //�`������i���ɶ�
    private float crouchJumpBoost = 2.5f;   //��U�ۮ��B�~���D�[��

    float jumpTime;


    [Header("���A")]
    public bool isCrouch;   //�U�۪��A
    public bool isOnGround; //�b�a��
    public bool isJump;     //�b���D�L�{��

    [Header("�����˴�")]
    public float footOffset = 0.4f;     //�g�u�e��
    public float headClearance = 0.5f;  //�Y���˴��Z��
    public float groundDistance = 0.2f; //�˴��P�a�������Z��
    public LayerMask groundLayer;

    float xVelocity;
    //����]�m
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;

    //�I����ؤo 
    Vector2 colliderStandSize;      //���ߪ��ؤo
    Vector2 colliderStandOffset;    //���ߦ�m
    Vector2 colliderCrouchSize;     //�U�۪��ؤo
    Vector2 colliderCrouchOffset;   //�U�ۦ�m


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        colliderStandSize = coll.size;          //������ߪ��ؤo
        colliderStandOffset = coll.offset;      //������ߦ�m
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);    //�]�m�U�ۭ���ؤo
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);  //�]�m�U�ۦ�m
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
    private void PhysicsChech()     //���z���ҧP�_
    {
        if (coll.IsTouchingLayers(groundLayer))
            isOnGround = true;
        else isOnGround = false;
    }
    void GroundMovement()
    {
        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch)    //�۰ʰ_��
            StandUp();
        else if (!isOnGround && isCrouch)
            StandUp();

        xVelocity = Input.GetAxis("Horizontal");    //����1 ~-1�������ƭ�:����a���k��

        if (isCrouch)   //�ۤU�t��
            xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);

        FilpDirction();
    }

    void MidAirMovement()   //�b�Ť��P�_
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

    void FilpDirction() //���a���ۥH���k����ƭȧP�_
    {
        if (xVelocity < 0)
            transform.localScale = new Vector2(-1, 1);  //���a���۴¥�
        else
            transform.localScale = new Vector2(1, 1);   //���a���۴¥k
    }
    void Crouch()
    {
        isCrouch = true;
        //�U��coll�ؤo
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }
    void StandUp()
    {
        isCrouch = false;
        //����coll�ؤo
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffset;
    }
}
