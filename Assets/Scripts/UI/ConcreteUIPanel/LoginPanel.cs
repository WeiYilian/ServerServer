using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class LoginPanel : BasePanel
{
    private static readonly string path = "UI/Panel/LoginPanel";

    private LoginRequest loginRequest;
    
    private string username;
    
    private bool isLoadChatPanel;//判断是否可以在主线程切换到聊天室里面

    private bool LoginFail;

    public LoginPanel() : base(new UIType(path))
    {
        username = null;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnEnter()
    {
        loginRequest = GameObject.Find("Canvas/LoginPanel").GetComponent<LoginRequest>();
        
        UITool.FindChildGameObject("UserName").GetComponentInChildren<InputField>().text = username;
        //退出
        UITool.GetOrAddComponentInChildren<Button>("BtnExit").onClick.AddListener(()=>
        {
            AudioManager.Instance.PlayButtonAudio();
            Pop();
        });
        //注册
        UITool.GetOrAddComponentInChildren<Button>("BtnRegister").onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButtonAudio();
            Pop();
            Push(new RegisterPanel());
        });
        //登录
        UITool.GetOrAddComponentInChildren<Button>("BtnPlay").onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButtonAudio();
            //检查账号密码
            string username = UITool.FindChildGameObject("UserName").GetComponentInChildren<InputField>().text;
            string password = UITool.FindChildGameObject("PassWord").GetComponentInChildren<InputField>().text;
            
            if (string.IsNullOrEmpty(username))
            {
                Debug.Log("用户名不能为空");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                Debug.Log("密码不能为空");
                return;
            }

            this.username = username;
            
            // 发送数据给服务器
            loginRequest.SendRequest(username,password);
            
            
        });
    }

    public override void OnUpdata()
    {
        if (isLoadChatPanel)
        {
            Debug.Log("进入下一个场景");
            isLoadChatPanel = false;
            GameFacade.Instance.PlayerName = username;
            ClientManager.Instance.Username = username;
            SceneStateController.Instance.SetState(new MainScene());
        }

        if (LoginFail)
        {
            LoginFail = false;
            Push((new NoticePanel("登录失败，请检查用户名与密码")));
        }
    }

    //点击登陆界面的登录按钮后接收到响应的方法
    public void OnLoginResponse(ReturnCode requestCode)
    {
        if (requestCode.Equals(ReturnCode.Success))
        {
            isLoadChatPanel = true;
            Debug.Log("登陆成功");
        }
        else
        {
            LoginFail = true;
            Debug.Log("登陆失败");
        }
    }
}
