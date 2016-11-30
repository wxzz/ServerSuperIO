using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Base;
using ServerSuperIO.Log;

namespace ServerSuperIO.Service
{
    public class ServiceManager:IServiceManager<string ,IService>
    {
        private Manager<string, IService> _manager;
        public ServiceManager(ILog log=null)
        {
            Logger = log;
            _manager=new Manager<string, IService>();
        }

        public IService this[int index]
        {
            get {
                if (index >= 0 && index <= _manager.Count - 1)
                {
                    return _manager.Values.ToArray()[index];
                }
                else
                {
                    return null;
                }
            }
        }

        public IService this[string key]
        {
            get { return _manager[key]; }
        }

        public bool AddService(string serviceKey, IService service)
        {
            return _manager.TryAdd(serviceKey, service);
        }

        public IService GetService(string serviceKey)
        {
            if (_manager.ContainsKey(serviceKey))
            {
                IService service;
                if (_manager.TryGetValue(serviceKey, out service))
                {
                    return service;
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

        public bool RemoveService(string serviceKey)
        {
            IService service;
            if (_manager.TryRemove(serviceKey, out service))
            {
                if (!service.IsDisposed)
                {
                    try
                    {
                        service.StopService();
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(true, "", ex);
                    }

                    try
                    {
                        service.Dispose();
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(true, "", ex);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ContainsService(string serviceKey)
        {
            return _manager.ContainsKey(serviceKey);
        }

        public void BatchUpdateDevice(string deviceID,object obj)
        {
            foreach (KeyValuePair<string ,IService> kv in _manager)
            {
                if (!kv.Value.IsDisposed)
                {
                    try
                    {
                        kv.Value.UpdateDevice(deviceID, obj);
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                        {
                            Logger.Error(true, "", ex);
                        }
                    }
                }
            }
        }

        public void BatchRemoveDevice(string deviceID)
        {
            foreach (KeyValuePair<string, IService> kv in _manager)
            {
                try
                {
                    kv.Value.RemoveDevice(deviceID);
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                    {
                        Logger.Error(true, "", ex);
                    }
                }
            }
        }

        public void RemoveAllService()
        {
            Parallel.ForEach(_manager, kv =>
            {
                if (!kv.Value.IsDisposed)
                {
                    try
                    {
                        kv.Value.StopService();
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                        {
                            Logger.Error(true, "", ex);
                        }
                    }

                    try
                    {
                        kv.Value.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                        {
                            Logger.Error(true, "", ex);
                        }
                    }
                }
            });

            _manager.Clear();
        }

        public int Count {
            get { return this._manager.Count; }
        }

        public ILog Logger { get; private set; }
        public IEnumerator GetEnumerator()
        {
            return this._manager.GetEnumerator();
        }
    }
}
