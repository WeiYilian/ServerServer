using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using Common;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance;//单例模式
    
    public string IP;
    public int PORT;
    
    private Socket clientSocket;
    private Message ms = new Message();

    //字典保存我们每个响应事件对应的处理方法
    private Dictionary<ActionCode, BaseRequest> requestDict = new Dictionary<ActionCode, BaseRequest>();

    //当前玩家的用户名
    private string username;

    public string Username { get => username; set => username = value; }

    //获取IP的静态方法
    public static string getLocalIPAddressWithNetworkInterface(NetworkInterfaceType _type)
    {
        string output = "";
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        output = ip.Address.ToString();
                    }
                }
            }
        }
        Console.WriteLine("IP Address = " + output);
        return output;
    }

    private void Awake()
    {
        if(!Instance)
            Instance = this;
        
        IP = getLocalIPAddressWithNetworkInterface(NetworkInterfaceType.Wireless80211);//获取ip
    }

    private void Start()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(IPAddress.Parse(IP), PORT);

        clientSocket.BeginReceive(ms.Data, ms.StarIndex, ms.RemainSize, SocketFlags.None, ReceiveCallBack, null);
    }

    private void OnDestroy()
    {
        clientSocket.Close();
    }

    //向服务器发送请求的公共方法
    public void SendRequest(RequestCode requestCode,ActionCode actionCode,string data)
    {
        //把要发送的数据转成字节数组
        //实际数据：数据长度（优化粘包问题）+RequestCode（区分请求的操作类型）+ActionCode（操作的事件）+data
        byte[] byteData = Message.PackData(requestCode,actionCode,data);
        clientSocket.Send(byteData);
    }

    //接收消息的回调函数  参数需要引入命名空间 using Systems
    private void ReceiveCallBack(IAsyncResult ar)
    {
        int count = clientSocket.EndReceive(ar);//接收到服务器的一次的长度
        
        ms.ReadMessage(count,OnProcessMessage);
        
        //递归
        clientSocket.BeginReceive(ms.Data, ms.StarIndex, ms.RemainSize, SocketFlags.None, ReceiveCallBack, null);
    }
    
    //作为解析数据的委托
    private void OnProcessMessage(ActionCode actionCode, string data)
    {
        Debug.Log("接收到服务器的数据："+data);
        //把得到的请求数据，分发给不同的Request响应
        BaseRequest baseRequest;
        requestDict.TryGetValue(actionCode, out baseRequest);
        baseRequest.OnReponse(data);
    }

    //在每个Request里面添加和移除
    public void AddRequest(ActionCode actionCode, BaseRequest baseRequest)
    {
        requestDict.Add(actionCode,baseRequest);
    }

    public void RemoveRequest(ActionCode actionCode)
    {
        requestDict.Remove(actionCode);
    }
    
}
