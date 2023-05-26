using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

//所有Request共有的父类
//继承的目的主要是在处理服务器响应的时候，作为数据类型保存在字典里
public class BaseRequest : MonoBehaviour
{
    protected RequestCode requestCode;// 向服务器发送请求的操作代码
    protected ActionCode actionCode;//处理服务器响应的事件代码
    
    public virtual void SendRequest() { }//发送请求给服务器
    public virtual void OnReponse(string data) { }//处理服务器的响应
}
