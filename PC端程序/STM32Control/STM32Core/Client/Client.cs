using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Core
{
    public class Client
    {
        public JsonMapper Mapper;
        public string Pwd = "";
        public Client(string pwd)
        {
            this.Pwd = pwd;
            this.Mapper = new JsonMapper();
        }

        /// <summary>
        /// DHT11温湿度传感器操作
        /// </summary>
        /// <returns></returns>
        public byte[] GetDHT1x()
        {
            return BuffersWriter.Merge(BuffersWriter.GetBytes((int)Operations.GetDHT1x), BuffersWriter.GetBytes(Pwd,Encoding.UTF8));
        }
        /// <summary>
        /// 设置数字端口电平
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public byte[] SetDigitalLevel(int pin, DigitalLevel level)
        {
            return BuffersWriter.Merge(BuffersWriter.GetBytes((int)Operations.SetDigital), BuffersWriter.GetBytes(Pwd,Encoding.UTF8),
                BuffersWriter.GetBytes((byte)pin),BuffersWriter.GetBytes((byte)level));
        }
        /// <summary>
        /// 获取数字端口和模拟端口状态
        /// </summary>
        /// <returns></returns>
        public byte[] GetDigitalAndAnalog()
        {
            return BuffersWriter.Merge(BuffersWriter.GetBytes((int)Operations.GetDigitalAndAnalog), BuffersWriter.GetBytes(Pwd, Encoding.UTF8));
        }
        /// <summary>
        /// 设置脉冲宽度
        /// </summary>
        /// <param name="pin">引脚号</param>
        /// <param name="value">脉冲值</param>
        /// <returns></returns>
        public byte[] SetPWM(int pin, byte value)
        {
            return BuffersWriter.Merge(BuffersWriter.GetBytes((int)Operations.SetPWM), BuffersWriter.GetBytes(Pwd, Encoding.UTF8),
                BuffersWriter.GetBytes((byte)pin), BuffersWriter.GetBytes(value));
        }
        /// <summary>
        /// 红外学习
        /// </summary>
        /// <returns></returns>
        public byte[] IRLearn()
        {
            return BuffersWriter.Merge(BuffersWriter.GetBytes((int)Operations.IRLearn), BuffersWriter.GetBytes(Pwd, Encoding.UTF8));
        }
        /// <summary>
        /// 红外发射
        /// </summary>
        /// <returns></returns>
        public byte[] IRSend()
        {
            return BuffersWriter.Merge(BuffersWriter.GetBytes((int)Operations.IRSend), BuffersWriter.GetBytes(Pwd, Encoding.UTF8));
        }
    }
}
