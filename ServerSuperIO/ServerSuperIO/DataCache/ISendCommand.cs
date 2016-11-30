using System;

namespace ServerSuperIO.DataCache
{
    public interface ISendCommand
    {
        /// <summary>
        /// 命令字节
        /// </summary>
        byte[] Bytes { get; }

        /// <summary>
        /// 命令关键字
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 命令优先级
        /// </summary>
        Priority Priority { get; }
    }
}
