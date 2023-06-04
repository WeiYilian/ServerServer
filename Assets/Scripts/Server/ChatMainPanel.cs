using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ChatMainPanel : MonoBehaviour
{
    public GameObject TextPrefabs;//对话框的预设
    public Transform ParentTra;//对话框生成的父物体
    public InputField MessageIF;//输入框内容
    public ScrollRect ScrollRect;//更新对话滑动条到最新的位置

    private ChatRequest mainRequest;

    private bool isSendInfo;//是否同步聊天内容，转到主线程处理

    private string data;//临时变量，保存服务器发来的数据,转到主线程处理
    
    private void Start()
    {
        mainRequest = GetComponent<ChatRequest>();
    }

    private void Update()
    {
        if (isSendInfo)
        {
            isSendInfo = false;
            string[] str = data.Split('|');
            //判断服务器发来的是不是当前用户，避免重复发送消息
            if(str[0] != ClientManager.Instance.Username)
                Instantiate(TextPrefabs, ParentTra).GetComponent<Text>().text = str[0] + "：" + str[1];
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (string.IsNullOrEmpty(MessageIF.text)) return;
            //当前客户端生成
            GameObject go = Instantiate(TextPrefabs, ParentTra);
            go.GetComponent<Text>().text = ClientManager.Instance.Username + "：" + MessageIF.text;
            go.GetComponent<Text>().color = Color.green;
            //开启协程，更新滑动条位置
            StartCoroutine(nameof(ScrollToBottom));
            //发到服务器同步
            mainRequest.SendRequest(ClientManager.Instance.Username + "|" + MessageIF.text);
            MessageIF.text = "";
        }
    }

    //控件写入需要等待一帧时间才能更新
    IEnumerator ScrollToBottom()
    {
        yield return null;
        //生成对话之后，更新滑动条位置
        ScrollRect.verticalNormalizedPosition = 0;
    }
    //处理聊天内容同步的响应
    public void OnReponse(string data)
    {
        isSendInfo = true;
        this.data = data;
    }

    public void Exit()
    {
        GameFacade.Instance.PanelManager.Pop();
    }
}
