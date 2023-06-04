using Common;
using UnityEngine;

public class LoginRequest : BaseRequest
{
    private void Start()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Login;
        ClientManager.Instance.AddRequest(actionCode,this);
    }

    private void OnDestroy()
    {
        ClientManager.Instance.RemoveRequest(actionCode);
    }

    //发送请求
    public void SendRequest(string username,string password)
    {
        string data = username + "," + password;
        //通过clientManager发送登录请求
        ClientManager.Instance.SendRequest(requestCode,actionCode,data);
    }
    
    //登录请求的响应处理
    public override void OnReponse(string data)
    {
        base.OnReponse(data);
        if (GameFacade.Instance.PanelManager.CurrentPanel().GetType() == typeof(LoginPanel))
        {
            LoginPanel loginPanel = (LoginPanel)GameFacade.Instance.PanelManager.CurrentPanel();
            loginPanel.OnLoginResponse((ReturnCode) (int.Parse(data)));
        }
    }
}
