using Common;
using UnityEngine;

public class RegisterRequest : BaseRequest
{
    private void Start()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Register;
        ClientManager.Instance.AddRequest(actionCode,this);
    }

    private void OnDestroy()
    {
        ClientManager.Instance.RemoveRequest(actionCode);
    }

    //发送请求
    public void SendRequest(string username,string password)
    {
        Debug.Log("发送注册请求");
        string data = username + "," + password;
        // 通过clientManager发送注册请求
        ClientManager.Instance.SendRequest(requestCode,actionCode,data);
    }
    
    //注册请求的响应处理
    public override void OnReponse(string data)
    {
        base.OnReponse(data);
        if (GameFacade.Instance.PanelManager.CurrentPanel().GetType() == typeof(RegisterPanel))
        {
            RegisterPanel registerPanel = (RegisterPanel)GameFacade.Instance.PanelManager.CurrentPanel();
            registerPanel.OnRegisterResponse((ReturnCode) (int.Parse(data)));
        }
    }
}
