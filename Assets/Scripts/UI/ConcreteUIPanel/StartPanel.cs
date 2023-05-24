using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开始主面板
/// </summary>
public class StartPanel : BasePanel
{
    private static readonly string path = "UI/Panel/StartPanel"; /*readonly：声明字段，表示声明的字段只能在声明时或同一个类的构造函数当中进行赋值*/

    public StartPanel() : base(new UIType(path)) { }

    private bool isMute; 
    
    public override void OnEnter()
    {

        UITool.GetOrAddComponentInChildren<Button>("BtnVoice").onClick.AddListener(() =>
        {
            isMute = !isMute;
            if (!isMute)
                GameObject.Find("StartPanel").transform.Find("BtnVoice").GetComponent<Image>().sprite =
                    GameFacade.Instance.LoadSprite("audio_on");
            else
                GameObject.Find("StartPanel").transform.Find("BtnVoice").GetComponent<Image>().sprite =
                    GameFacade.Instance.LoadSprite("audio_mute");
        });
        
        UITool.GetOrAddComponentInChildren<Button>("BtnPlay").onClick.AddListener(() =>
        {
            //打开登录面板
            //TODO:Push(new LoginPanel());
        });
        UITool.GetOrAddComponentInChildren<Button>("BtnUserManager").onClick.AddListener(() =>
        {
            //打开账号管理面板
            //Push(new UserManagerPanel());
        });
        
        UITool.GetOrAddComponentInChildren<Button>("ExitGame").onClick.AddListener(() =>
       {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//在编辑器中退出
        #else
            Application.Quit();//在打包之后的退出游戏
        #endif
       }); 
    }
}