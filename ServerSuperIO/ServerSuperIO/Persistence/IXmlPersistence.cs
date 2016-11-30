using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Persistence
{
    public interface IXmlPersistence:IPersistence
    {
        /// <summary>
        /// 保存的路径
        /// </summary>
        string SavePath { get; }

        /// <summary>
        /// 修复持久化文件
        /// </summary>
        object Repair();
    }
}
