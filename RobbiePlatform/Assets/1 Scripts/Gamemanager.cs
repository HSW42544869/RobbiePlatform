using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    #region ���
    static Gamemanager instance; // ÷���@���R�A�����

    SceneFader sceneFader;

    List<Orb> orbs;
    #endregion

    #region ��k
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        orbs = new List<Orb>();

        DontDestroyOnLoad(this);
    }
    /// <summary>
    /// �����e�������_�]
    /// </summary>
    public static void RegisterOrb(Orb orb)
    {
        // �p�G���������]�t�o�_�]
        if (!instance.orbs.Contains(orb))
        {
            instance.orbs.Add(orb);
        }
    }

    public static void RegisterSceneFader(SceneFader obj)
    {
        // ��e�� Gamemanager = obj
        instance.sceneFader = obj;
    }

    /// <summary>
    /// ���a���`�᭫�s���J
    /// </summary>
    public static void PlayerDied()
    {
        //�b���a���`���e����o�e���ĪG FadeOut �O RegisterSceneFader �X�ͪ�
        instance.sceneFader.FadeOut();
        instance.Invoke("RestScene", 1.5f);
    }

    /// <summary>
    /// ���s�[������
    /// </summary>
    void RestScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
#endregion
