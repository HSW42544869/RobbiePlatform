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
        // 獲取父級的的PlayerMovement
        playerMovement = GetComponentInParent<PlayerMovement>();
        //另一種使用ID方式抓取方法
        groundID = Animator.StringToHash("isOnGround");
        hangingID = Animator.StringToHash("isHanging");
        crouchID = Animator.StringToHash("isCrouching");
        speedID = Animator.StringToHash("speed");
        fallID = Animator.StringToHash("verticalVelocity");
    }

    // Update is called once per frame
    void Update()
    {
        //獲取在PlayerMovement中的橫向移動數值
        ani.SetFloat(speedID, Mathf.Abs(playerMovement.xVelocity));

        //將PlayerMovement中的Bool值傳遞進來
        //ani.SetBool("isOnGround", playerMovement.isOnGround);

        //使用ID方式將PlayerMovement中的Bool值傳遞進來
        ani.SetBool(groundID, playerMovement.isOnGround);
        ani.SetBool(hangingID, playerMovement.isHanging);
        ani.SetBool(crouchID, playerMovement.isCrouch);
        ani.SetFloat(fallID, rig.velocity.y);
    }
    /// <summary>
    /// 抓取並播放玩家移動聲音
    /// </summary>
    public void StepAudio()
    {
        AudioManmager.PlayFootstepAudio();
    }
    /// <summary>
    /// 抓取並撥放玩家下蹲的聲音
    /// </summary>
    public void CrouchSetAudio()
    {
        AudioManmager.PlayCrouchFootstepAudio();
    }

}
