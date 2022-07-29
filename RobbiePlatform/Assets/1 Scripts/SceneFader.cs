using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : MonoBehaviour
{

    Animator ani;

    int faderID;

    private void Start()
    {

        ani = GetComponent<Animator>();

        faderID = Animator.StringToHash("Fade");
        //使用這個方法
        Gamemanager.RegisterSceneFader(this);
    }

    public void FadeOut()
    {
        ani.SetTrigger(faderID);
    }
}
