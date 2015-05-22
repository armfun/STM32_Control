using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace STM32Core
{
    public class Proxy : IDisposable
    {
        #region 属性
        bool isSending = false;
        Queue<byte[]> outQueue = new Queue<byte[]>(10000);
        public OnConnect onConnect { set; get; }
        public OnData onData { set; get; }
        public OnDisconnect onDisconnect { set; get; }

        private AsyncOperation asyncOp;
        byte[] RecevieBuff { set; get; }
        SocketAsyncEventArgs ReceiveEventArgs { set; get; }
        public Socket socket { set; get; }
        byte[] SendBuff { set; get; }
        SocketAsyncEventArgs SendEventArgs { set; get; }
        BuffList buffList;
        static byte[] data;
        #endregion

        #region 方法
        public void Connect(string serverIP, int serverPort, int buffCount)
        {
            asyncOp = AsyncOperationManager.CreateOperation(null);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            buffList = new BuffList(buffCount);
            ReceiveEventArgs = new SocketAsyncEventArgs();
            RecevieBuff = new byte[buffCount];
            ReceiveEventArgs.Completed += Connect_Completed;
            ReceiveEventArgs.RemoteEndPoint = new DnsEndPoint(serverIP, serverPort);
            socket.ConnectAsync(ReceiveEventArgs);
            Thread outThread = new Thread(new ThreadStart(outExcution));
            outThread.IsBackground = true;
            outThread.Start();
        }

        void Connect_Completed(object sender, SocketAsyncEventArgs e)
        {
            ReceiveEventArgs.Completed -= Connect_Completed;
            if (e.SocketError == SocketError.Success)
            {
                ReceiveEventArgs.SetBuffer(RecevieBuff, 0, RecevieBuff.Length);
                SendEventArgs = new SocketAsyncEventArgs();
                SendBuff = new byte[RecevieBuff.Length];
                SendEventArgs.SetBuffer(SendBuff, 0, SendBuff.Length);
                SendEventArgs.RemoteEndPoint = e.RemoteEndPoint;
                SendEventArgs.Completed += SendEventArgs_Completed;
                ReceiveEventArgs.Completed += Receive_Completed;
                socket.ReceiveAsync(ReceiveEventArgs);
                if (onConnect != null)
                {
                    asyncOp.Post(result =>
                        {
                            onConnect(true);
                        }, null);
                }
            }
            else
            {
                if (onConnect != null)
                {
                    asyncOp.Post(result =>
                        {
                            onConnect(false);
                        }, null);
                }
                handleExpression();
            }
        }

        void SendEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            isSending = false;
        }

        void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                byte[] temp = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, 0, temp, 0, temp.Length);
                Array.Clear(e.Buffer, e.Offset, e.BytesTransferred);
                buffList.InserByteArray(temp);
                do 
                {
                    byte[] buff = buffList.GetData();
                    if (buff != null)
                    {
                        data = new byte[buff.Length - 4];
                        Buffer.BlockCopy(buff, 4, data, 0, data.Length);
                        ThreadPool.QueueUserWorkItem(ondata, data);
                    }
                    else
                    {
                        break;
                    }
                } while (true);
                socket.ReceiveAsync(ReceiveEventArgs);
            }
        }

        /// <summary>
        /// 发送指令到设备
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public string ToSTM32(byte[] order)
        {
            if (order != null)
            {
                byte[] data = BuffersWriter.Merge(order, Encoding.UTF8.GetBytes(new char[] { '\r', '\n' }));
                data = BuffersWriter.Merge(BuffersWriter.GetBytes(data.Length + 4), data);
                try
                {
                    Buffer.BlockCopy(data, 0, SendBuff, 0, data.Length);
                    SendEventArgs.SetBuffer(0, data.Length);
                    if (!socket.SendAsync(SendEventArgs))
                    {
                        Array.Clear(SendBuff, 0, SendBuff.Length);
                    }
                }
                catch (ArgumentNullException ex)
                {
                    return "无法连接到硬件";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return "完成";
        }

        /// <summary>
        /// 分发器
        /// </summary>
        void outExcution()
        {
            while (true)
            {
                if (outQueue.Count >0 && isSending == false)
                {
                    send(popOut());
                }
                System.Threading.Thread.Sleep(20);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        void handleExpression()
        {
            if (onDisconnect != null)
            {
                asyncOp.Post(result =>
                {
                    onDisconnect("连接失败请重试");
                    Dispose();
                }, null);
            }
        }

        void ondata(object data)
        {
            if (onData != null)
            {
                asyncOp.Post(result =>
                    {
                        onData((byte[])data);
                    },null);
            }
        }

        void send(object data)
        {
            if (data != null)
            {
                isSending = true;
                byte[] buff = (byte[])data;
                buff = BuffersWriter.Merge(BuffersWriter.GetBytes(buff.Length + 4), buff);
                Buffer.BlockCopy(buff, 0, SendBuff, 0, buff.Length);
                SendEventArgs.SetBuffer(0, buff.Length);
                if (!socket.SendAsync(SendEventArgs))
                {
                    Array.Clear(SendBuff, 0, SendBuff.Length);
                }
            }
        }

        #region 消息队列处理
        void addOut(object data)
        {
            lock (outQueue)
            {
                outQueue.Enqueue((byte[])data);
            }
        }

        byte[] popOut()
        {
            lock (outQueue)
            {
                return outQueue.Dequeue();
            }
        }
        #endregion

        /// <summary>
        /// 释放客户端代理
        /// </summary>
        public void Dispose()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
            socket.Dispose();
            ReceiveEventArgs.Dispose();
            try
            {
                SendEventArgs.Dispose();
            }
            catch { }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
