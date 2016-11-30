using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Base
{
    public class Manager<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {

    }
}
