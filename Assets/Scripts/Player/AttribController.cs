using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class AttribController
{
    private PlayerConctroller playerConctroller;
    private CharacterStats characterStats;
    
    private Text userName;

    private Text levelText;
    
    private Image hp;
    private Image exp;
    private Text hpText;
    private Text expText;



    public AttribController()
    {
        playerConctroller = PlayerConctroller.Instance;
        characterStats = playerConctroller.CharacterStats;
    }

    public void Init()
    {
        userName = playerConctroller.MainPanel.userName;
        levelText = playerConctroller.MainPanel.levelText;
        hp = playerConctroller.MainPanel.hp;
        exp = playerConctroller.MainPanel.exp; 
        hpText = playerConctroller.MainPanel.hpText;
        expText = playerConctroller.MainPanel.expText;
    }


    // ReSharper disable Unity.PerformanceAnalysis
    public void SuperviserNumber()
    {
        //userName.text = playerConctroller.PlayerAttrib[1];
        levelText.text = "LV." + characterStats.CurrentLevel;
        hp.fillAmount = characterStats.CurrentHealth / characterStats.MaxHealth;
        exp.fillAmount = characterStats.CurrentExp / characterStats.BaseExp;
        hpText.text = characterStats.CurrentHealth + "/" + characterStats.MaxHealth;
        expText.text = characterStats.CurrentExp + "/" + characterStats.BaseExp;
    }
}
