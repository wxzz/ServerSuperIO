using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Base;
using ServerSuperIO.Communicate.COM;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Log;

namespace ServerSuperIO.Communicate
{
    public class ChannelManager : IChannelManager<string, IChannel>
    {
        private Manager<string, IChannel> _Channels;
        private object _SyncLock;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChannelManager(ILog log=null)
        {
            Logger = log;
            _SyncLock=new object();
            _Channels = new Manager<string, IChannel>();
        }

        /// <summary>
        /// 同步对象
        /// </summary>
        public object SyncLock
        {
            get { return _SyncLock; }
        }

        /// <summary>
        /// 增加通道
        /// </summary>
        /// <param name="key"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool AddChannel(string key, IChannel channel)
        {
            return _Channels.TryAdd(key, channel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IChannel GetChannel(string key)
        {
            if (_Channels.ContainsKey(key))
            {
                IChannel val;
                if (_Channels.TryGetValue(key, out val))
                {
                    return val;
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

        public IChannel GetChannel(string ioPara1, CommunicateType comType)
        {
            IChannel channel = null;
            foreach (KeyValuePair<string,IChannel> c in _Channels)
            {
                if (c.Value.CommunicationType == comType && c.Value.Key==ioPara1)
                {
                    channel = c.Value;
                    break;
                }
            }
            return channel;
        }

        public IChannel GetChannel(string ioPara1,int ioPara2, CommunicateType comType)
        {
            IChannel channel = null;
            foreach (KeyValuePair<string, IChannel> c in _Channels)
            {
                if (c.Value.CommunicationType == comType)
                {
                    ISocketSession socketSession = (ISocketSession)c.Value;
                    if (socketSession.RemoteIP == ioPara1 && socketSession.RemotePort == ioPara2)
                    {
                        channel = c.Value;
                        break;
                    }
                    
                }
                else if (c.Value.CommunicationType == comType)
                {
                    IComSession comSession = (IComSession)c.Value;
                    if (ComUtils.PortToString(comSession.Port)  == ioPara1 && comSession.Baud == ioPara2)
                    {
                        channel = c.Value;
                        break;
                    }
                }
            }
            return channel;
        }


        public ICollection<IChannel> GetChannels(CommunicateType ioType)
        {
           return GetValues().Where(v => v.CommunicationType == CommunicateType.NET).ToList();
        }

        /// <summary>
        /// 获得值
        /// </summary>
        /// <returns></returns>
        public ICollection<IChannel> GetValues()
        {
            return _Channels.Values;
        }

        /// <summary>
        /// 获得关键字
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetKeys()
        {
            return _Channels.Keys;
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainChannel(string key)
        {
            return _Channels.ContainsKey(key);
        }

        /// <summary>
        /// 删除通道
        /// </summary>
        /// <param name="key"></param>
        public bool RemoveChannel(string key)
        {
            IChannel channel;
            if (_Channels.TryRemove(key, out channel))
            {
                if (!channel.IsDisposed)
                {
                    try
                    {
                        channel.Close();
                        channel.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                        {
                            this.Logger.Error(true,"",ex);
                        }
                    }
                  
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除所有通道
        /// </summary>
        public void RemoveAllChannel()
        {
            //foreach (KeyValuePair<string, IChannel> kv in _Channels)
            //{
            //    if (!kv.Value.IsDisposed)
            //    {
            //        kv.Value.Close();
            //        kv.Value.Dispose();
            //    }
            //}

            Parallel.ForEach(_Channels, channel =>
            {
                try
                {
                    channel.Value.Close();
                    channel.Value.Dispose();
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                    {
                        this.Logger.Error(true, "", ex);
                    }
                }
            });

            _Channels.Clear();
        }


        public int ChannelCount
        {
            get { return _Channels.Count; }
        }

        public ILog Logger { get; private set; }
    }
}
