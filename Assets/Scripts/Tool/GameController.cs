using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    private int box;

    public float Duration;
    
    private bool isFinalStage;

    private bool firstOver;

    private bool gameStart;

    [HideInInspector]public bool TwoStart;

    public GameObject EndPoint;
    
    public GameObject Guidelines;
    
    private Image HP1;
    
    private Image HP2;

    public CharacterStats boss;

    public GameObject MainCamera;

    private GameObject MainPanel;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    public bool IsFinalStage
    {
        get => isFinalStage;
        private set => isFinalStage = value;
    }

    public int Box
    {
        get => box;
        set => box = value;
    }

    public bool GameStart
    {
        get => gameStart;
        set => gameStart = value;
    }

    public bool FirstOver
    {
        get => firstOver;
        private set => firstOver = value;
    }

    private IEnumerator Start()
    {
        yield return null;
        HP1 = GameFacade.Instance.PanelManager.MainPanel().BossHealth.transform.GetChild(0).GetComponent<Image>();
        BoxAdd();
        MainPanel = GameFacade.Instance.PanelManager.MainPanel().mainPanel;
        MainPanel.SetActive(false);
    }

    private void Update()
    {
        if (MainCamera.activeSelf && MainPanel && !gameStart)
        {
            MainPanel.SetActive(true);
            GameFacade.Instance.PanelManager.Push(new GuidePanel());
            gameStart = true;
        }
        if(!MainCamera.activeSelf || !MainPanel) return;
        
        if (GameLoop.Instance.isTimeOut) return;
        
        if (TwoStart)
        {
            HP1.fillAmount = boss.CurrentHealth / boss.MaxHealth;
        }

        if (isFinalStage && !firstOver)
        {
            Duration -= Time.deltaTime;
            GameFacade.Instance.PanelManager.MainPanel().Test.text = "剩余时间："+Mathf.FloorToInt(Duration);
            GameFacade.Instance.PanelManager.MainPanel().Test.color = Color.red;
        }
        
        if(Duration<=0 && !firstOver)
        {
            firstOver = true;
            GameFacade.Instance.PanelManager.MainPanel().Test.text = " ";
            GameFacade.Instance.PanelManager.Push(new NoticePanel("第一阶段的任务已完成\n请寻找传送阵进入下一阶段"));
            EndPoint.SetActive(true);
            Guidelines.SetActive(true);
        }
    }

    public void BoxAdd()
    {
        GameFacade.Instance.PanelManager.MainPanel().Test.text = "已收集物资：" + box + "/3";
        
        if (box >= 3 && !isFinalStage)
        {
            GameFacade.Instance.PanelManager.MainPanel().Test.text = "已收集物资：" + box + "/3";
            GameFacade.Instance.PanelManager.Push(new ConfirmPanel("狂暴模式已开启!\n请坚持3分钟"));
            isFinalStage = true;
            
        }
    }
}
