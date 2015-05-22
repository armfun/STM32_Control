using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace STM32Core
{
    internal sealed class BufferManager
    {
        private byte[] buffer;
        private int bufferSize;
        private int numSize;
        private int currentIndex;
        private Stack<int> freeIndexPool;

        internal BufferManager(int numsize, int buffersize)
        {
            this.numSize = numsize;
            this.bufferSize = buffersize;
        }

        internal void Inint()
        {
            buffer = new byte[numSize];
            freeIndexPool = new Stack<int>(numSize / bufferSize);
        }

        internal void FreeBuffer(SocketAsyncEventArgs args)
        {
            freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

        internal Boolean SetBuffer(SocketAsyncEventArgs args)
        {
            if (this.freeIndexPool.Count > 0)
            {
                args.SetBuffer(this.buffer, this.freeIndexPool.Pop(), this.bufferSize);
            }
            else
            {
                if ((this.numSize-this.bufferSize)<this.currentIndex)
                {
                    return false;
                }
                args.SetBuffer(this.buffer, this.currentIndex, this.bufferSize);
                this.currentIndex += this.bufferSize;
            }
            return true;
        }
    }
}
