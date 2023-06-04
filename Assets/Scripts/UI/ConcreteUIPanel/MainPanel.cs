using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    private static readonly string path = "UI/Panel/MainPanel";
    
    public MainPanel():base(new UIType(path)){}

    public GameObject mainPanel;
    
    public Text userName;

    public Text hpText;
    public Text expText;
    public Image hp;
    public Image exp;

    public GameObject BossHealth;

    public Text levelText;

    public Text Test;

    public Image skill1;
    public Image skill2;
    public Image skill3;

    private bool openBag;

    public override void OnEnter()
    {
        Init();
        EvenCenter.AddListener(EventNum.GAMEOVER,ShowEndPanel);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnUpdata()
    {
        //加其他模块
        if (Input.GetKeyDown(KeyCode.B))
        {
            Push(new CharacterPanel());
        }
        if(Input.GetKeyDown(KeyCode.Return))
            Push(new ChatPanel());
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Push(new SetPanel());
        }
    }

    public override void OnPause()
    {
        base.OnPause();
        GameLoop.Instance.isTimeOut = true;
    }

    public override void OnResume()
    {
        base.OnResume();
        GameLoop.Instance.isTimeOut = false;
    }

    public override void OnExit()
    {
        EvenCenter.RemoveListener(EventNum.GAMEOVER,ShowEndPanel);
    }

    public void Init()
    {
        mainPanel = GameObject.Find("Canvas/MainPanel");
        userName = mainPanel.transform.Find("Header/Left/UserName").GetComponent<Text>();
        levelText = mainPanel.transform.Find("Header/Left/LV").GetComponent<Text>();
        hp = mainPanel.transform.Find("Header/Left/Bg/HP").GetComponent<Image>();
        exp = mainPanel.transform.Find("Header/Left/Bg/Exp").GetComponent<Image>();
        hpText = mainPanel.transform.Find("Header/Left/Bg/HP/Text").GetComponent<Text>();
        expText = mainPanel.transform.Find("Header/Left/Bg/Exp/Text").GetComponent<Text>();
        Test = mainPanel.transform.Find("Header/Right/Text").GetComponent<Text>();
        skill1 = mainPanel.transform.Find("Bottom/SkillPanel/skillOne/Image/skill1").GetComponent<Image>();
        skill2 = mainPanel.transform.Find("Bottom/SkillPanel/skillTwo/Image/skill2").GetComponent<Image>();
        skill3 = mainPanel.transform.Find("Bottom/SkillPanel/skillThree/Image/skill3").GetComponent<Image>();
        BossHealth = mainPanel.transform.Find("Content/HealthBar").gameObject;
        GameLoop.Instance.isTimeOut = false;
    }

    private void ShowEndPanel()
    {
        if(PlayerConctroller.Instance.CharacterStats.CurrentHealth <= 0)
        {
            if (!GameObject.Find("Canvas/OverPanel"))
                GameFacade.Instance.PanelManager.Push(new OverPanel("游戏失败",true));
            AudioManager.Instance.PlayAudio(4,"失败音效");
        }
        else
        {
            if (!GameObject.Find("Canvas/OverPanel"))
                GameFacade.Instance.PanelManager.Push(new OverPanel("游戏通关"));
            AudioManager.Instance.PlayAudio(4,"胜利音效");
        }
    }
}
