using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    #region 欄位
    static Gamemanager instance; // 繩成一個靜態的實例

    SceneFader sceneFader;

    List<Orb> orbs;
    #endregion

    #region 方法
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
    /// 獲取當前場景的寶珠
    /// </summary>
    public static void RegisterOrb(Orb orb)
    {
        // 如果場景中部包含這寶珠
        if (!instance.orbs.Contains(orb))
        {
            instance.orbs.Add(orb);
        }
    }

    public static void RegisterSceneFader(SceneFader obj)
    {
        // 當前的 Gamemanager = obj
        instance.sceneFader = obj;
    }

    /// <summary>
    /// 玩家死亡後重新載入
    /// </summary>
    public static void PlayerDied()
    {
        //在玩家死亡之前撥放這畫面效果 FadeOut 是 RegisterSceneFader 出生的
        instance.sceneFader.FadeOut();
        instance.Invoke("RestScene", 1.5f);
    }

    /// <summary>
    /// 重新加載場景
    /// </summary>
    void RestScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
#endregion
