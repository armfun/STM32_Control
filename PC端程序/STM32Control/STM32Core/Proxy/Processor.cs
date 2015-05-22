using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Core
{
    /// <summary>
    /// 连接通知
    /// </summary>
    /// <param name="message"></param>
    public delegate void OnConnect(bool message);
    /// <summary>
    /// 消息通知
    /// </summary>
    /// <param name="data"></param>
    public delegate void OnData(byte[] data);
    /// <summary>
    /// 断开连接通知
    /// </summary>
    /// <param name="message"></param>
    public delegate void OnDisconnect(string message);
}
