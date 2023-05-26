using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverPanel : BasePanel
{
    private static readonly string path = "UI/Panel/OverPanel";

    private string message;

    private bool isGameOver;

    public OverPanel(string message,bool GameOver = false) : base(new UIType(path))
    {
        this.message = message;
        isGameOver = GameOver;
    }

    public override void OnEnter()
    {
        if(isGameOver) UITool.GetOrAddComponentInChildren<Button>("Next").gameObject.SetActive(false);
        
        GameObject.Find("Canvas").transform.Find("OverPanel/Text").GetComponent<Text>().text = message;

        if (!isGameOver)
        {
             UITool.GetOrAddComponentInChildren<Button>("Next").onClick.AddListener(() =>
            {
                SceneStateController.Instance.SetState(new Game2Scene());
            });
        }
        
        UITool.GetOrAddComponentInChildren<Button>("Resume").onClick.AddListener(() =>
        {
            SceneStateController.Instance.SetState(new StartScene());
        });
        
        UITool.GetOrAddComponentInChildren<Button>("Quit").onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//在编辑器中退出
    #else
           Application.Quit();//在打包之后的退出游戏
    #endif
        });
    }
}
