using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    private int box;

    public float Duration;
    
    private bool isFinalStage;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

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
        if (GameLoop.Instance.isTimeOut) return;
        
        if (isFinalStage)
        {
            Duration -= Time.deltaTime;
            GameFacade.Instance.PanelManager.MainPanel().Test.text = "剩余时间："+Mathf.FloorToInt(Duration);
            GameFacade.Instance.PanelManager.MainPanel().Test.color = Color.red;
            
            
            if(Duration<=0)
            {
                EvenCenter.BroadCast(EventNum.GAMEOVER);
            }
        }
        
        if (Box < 3)
            GameFacade.Instance.PanelManager.MainPanel().Test.text = "任务进度：" + Box + "/3";

        if (Box >= 3 && !isFinalStage)
        {
            GameFacade.Instance.PanelManager.MainPanel().Test.text = "任务进度：" + Box + "/3";
            GameFacade.Instance.PanelManager.Push(new ConfirmPanel("狂暴模式已开启!\n请坚持3分钟"));
            isFinalStage = true;
        }
    }
}
