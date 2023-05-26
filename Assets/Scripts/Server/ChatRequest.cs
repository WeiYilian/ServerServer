using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class ChatRequest : BaseRequest
{
    private ChatMainPanel chatMainPanel;
    private void Start()
    {
        requestCode = RequestCode.Chat;
        actionCode = ActionCode.HandleChat;
        ClientManager.Instance.AddRequest(actionCode,this);
        chatMainPanel = GetComponent<ChatMainPanel>();
    }
    
    private void OnDestroy()
    {
        ClientManager.Instance.RemoveRequest(actionCode);
    }
    
    //发送请求
    public void SendRequest(string data)
    {
        //通过clientManager发送登录请求
        ClientManager.Instance.SendRequest(requestCode,actionCode,data);
    }
    
    //登录请求的响应处理
    public override void OnReponse(string data)
    {
        base.OnReponse(data);
        chatMainPanel.OnReponse(data);
    }
}
