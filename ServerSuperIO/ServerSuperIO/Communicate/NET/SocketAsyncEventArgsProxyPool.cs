using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerSuperIO.Communicate.NET
{
    public class SocketAsyncEventArgsProxyPool
    {
        //just for assigning an ID so we can watch our objects while testing.
        private Int32 nextTokenId = 0;
        
        // Pool of reusable SocketAsyncEventArgs objects.        
        ConcurrentQueue<ISocketAsyncEventArgsProxy> pool;
        
        // initializes the object pool to the specified size.
        // "capacity" = Maximum number of SocketAsyncEventArgs objects
        internal SocketAsyncEventArgsProxyPool()
        {
            
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    Program.testWriter.WriteLine("SocketAsyncEventArgsPool constructor");
            //}

            this.pool = new ConcurrentQueue<ISocketAsyncEventArgsProxy>();
        }

        // The number of SocketAsyncEventArgs instances in the pool.         
        internal Int32 Count
        {
            get { return this.pool.Count; }
        }

        internal Int32 AssignTokenId()
        {
            Int32 tokenId = Interlocked.Increment(ref nextTokenId);            
            return tokenId;
        }

        // Removes a SocketAsyncEventArgs instance from the pool.
        // returns SocketAsyncEventArgs removed from the pool.
        internal ISocketAsyncEventArgsProxy Pop()
        {
            ISocketAsyncEventArgsProxy proxy;

            lock (this.pool)
            {
                return this.pool.TryDequeue(out proxy) == true? proxy : null;
            }
        }

        // Add a SocketAsyncEventArg instance to the pool. 
        // "item" = SocketAsyncEventArgs instance to add to the pool.
        internal void Push(ISocketAsyncEventArgsProxy item)
        {
            if (item == null) 
            { 
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); 
            }

            lock (this.pool)
            {
                this.pool.Enqueue(item);
            }
        }

        internal void Clear()
        {
            lock (this.pool)
            {
                if (pool.Count > 0)
                {
                    ISocketAsyncEventArgsProxy proxy;
                    for(int i=0;i<pool.Count;i++)
                    {
                        if (pool.TryDequeue(out proxy))
                        {
                            proxy.Reset();
                            proxy.SocketReceiveEventArgsEx.Dispose();
                            proxy.SocketReceiveEventArgsEx = null;
                            proxy.SocketSendEventArgs.Dispose();
                            proxy.SocketSendEventArgs = null;
                        }
                    }
                }
            }
        }
    }
}
