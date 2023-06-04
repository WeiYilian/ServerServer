using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class RegisterPanel : BasePanel
{
    private static readonly string path = "UI/Panel/RegisterPanel";
    
    public RegisterPanel():base(new UIType(path)){}
    
    private RegisterRequest registerRequest;

    private bool isLoadChatPanel;//判断是否可以在主线程切换到聊天室里面

    private bool RegisterFail;
    
    public override void OnEnter()
    {
        registerRequest = GameObject.Find("Canvas/RegisterPanel").GetComponent<RegisterRequest>();
        
        UITool.GetOrAddComponentInChildren<Button>("BtnExit").onClick.AddListener(Pop);

        UITool.GetOrAddComponentInChildren<Button>("BtnRegister").onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayButtonAudio();
            //将账号密码加入数据库
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

            // 发送数据给服务器
            registerRequest.SendRequest(username,password);
            
           
        });
    }

    public override void OnUpdata()
    {
         if (isLoadChatPanel)
        {
            isLoadChatPanel = false;
            Pop();
            Push((new NoticePanel("注册成功")));
        }
         
        if(RegisterFail)
        {
            RegisterFail = false;
            Push((new NoticePanel("注册失败，用户名已被使用")));
        }
    }

    //点击注册界面的注册按钮后接收到响应的方法
    public void OnRegisterResponse(ReturnCode requestCode)
    {
        if (requestCode.Equals(ReturnCode.Success))
        {
            isLoadChatPanel = true;
            Debug.Log("注册成功");
        }
        else
        {
            RegisterFail = true;
            Debug.Log("注册失败");
        }
    }
}
