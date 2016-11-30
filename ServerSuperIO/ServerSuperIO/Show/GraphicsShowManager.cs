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
using ServerSuperIO.Service;

namespace ServerSuperIO.Show
{
    public class GraphicsShowManager : IGraphicsShowManager<string ,IGraphicsShow>
    {
        private Manager<string, IGraphicsShow> _manager;
        public GraphicsShowManager(ILog log=null)
        {
            Logger = log;
            _manager=new Manager<string, IGraphicsShow>();
        }

        public IGraphicsShow this[int index]
        {
            get
            {
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

        public IGraphicsShow this[string key]
        {
            get { return _manager[key]; }
        }

        public bool AddShow(string showKey, IGraphicsShow show)
        {
            return _manager.TryAdd(showKey, show);
        }

        public IGraphicsShow GetShow(string showKey)
        {
            if (_manager.ContainsKey(showKey))
            {
                IGraphicsShow show;
                if (_manager.TryGetValue(showKey, out show))
                {
                    return show;
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

        public bool RemoveShow(string showKey)
        {
            IGraphicsShow show;
            if (_manager.TryRemove(showKey, out show))
            {
                if (!show.IsDisposed)
                {
                    try
                    {
                        show.Dispose();
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

        public bool ContainsShow(string showKey)
        {
            return _manager.ContainsKey(showKey);
        }

        public void BatchUpdateDevice(string deviceID,object obj)
        {
            foreach (KeyValuePair<string ,IGraphicsShow> kv in _manager)
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
                            Logger.Error(true,"",ex);
                        }
                    }
                }
            }
        }

        public void BatchRemoveDevice(string deviceID)
        {
            foreach (KeyValuePair<string, IGraphicsShow> kv in _manager)
            {
                try
                {
                    kv.Value.RemoveDevice(deviceID);
                }
                catch(Exception ex)
                {
                    if (Logger != null)
                    {
                        Logger.Error(true, "", ex);
                    }
                }
            }
        }

        public void RemoveAllShow()
        {
            Parallel.ForEach(_manager, kv =>
            {
                if (!kv.Value.IsDisposed)
                {
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
