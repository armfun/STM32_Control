using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Core
{
    public static class BuffersWriter
    {
        /// <summary>
        /// 将一个二维数据包整合成一个一维数据包
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static byte[] Merge(params byte[][] args)
        {
            int length = 0;
            //计算数据包总长度
            foreach (byte[] temp in args)
            {
                length += temp.Length;
            }
            byte[] bytes = new byte[length];
            int tempLength = 0;
            foreach (byte[] temp in args)
            {
                temp.CopyTo(bytes, tempLength);
                tempLength += temp.Length;
            }
            return bytes;
        }

        /// <summary>
        /// 将一个32位整形转换成一个BYTE[]4字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetBytes(Int32 data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个float值转换成一个BYTE[]4字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public byte[] GetBytes(float data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个16位整形转换成一个BYTE[]2字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetBytes(Int16 data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个Double值转换成一个BYTE[]4字节
        /// </summary>
        static public byte[] GetBytes(double data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个DateTime值转换成一个BYTE[]8字节
        /// </summary>
        static public byte[] GetBytes(DateTime data)
        {
            return BitConverter.GetBytes(data.Ticks);
        }

        /// <summary>
        /// 将一个Bool值转换成一个BYTE(True:1,False:0)
        /// </summary>
        static public Byte[] GetBytes(bool data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个64位整型转换成以个BYTE[] 8字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetBytes(Int64 data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个 1位CHAR转换成1位的BYTE
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetBytes(byte data)
        {
            return new Byte[] { (Byte)data };
        }

        /// <summary>
        /// 将一个BYTE[]数据包添加首位长度
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetBytes(Byte[] data)
        {
            return Merge(GetBytes(data.Length), data);
        }

        /// <summary>
        /// 将一个字符串转换成BYTE[]，BYTE[]的首位是字符串的长度
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="encoder">编码器</param>
        /// <returns></returns>
        static public Byte[] GetBytes(String data, Encoding encoder)
        {
            Byte[] bytes = encoder.GetBytes(data);

            return Merge(GetBytes(bytes.Length),bytes);
        }
    }
}
