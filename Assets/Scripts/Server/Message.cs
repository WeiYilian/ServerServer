using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using UnityEngine;

//工具类脚本，用来解析和打包数据
public class Message
{
    //存取我们需要读取的数据
    private byte[] data = new byte[1024];
    //每次读取消息的时候，定义一个索引，根据索引读取数据内容
    private int startIndex = 0;

    public byte[] Data { get { return data; } }

    public int StarIndex { get { return startIndex; } }

    //当前数据里面还剩下多少字节的数据没有读取
    public int RemainSize { get { return data.Length - startIndex; } }

    //解析来自客户端的数据
    public void ReadMessage(int newCount, Action<ActionCode, string> processDataCallBack)
    {
        startIndex += newCount;//每一次更新索引
        while (true)
        {
            //当前数据已经没有可以读取的数据了，要从下一次接收到的数据里面读取
            if (startIndex <= 4) return;
            //得到数据长度的整型
            int count = BitConverter.ToInt32(data, 0);
            //判断当前data里面的数据够不够一次的读取量
            if (startIndex - 4 >= count)
            {
                //解析一次数据
                // string str = Encoding.UTF8.GetString(data, 12, count-8);
                // Console.WriteLine("接收到客户端发送的一次数据：" + str);

                //把收到的数据解析处理，分发给不同的Controller处理请求
                ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 4);
                string str = Encoding.UTF8.GetString(data, 8, count - 4);
                processDataCallBack(actionCode, str);

                //解析完之后，要把数据更新给下一次解析
                Array.Copy(data, 4 + count, data, 0, startIndex - count - 4);
                startIndex -= (count + 4);//每一次读完一次数据更新一次索引
            }
            else
                break;//数据不足或不完整 终止的条件
        }
    }
    
    /// <summary>
    /// 把需要给服务器的数据进行重组
    /// 实际数据：数据长度（优化粘包问题）+RequestCode（区分请求的操作类型）+ActionCode（操作的事件）+data
    /// BitConverter 根据值类型把数据转成字节数组
    /// Encoding.UTF8 根据字符转成字节数组
    /// </summary>
    /// <returns></returns>
    public static byte[] PackData(RequestCode requestCode,ActionCode actionCode,string data)
    {
        //requestCodeBytes和actionCodeBytes还有数据长度转成字节数组，都占用四个字节
        byte[] requestCodeBytes = BitConverter.GetBytes((int)requestCode);
        byte[] actionCodeBytes = BitConverter.GetBytes((int)actionCode);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        //获得数据长度
        int dataAmount = requestCodeBytes.Length + actionCodeBytes.Length + dataBytes.Length;
        //数据长度转成字节数组
        byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
        //四个字节数组重新组合成新的字节数组，作为发给服务器的数据
        return dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>()//在dataAmountBytes后面加上requestCodeBytes
            .Concat(actionCodeBytes).ToArray<byte>()//在dataAmountBytes和在dataAmountBytes后面加上actionCodeBytes
            .Concat(dataBytes).ToArray<byte>();//在dataAmountBytes和在dataAmountBytes和actionCodeBytes后面加上dataBytes
    }
}
