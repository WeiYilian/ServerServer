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
    
    public override void OnEnter()
    {
        registerRequest = GameObject.Find("Canvas/RegisterPanel").GetComponent<RegisterRequest>();
        
        UITool.GetOrAddComponentInChildren<Button>("BtnExit").onClick.AddListener(Pop);

        UITool.GetOrAddComponentInChildren<Button>("BtnRegister").onClick.AddListener(() =>
        {
            //将账号密码加入数据库
            string username = UITool.FindChildGameObject("UserName").GetComponentInChildren<InputField>().text;
            string password = UITool.FindChildGameObject("PassWord").GetComponentInChildren<InputField>().text;
            string age = UITool.FindChildGameObject("Age").GetComponentInChildren<InputField>().text;
            string gender = UITool.FindChildGameObject("Gender").transform.GetChild(1).GetComponentInChildren<Text>().text;
            
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
            
            if (isLoadChatPanel)
            {
                isLoadChatPanel = false;
                Pop();
                Push((new NoticePanel("注册成功")));
            }
            else
            {
                Push((new NoticePanel("注册失败，用户名已被使用")));
            }
            
            
        });
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
            Debug.Log("注册失败");
        }
    }
}
