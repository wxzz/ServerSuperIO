using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerSuperIO.Common;

namespace ServerSuperIO.Persistence
{
    public abstract class XmlPersistence:IXmlPersistence
    {
        private ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly TimeSpan _lockTimeOut = TimeSpan.FromMilliseconds(100);

        public void Save<T>(T t)
        {
            if (_rwLock.TryEnterWriteLock(_lockTimeOut))
            {
                try
                {
                    SerializeUtil.XmlSerialize<T>(this.SavePath, t);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }
        }

        public T Load<T>()
        {
            if (_rwLock.TryEnterReadLock(_lockTimeOut))
            {
                try
                {
                    return SerializeUtil.XmlDeserailize<T>(this.SavePath);
                }
                catch
                {
                    return (T)Repair();
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }
            else
            {
                return default(T);
            }
        }

        public void Delete()
        {
            if (System.IO.File.Exists(this.SavePath))
            {
                System.IO.File.Delete(this.SavePath);
            }
        }

        public abstract string SavePath { get; }

        public abstract object Repair();
    }
}
