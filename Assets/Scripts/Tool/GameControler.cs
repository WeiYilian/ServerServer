using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControler : MonoBehaviour
{
    private int box;

    public float Duration;
    
    private bool isFinalStage;

    public bool IsFinalStage
    {
        get => isFinalStage;
        set => isFinalStage = value;
    }

    public int Box
    {
        get => box;
        set => box = value;
    }

    private void Update()
    {
        if (Box < 3)
            GameFacade.Instance.PanelManager.MainPanel().Test.text = "任务进度：" + Box + "/3";

        if (Box >= 3 && !isFinalStage)
        {
            Invoke(nameof(FinalStage), 5);
        }
            
            
        if (isFinalStage)
        {
            Duration -= Time.deltaTime;
            GameFacade.Instance.PanelManager.MainPanel().Test.text = "剩余时间："+Mathf.FloorToInt(Duration);
                 
            
            
            if(Duration<=0)
            {
                EvenCenter.BroadCast(EventNum.GAMEOVER);
            }
        }
    }
    
    private void FinalStage()
    {
        isFinalStage = true;
    }
}
