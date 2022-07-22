using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmimation : MonoBehaviour
{
    PlayerMovement playerMovement;
    Animator ani;
    Rigidbody2D rig;

    int groundID;
    int hangingID;
    int crouchID;
    int speedID;
    int fallID;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        rig = GetComponentInParent<Rigidbody2D>();
        // ������Ū���PlayerMovement
        playerMovement = GetComponentInParent<PlayerMovement>();
        //�t�@�بϥ�ID�覡�����k
        groundID = Animator.StringToHash("isOnGround");
        hangingID = Animator.StringToHash("isHanging");
        crouchID = Animator.StringToHash("isCrouching");
        speedID = Animator.StringToHash("speed");
        fallID = Animator.StringToHash("verticalVelocity");
    }

    // Update is called once per frame
    void Update()
    {
        //����bPlayerMovement������V���ʼƭ�
        ani.SetFloat(speedID, Mathf.Abs(playerMovement.xVelocity));

        //�NPlayerMovement����Bool�ȶǻ��i��
        //ani.SetBool("isOnGround", playerMovement.isOnGround);

        //�ϥ�ID�覡�NPlayerMovement����Bool�ȶǻ��i��
        ani.SetBool(groundID, playerMovement.isOnGround);
        ani.SetBool(hangingID, playerMovement.isHanging);
        ani.SetBool(crouchID, playerMovement.isCrouch);
        ani.SetFloat(fallID, rig.velocity.y);
    }
    /// <summary>
    /// ����ü��񪱮a�����n��
    /// </summary>
    public void StepAudio()
    {
        AudioManmager.PlayFootstepAudio();
    }
    /// <summary>
    /// ����ü��񪱮a�U�۪��n��
    /// </summary>
    public void CrouchSetAudio()
    {
        AudioManmager.PlayCrouchFootstepAudio();
    }

}
