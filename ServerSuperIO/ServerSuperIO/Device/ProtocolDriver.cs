using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ServerSuperIO.Base;
using ServerSuperIO.Communicate;
using ServerSuperIO.DataCache;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;

namespace ServerSuperIO.Device
{
    public abstract class ProtocolDriver:IProtocolDriver
    {
        private Manager<string,IProtocolCommand> _Commands = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        protected ProtocolDriver()
        {
            _Commands = new Manager<string, IProtocolCommand>();
            SendCache=new SendCache();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~ProtocolDriver()
        {
            if (_Commands != null)
            {
                _Commands.Clear();
                _Commands = null;
            }

            if (SendCache != null && SendCache.Count > 0)
            {
                SendCache.Clear();
            }
        }

        /// <summary>
        /// 初始化协议驱动
        /// </summary>
        /// <param name="cmdAsmType">带命令驱动的程序集类型</param>
        /// <param name="receiveFilter"></param>
        public virtual void InitDriver(Type cmdAsmType, IReceiveFilter receiveFilter)
        {
            ReceiveFilter = receiveFilter;

            this._Commands.Clear();
            System.Reflection.Assembly asm = cmdAsmType.Assembly;
            Type[] types = asm.GetTypes();
            foreach (Type t in types)
            {
                if (typeof(IProtocolCommand).IsAssignableFrom(t))
                {
                    if (t.Name != "IProtocolCommand"
                        && t.Name != "ProtocolCommand"
                        && !t.IsAbstract)
                    {
                        IProtocolCommand cmd = (IProtocolCommand)t.Assembly.CreateInstance(t.FullName);
                        if (cmd != null)
                        {
                            cmd.Setup(this);
                            _Commands.TryAdd(cmd.Name, cmd);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public IProtocolCommand GetProcotolCommand(string cmdName)
        {
            IProtocolCommand cmd;
            if (this._Commands!=null && this._Commands.TryGetValue(cmdName, out cmd))
            {
                return cmd;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 驱动命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="t"></param>
        public void DriverCommand<T>(string cmdName,T t)
        {
            IProtocolCommand cmd = GetProcotolCommand(cmdName);
            if (cmd != null)
            {
                cmd.ExcuteCommand<T>(t);
            }
        }

        public void DriverCommand<T1, T2>(string cmdName, T1 t1, T2 t2)
        {
            IProtocolCommand cmd = GetProcotolCommand(cmdName);
            if (cmd != null)
            {
                cmd.ExcuteCommand<T1,T2>(t1,t2);
            }
        }

        public dynamic DriverAnalysis<T>(string cmdName, byte[] data, T t)
        {
            IProtocolCommand cmd = GetProcotolCommand(cmdName);
            if (cmd != null)
            {
                return cmd.Analysis<T>(data, t);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="data"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public dynamic DriverAnalysis<T1,T2>(string cmdName, byte[] data, T1 t1,T2 t2)
        {
            IProtocolCommand cmd = GetProcotolCommand(cmdName);
            if (cmd != null)
            {
                return cmd.Analysis<T1, T2>(data, t1, t2);
            }
            else
            {
                return null;
            }
        }


        public byte[] DriverPackage<T>(string code, string cmdName, T t)
        {
            IProtocolCommand cmd = GetProcotolCommand(cmdName);
            if (cmd != null)
            {
                return cmd.Package<T>(code, t);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="code"></param>
        /// <param name="cmdName"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public byte[] DriverPackage<T1, T2>(string code, string cmdName, T1 t1,T2 t2)
        {
            IProtocolCommand cmd = GetProcotolCommand(cmdName);
            if (cmd != null)
            {
                return cmd.Package<T1, T2>(code, t1, t2);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 校验数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract bool CheckData(byte[] data);

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract byte[] GetCommand(byte[] data);

        /// <summary>
        /// 获得地址
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract int GetAddress(byte[] data);

        /// <summary>
        /// 获得校验数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract byte[] GetCheckData(byte[] data);

        /// <summary>
        /// 获得ID信息，是该传感器的唯一标识。2016-07-29新增加（wxzz)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract string GetCode(byte[] data);

        /// <summary>
        /// 获得应该接收的数据长度，如果当前接收的数据小于这个返回值，那么继续接收数据，直到大于等于这个返回长度。如果接收数据超时，则直接返回当前已经接收的数据。
        /// </summary>
        /// <param name="data">接收的数据</param>
        /// <param name="channel">IO通道，用于返回确认数据</param>
        /// <param name="readTimeout">返回读数据超时间隔时间</param>
        /// <returns></returns>
        public abstract int GetPackageLength(byte[] data, IChannel channel, ref int readTimeout);

        /// <summary>
        /// 协议头
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract byte[] GetHead(byte[] data);

        /// <summary>
        /// 协议尾
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract byte[] GetEnd(byte[] data);


        /// <summary>
        /// 发送数据缓存
        /// </summary>
        public ISendCache SendCache { private set; get; }

        /// <summary>
        /// 协议过滤器
        /// </summary>
        public IReceiveFilter ReceiveFilter { set; get; }
    }
}
