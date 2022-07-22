using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("���ʰѼ�")] public float speed = 8;     //���ʼƫ�
    public float crouchSpeedDivisor = 3f;   //�U�ۮɲ��ʼƫ�

    [Header("���D�Ѽ�")]
    public float jumpForce = 6.3f;         //�򥻸��D�Ѽ�
    public float jumpHoldForce = 1.9f;     //�������D�[���Ѽ�
    public float jumpHoldDuration = 0.1f;  //�`������i���ɶ�
    public float crouchJumpBoost = 5f;   //��U�ۮ��B�~���D�[��
    public float hangingJumpForce = 15f;    //�a���ɪ��ӥ~���D

    [Header("���D�ɶ�")]
    float jumpTime;


    [Header("���A")]
    public bool isCrouch;       //�U�۪��A
    public bool isOnGround;     //�b�a��
    public bool isJump;         //�b���D�L�{��
    public bool isHeadBlocked;  //�P�_�Y��
    public bool isHanging;      //�P�_�O�_�a��


    [Header("�����˴�")]
    public float footOffset = 0.4f;     //�g�u�e��
    public float headClearance = 0.5f;  //�Y���˴��Z��
    public float groundDistance = 0.2f; //�˴��P�a�������Z��
    float playerHeight;                 //�Y����m
    public float eyeHeight = 1.5f;      //�������׮g�u
    public float grabDistance = 0.4f;   //�Z�����e���
    public float reachOffset = 0.7f;    //�P�_�Y���W�U�O�_�����

    [Header("�a���ʴ�")]
    public LayerMask groundLayer;

    public float xVelocity;

    [Header("���L�Ȫ��A�}��")]
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;
    bool crouchPressed;

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


        playerHeight = coll.size.y;             //(�b�@�}�l��o���a������)���a���� = ���鰪��
        colliderStandSize = coll.size;          //������ߪ��ؤo
        colliderStandOffset = coll.offset;      //������ߦ�m
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);        //�]�m�U�ۭ���ؤo
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);  //�]�m�U�ۦ�m
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
    /// ���Ҫ��z�P�_
    /// </summary>
    private void PhysicsChech()
    {
        //Vector2 pos = transform.position;                   //������m
        //Vector2 offset = new Vector2(-footOffset, 0f);      //��������

        //RaycastHit2D leftCheck = Physics2D.Raycast(pos + offset, Vector2.down, groundDistance, groundLayer);    //(�C������+������m�A��V�V�U�A�Z���A�ϼh)
        //Debug.DrawRay(pos + offset, Vector2.down, Color.red, 0.2f);                                             //�W���g�u
        //���k�}�g�u�P�_
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);

        if (leftCheck || rightCheck)
            isOnGround = true;
        else isOnGround = false;

        //�Y���]���P�_
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);

        if (headCheck)
            isHeadBlocked = true;
        else isHeadBlocked = false;

        float direction = transform.localScale.x;       //���k�a���g�u��V(��float�u�O�@�Ӽƭ�)
        Vector2 grabDir = new Vector2(direction, 0f);   //�]�� direction �O �@�ӯB�I�����ƭ� �ݭn�t�~��@��2�����ܶq (�N�ƭȧ��ܦ���V)[�ഫ��2���V�q�Ϊk]

        // �Y�����g�u �_�I = Raycast(new Vector2(X = ���a�g�u�e�� * �B�I�����aX�ƭ�,Y = ���a����),���a��V,���a�Z��,�P�_�I��������)
        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);
        // �������g�u ����T�{ = Raycast(new Vector2(X = ���a���g�u���� * �B�I�����aX�ƭ�,Y = ���a����������),���a��V,���a�Z��,�P�_�I��������)
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance, groundLayer);
        // ��V�V�U���g�u �������˴� = Raycast(new Vector2(X = ���a���g�u���� * �B�I�����aX�ƭ�,Y = ���a����������),�V�U��2���V�q,���a�Z��,�P�_�I��������)
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);

        if (!isOnGround && rb.velocity.y < 0 && ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector3 pos = transform.position;             //���s�w�@���a���ƭ�  Vector3  = �{�b��transform.position

            pos.x += (wallCheck.distance - 0.05f) * direction;    //�s�����a�a���ɥ[�W�𭱪��Z��

            pos.y -= ledgeCheck.distance;                 //��h���a�a�����Y�����X���Z��

            transform.position = pos;                     //�N�ƭȭ��s���^transform.position

            rb.bodyType = RigidbodyType2D.Static; //�N���a��m�v�b��W
            isHanging = true;                     //�a�����A = true
        }
    }
    /// <summary>
    /// �a��
    /// </summary>
    void GroundMovement()
    {
        if (isHanging)  //�a���ɭ��ư���H�K½��
            return;     //��^��  �N�|���_�i���a��

        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch && !isHeadBlocked)    //�۰ʰ_��
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
        if (isHanging)
        {
            if (jumpPressed)    //�a�����_��^��a��(�B���U�榸���D)
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
                //�W�[�a������D�W�ɪ����D
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

    void FilpDirction() //���a���ۥH���k����ƭȧP�_
    {
        if (xVelocity < 0)
            transform.localScale = new Vector3(-1, 1);  //���a���۴¥�
        else if (xVelocity > 0)
            transform.localScale = new Vector3(1, 1);   //���a���۴¥k
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
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)       //(�첾�A��V�B�B�I�����ܶq�B�ϼh)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green;

        Debug.DrawRay(pos + offset, rayDiraction * length, color);

        return hit;
    }


}
