using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Common.Assembly
{
    public class AssemblyUtil
    {
        public static string GetAssemblyVersion(string path)
        {
            if (File.Exists(path))
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFile(path);
                return asm.GetName().Version.ToString();
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
