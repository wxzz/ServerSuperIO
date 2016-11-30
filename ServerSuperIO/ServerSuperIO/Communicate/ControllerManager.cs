using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Base;
using ServerSuperIO.Communicate;
using ServerSuperIO.Log;

namespace ServerSuperIO.Communicate
{
    public class ControllerManager : IControllerManager<string, IController>
    {
        private Manager<string, IController> _Controllers;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ControllerManager(ILog log=null)
        {
            Logger = log;
            _Controllers = new Manager<string, IController>();
        }

        /// <summary>
        /// 增加控制器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool AddController(string key, IController val)
        {
            return _Controllers.TryAdd(key, val);
        }

        /// <summary>
        /// 获得控制器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IController GetController(string key)
        {
            IController val;
            if (_Controllers.TryGetValue(key, out val))
            {
                return val;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainController(string key)
        {
            return _Controllers.ContainsKey(key);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public bool RemoveController(string key)
        {
            IController controller;
            if (_Controllers.TryRemove(key, out controller))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 移除所有
        /// </summary>
        public void RemoveAllController()
        {
            //foreach (KeyValuePair<string, IController> kv in _Controllers)
            //{
            //    if (!kv.Value.IsDisposed)
            //    {
            //        kv.Value.Dispose();
            //    }
            //}

            Parallel.ForEach(_Controllers, controller =>
            {
                try
                {
                    controller.Value.Dispose();
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                    {
                        Logger.Error(true,"",ex);
                    }
                }
            });

            _Controllers.Clear();
        }

        /// <summary>
        /// 获得值
        /// </summary>
        /// <returns></returns>
        public ICollection<IController> GetValues()
        {
            return _Controllers.Values;
        }

        /// <summary>
        /// 获得键
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetKeys()
        {
            return _Controllers.Keys;
        }


        public int ControllerCount
        {
            get { return _Controllers.Count; }
        }

        public ILog Logger { get; private set; }
    }
}
