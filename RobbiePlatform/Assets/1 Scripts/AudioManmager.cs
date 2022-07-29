using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManmager : MonoBehaviour
{
    static AudioManmager current;   //當前音效

    [Header("環境聲音")]
    public AudioClip ambientClip;
    public AudioClip musicClip;

    [Header("FX音效")]
    public AudioClip deathFXClip;
    [Header("獲得寶珠的音效")]public AudioClip orbFXClip;

    [Header("玩家音效")]
    public AudioClip[] walkStepClips;
    public AudioClip[] crouchStepClips;
    [Header("跳躍")]public AudioClip jumpClip;
    [Header("死亡")] public AudioClip deathClip;

    [Header("跳躍聲音")]public AudioClip jumpVoiceClip;
    [Header("死亡時人物的聲音")] public AudioClip deathVoiceClip;
    [Header("寶珠收起來的音效")] public AudioClip orbVoiceClip;


    [Header("環境音效")] AudioSource ambientSource; 
    [Header("背景音樂")] AudioSource musicSource;
    [Header("人聲")] AudioSource fxSource;
    [Header("腳步聲")] AudioSource playerSource;
    AudioSource voiceSource;

    private void Awake()
    {
        // 如果場上current != 0
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }
        current = this;

        // 使當前這個物件無法被刪除
        DontDestroyOnLoad(gameObject);

        // 創建個個音效的 AudioSource
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        playerSource = gameObject.AddComponent<AudioSource>();
        voiceSource = gameObject.AddComponent<AudioSource>();
        StartLeveAudio();
    }
    /// <summary>
    /// 環境、背景音樂音效
    /// </summary>
    void StartLeveAudio()
    {
        // 當前的音效(Source) = 環境音效
        current.ambientSource.clip = current.ambientClip;
        // 將此音效的Loop 改為 true
        current.ambientSource.loop = true;       
        // 將音效播放出來
        current.ambientSource.Play();

        // 當前的音效(Source) = 背景音樂
        current.musicSource.clip = current.musicClip;
        // 將此音效的Loop 改為 true
        current.musicSource.loop = true;
        // 將音效播放出來
        current.musicSource.Play();
    }
    /// <summary>
    /// 腳步聲音添加
    /// </summary>
    public static void PlayFootstepAudio()
    {
        //包含的關於聲音的數主，並以亂數紀錄下來
        int index = Random.Range(0, current.walkStepClips.Length);

        //當前場中音效會變成腳步音效的亂數
        current.playerSource.clip = current.walkStepClips[index];
        //播放音效
        current.playerSource.Play();
    }
    /// <summary>
    /// 下蹲聲音添加
    /// </summary>
    public static void PlayCrouchFootstepAudio()
    {
        //包含的關於聲音的數主，並以亂數紀錄下來
        int index = Random.Range(0, current.crouchStepClips.Length);

        //當前場中音效會變成腳步音效的亂數
        current.playerSource.clip = current.crouchStepClips[index];
        //播放音效
        current.playerSource.Play();
    }
    /// <summary>
    /// 跳躍聲音
    /// </summary>
    public static void PlayJumpAudio()
    {
        current.playerSource.clip = current.jumpClip;
        current.playerSource.Play();

        current.voiceSource.clip = current.jumpVoiceClip;
        current.playerSource.Play();
    }
    /// <summary>
    /// 死亡音效
    /// </summary>
    public static void PlayDeathAudio()
    {
        // 玩家死亡遊戲音效
        current.playerSource.clip = current.deathClip;
        current.playerSource.Play();
        // 玩家死亡的叫聲
        current.voiceSource.clip = current.deathVoiceClip;
        current.voiceSource.Play();
        // 收集的寶珠在死亡後散落的聲音
        current.fxSource.clip = current.deathFXClip;
        current.fxSource.Play();
    }
    /// <summary>
    /// 寶珠獲取音效
    /// </summary>
    public static void PlayOrbAudio()
    {
        current.fxSource.clip = current.orbFXClip;
        current.fxSource.Play();

        current.voiceSource.clip = current.orbVoiceClip;
        current.voiceSource.Play();
    }
}
