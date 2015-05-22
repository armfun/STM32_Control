using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Core
{
    internal class BuffList
    {
        #region 属性
        internal List<byte> ByteList { set; get; }
        internal int CurrentPos { get; set; }
        internal bool SecondIn = false;
        internal bool FirstIn = true;
        #endregion

        #region 方法
        internal BuffList(int BuffCount)
        {
            ByteList = new List<byte>(BuffCount);
        }

        internal void Reset()
        {
            CurrentPos = 0;
        }

        internal void InserByteArray(byte[] data)
        {
            if (SecondIn)
            {
                ByteList.Clear();
                SecondIn = false;
            }
            if (FirstIn)
            {
                SecondIn = true;
                FirstIn = false;
            }
            ByteList.AddRange(data);
        }

        internal byte[] GetData()
        {
            try
            {
                byte[] data = ByteList.ToArray();
                if (data.Length >= 4)
                {
                    int length = BitConverter.ToInt32(data, 0);
                    if (length == ByteList.Count)
                    {
                        Reset();
                        ByteList.Clear();
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                Reset();
                ByteList.Clear();
                return null;
            }
        }
        #endregion
    }
}
