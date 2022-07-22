using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManmager : MonoBehaviour
{
    static AudioManmager current;   //��e����

    [Header("�����n��")]
    public AudioClip ambientClip;
    public AudioClip musicClip;

    [Header("FV����")]
    public AudioClip deathFXClip;

    [Header("���a����")]
    public AudioClip[] walkStepClips;
    public AudioClip[] crouchStepClips;
    public AudioClip jumpClip;
    public AudioClip deathClip;

    public AudioClip jumpVoiceClip;
    [Header("���`�ɤH�����n��")]
    public AudioClip deathVoiceClip;    


    [Header("���ҭ���")] AudioSource ambientSource; 
    [Header("�I������")] AudioSource musicSource;
    [Header("�H�n")] AudioSource fxSource;
    [Header("�}�B�n")] AudioSource playerSource;
    AudioSource voiceSource;

    private void Awake()
    {
        current = this;

        // �Ϸ�e�o�Ӫ���L�k�Q�R��
        DontDestroyOnLoad(gameObject);

        // �Ыحӭӭ��Ī� AudioSource
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        playerSource = gameObject.AddComponent<AudioSource>();
        voiceSource = gameObject.AddComponent<AudioSource>();
        StartLeveAudio();
    }
    /// <summary>
    /// ���ҡB�I�����֭���
    /// </summary>
    void StartLeveAudio()
    {
        // ��e������(Source) = ���ҭ���
        current.ambientSource.clip = current.ambientClip;
        // �N�����Ī�Loop �אּ true
        current.ambientSource.loop = true;       
        // �N���ļ���X��
        current.ambientSource.Play();

        // ��e������(Source) = �I������
        current.musicSource.clip = current.musicClip;
        // �N�����Ī�Loop �אּ true
        current.musicSource.loop = true;
        // �N���ļ���X��
        current.musicSource.Play();
    }
    /// <summary>
    /// �}�B�n���K�[
    /// </summary>
    public static void PlayFootstepAudio()
    {
        //�]�t�������n�����ƥD�A�åH�üƬ����U��
        int index = Random.Range(0, current.walkStepClips.Length);

        //��e�������ķ|�ܦ��}�B���Ī��ü�
        current.playerSource.clip = current.walkStepClips[index];
        //���񭵮�
        current.playerSource.Play();
    }
    /// <summary>
    /// �U���n���K�[
    /// </summary>
    public static void PlayCrouchFootstepAudio()
    {
        //�]�t�������n�����ƥD�A�åH�üƬ����U��
        int index = Random.Range(0, current.crouchStepClips.Length);

        //��e�������ķ|�ܦ��}�B���Ī��ü�
        current.playerSource.clip = current.crouchStepClips[index];
        //���񭵮�
        current.playerSource.Play();
    }
    /// <summary>
    /// ���D�n��
    /// </summary>
    public static void PlayJumpAudio()
    {
        current.playerSource.clip = current.jumpClip;
        current.playerSource.Play();

        current.voiceSource.clip = current.jumpVoiceClip;
        current.playerSource.Play();
    }
    /// <summary>
    /// ���`����
    /// </summary>
    public static void PlayDeathAudio()
    {
        current.playerSource.clip = current.deathClip;
        current.playerSource.Play();

        current.voiceSource.clip = current.deathClip;
        current.voiceSource.Play();

        current.fxSource.clip = current.deathFXClip;
        current.fxSource.Play();
    }
}
