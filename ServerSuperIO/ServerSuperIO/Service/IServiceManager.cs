using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Log;

namespace ServerSuperIO.Service
{
    public interface IServiceManager<TKey,TValue>:ILoggerProvider,IEnumerable where TValue : IService
    {
        TValue this[int index] { get; }

        TValue this[string key] { get; }

        bool AddService(string serviceKey, IService service);

        IService GetService(string serviceKey);

        bool RemoveService(string serviceKey);

        bool ContainsService(string serviceKey);

        void BatchUpdateDevice(string deviceID, object obj);

        void BatchRemoveDevice(string deviceID);

        void RemoveAllService();

        int Count { get; }
    }
}
