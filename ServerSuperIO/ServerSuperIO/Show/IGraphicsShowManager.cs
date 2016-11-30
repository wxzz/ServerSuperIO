using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Log;
using ServerSuperIO.Show;

namespace ServerSuperIO.Show
{
    public interface IGraphicsShowManager<TKey,TValue>: ILoggerProvider,IEnumerable where TValue : IGraphicsShow
    {
        TValue this[int index] { get; }

        TValue this[string key] { get; }

        bool AddShow(string showKey, IGraphicsShow service);

        IGraphicsShow GetShow(string showKey);

        bool RemoveShow(string showKey);

        bool ContainsShow(string showKey);

        void BatchUpdateDevice(string deviceID, object obj);

        void BatchRemoveDevice(string deviceID);

        void RemoveAllShow();

        int Count { get; }
    }
}
