using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ServerSuperIO.Common;

namespace ServerSuperIO.Config
{
    public class GlobalConfigTool
    {
        private static readonly string _GlobalConfigPath = AppDomain.CurrentDomain.BaseDirectory + "ServerSuperIO/GlobalConfig.cfg";

        private static readonly object SyncObject = new object();

        private static GlobalConfig _GlobalConfig = null;

        static GlobalConfigTool()
        {
            string parentDir = System.IO.Path.GetDirectoryName(_GlobalConfigPath);
            if (!String.IsNullOrEmpty(parentDir))
            {
                if (!System.IO.Directory.Exists(parentDir))
                {
                    System.IO.Directory.CreateDirectory(parentDir);
                }
            }
        }

        public static GlobalConfig GlobalConfig
        {
            get
            {
                if (_GlobalConfig == null)
                {
                    lock (SyncObject)
                    {
                        Load();
                    }
                }
                return _GlobalConfig;
            }
        }

        protected static string GlobalConfigPath
        {
            get
            {
                return _GlobalConfigPath;
            }
        }

        public static void Save(GlobalConfig gc)
        {
            Save(_GlobalConfigPath, gc);
        }

        protected static void Save(string path, GlobalConfig gc)
        {
            SerializeUtil.XmlSerialize(path, gc);
        }

        private static void Load()
        {
            if (_GlobalConfig == null)
            {
                ReLoad();
            }
        }

        public static void ReLoad()
        {
            lock (SyncObject)
            {
                if (!FileUtil.IsExists(_GlobalConfigPath))
                {
                    Save(new GlobalConfig());
                }

                _GlobalConfig = SerializeUtil.XmlDeserailize<GlobalConfig>(_GlobalConfigPath);
            }
        }
    }
}
