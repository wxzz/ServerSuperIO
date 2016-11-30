using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Log;

namespace ServerSuperIO.Communicate
{
    public interface IControllerManager<TKey, TValue> :ILoggerProvider where TValue:IController
    {
        /// <summary>
        ///     增加控制器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        bool AddController(TKey key, TValue val);

        /// <summary>
        ///     获得可使用的控制器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue GetController(TKey key);

        /// <summary>
        ///     判断该Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainController(TKey key);

        /// <summary>
        ///     关闭指定控制器
        /// </summary>
        /// <param name="key"></param>
        bool RemoveController(TKey key);

        /// <summary>
        ///     关闭所有控制器
        /// </summary>
        void RemoveAllController();

        /// <summary>
        ///     获得值集合
        /// </summary>
        /// <returns></returns>
        ICollection<TValue> GetValues();

        /// <summary>
        ///     获得关键字集合
        /// </summary>
        /// <returns></returns>
        ICollection<TKey> GetKeys();

        /// <summary>
        /// 
        /// </summary>
        int ControllerCount { get; }
    }
}
