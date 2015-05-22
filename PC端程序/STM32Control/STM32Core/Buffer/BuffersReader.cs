using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace STM32Core
{
    public class BuffersReader
    {
        /// <summary>
        /// 游标
        /// </summary>
        public int Current;

        private byte[] Data;

        /// <summary>
        /// 内存流长度
        /// </summary>
        public int Length { get; set; }


        /// <summary>
        /// 重置内存流指针
        /// </summary>
        public void Reset()
        {
            Current = 0;
        }

        /// <summary>
        /// DYCom消息读取器
        /// </summary>
        /// <remarks>读取消息中的内容</remarks>
        public BuffersReader(Byte[] data)
        {
            if (data != null)
            {
                Data = data;
                this.Length = Data.Length;
                Current = 0;
            }
        }

        /// <summary>
        /// 读取内存流中的4位并转换成整型
        /// </summary>
        /// <param name="values">内存流</param>
        /// <returns></returns>
        public bool ReadInt32(out int values)
        {
            try
            {
                values = BitConverter.ToInt32(Data, Current);
                Current = Interlocked.Add(ref Current, 4);
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }
        }

        /// <summary>
        /// 读取内存流中的4位并转换浮点型
        /// </summary>
        /// <param name="values">内存流</param>
        /// <returns></returns>
        public bool ReadFloat(out float values)
        {
            try
            {
                values = BitConverter.ToSingle(Data, Current);
                Current = Interlocked.Add(ref Current, 4);
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }
        }

        /// <summary>
        /// 读取内存流中的Double值
        /// </summary>
        public bool ReadDouble(out double values)
        {
            try
            {
                values = BitConverter.ToDouble(Data, Current);
                Current = Interlocked.Add(ref Current, 8);
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }
        }

        /// <summary>
        /// 读取内存流中的Long值
        /// </summary>
        public bool ReadInt64(out long values)
        {
            try
            {
                values = BitConverter.ToInt64(Data, Current);
                Current = Interlocked.Add(ref Current, 8);
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }
        }

        /// <summary>
        /// 读取内存流中的Bool值
        /// </summary>
        public bool ReadBool(out bool values)
        {
            try
            {
                values = BitConverter.ToBoolean(Data, Current);
                Current = Interlocked.Add(ref Current, 1);
                return true;
            }
            catch
            {
                values = false;
                return false;
            }
        }

        /// <summary>
        /// 读取内存流中的DataTime值
        /// </summary>
        public bool ReadDateTime(out DateTime values)
        {
            try
            {
                long ticks = BitConverter.ToInt64(Data, Current);
                values = new DateTime(ticks);
                Current = Interlocked.Add(ref Current, 8);
                return true;
            }
            catch
            {
                values = DateTime.MinValue;
                return false;
            }
        }

        /// <summary>
        /// 读取内存流中的首位
        /// </summary>
        /// <param name="values">读出变量</param>
        /// <returns></returns>
        public bool ReadByte(out byte values)
        {
            try
            {
                values = (byte)Data[Current];
                Current = Interlocked.Increment(ref Current);
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }
        }


        /// <summary>
        /// 读取内存流中的2位并转换成整型
        /// </summary>
        /// <param name="values">读出变量</param>
        /// <returns></returns>
        public bool ReadInt16(out short values)
        {
            try
            {
                values = BitConverter.ToInt16(Data, Current);
                Current = Interlocked.Add(ref Current, 2);
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }
        }


        /// <summary>
        /// 读取内存流中一段字符串
        /// </summary>
        /// <param name="values">读出变量</param>
        /// <param name="encoder">解码器</param>
        /// <returns></returns>
        public bool ReadString(out string values, Encoding encoder)
        {
            int lengt;
            try
            {
                if (ReadInt32(out lengt))
                {
                    Byte[] buf = new Byte[lengt];
                    Buffer.BlockCopy(Data, Current, buf, 0, buf.Length);
                    values = encoder.GetString(buf, 0, buf.Length);
                    Current = Interlocked.Add(ref Current, lengt);
                    return true;
                }
                else
                {
                    values = "";
                    return false;
                }
            }
            catch
            {
                values = "";
                return false;
            }

        }


        /// <summary>
        /// 读取内存流中一段数据
        /// </summary>
        /// <param name="values">读出变量</param>
        /// <returns></returns>
        public bool ReadByteArray(out byte[] values)
        {
            int lengt;
            try
            {
                if (ReadInt32(out lengt))
                {
                    values = new Byte[lengt];
                    Buffer.BlockCopy(Data, Current, values, 0, values.Length);
                    Current = Interlocked.Add(ref Current, lengt);
                    return true;

                }
                else
                {
                    values = null;
                    return false;
                }
            }
            catch
            {
                values = null;
                return false;
            }

        }

        /// <summary>
        /// 读取内存流中剩余部份
        /// </summary>
        public byte[] GetCurrentToEnd()
        {
            byte[] m = new byte[Data.Length - Current];
            Buffer.BlockCopy(Data, Current, m, 0, Data.Length - Current);
            return m;
        }
    }
 }

