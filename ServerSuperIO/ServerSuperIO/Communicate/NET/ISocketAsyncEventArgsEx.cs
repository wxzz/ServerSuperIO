using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.DataCache;

namespace ServerSuperIO.Communicate.NET
{
    public interface ISocketAsyncEventArgsEx:IReceiveCache
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();
    }
}
