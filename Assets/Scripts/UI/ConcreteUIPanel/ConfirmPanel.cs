using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : BasePanel
{
    private static readonly string path = "UI/Panel/ConfirmPanel";

    private string message;
    public ConfirmPanel(string message) : base(new UIType(path))
    {
        this.message = message;
    }

    public override void OnEnter()
    {
        GameObject.Find("Canvas/ConfirmPanel/Text").GetComponent<Text>().text = message;
        
        UITool.GetOrAddComponentInChildren<Button>("BtnExit").onClick.AddListener(()=>
        {
            AudioManager.Instance.PlayButtonAudio();
            Pop();
        });
        
        UITool.GetOrAddComponentInChildren<Button>("Confirm").onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButtonAudio();
            Pop();
        });
    }
}
