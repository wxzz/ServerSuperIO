using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerSuperIO.Device;

namespace TransFileDriver
{
    public class Command : ProtocolCommand
    {
        public Command()
        {
        }
        public override string Name
        {
            get { return "writefile"; }
        }

        public override void ExcuteCommand<T>(T t)
        {
            throw new NotImplementedException();
        }

        public override dynamic Analysis<T1,T2>(byte[] data, T1 t1,T2 t2)
        {
            try
            {
               //count += data.Length - 24;
               //Console.WriteLine(count.ToString()+","+data[0].ToString() + "," + data[data.Length - 1].ToString());
                
                string path = Path.Combine(Environment.CurrentDirectory, "rev");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                string fileName = System.Text.Encoding.ASCII.GetString(data, 6, 16);
                path=Path.Combine(path, fileName);
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    fs.Seek(fs.Length, SeekOrigin.Current);
                    byte[] content = new byte[data.Length - 24];
                    Buffer.BlockCopy(data, 22, content, 0, content.Length);
                    fs.Write(content, 0, content.Length);
                    fs.Flush();
                }

            }
            catch
            {
                return -1;
            }
            return 0;
        }

        public override byte[] Package<T1,T2>(string code, T1 t1,T2 t2)
        {
            throw new NotImplementedException();
        }

        public override void ExcuteCommand<T1, T2>(T1 t1, T2 t2)
        {
            throw new NotImplementedException();
        }

        public override dynamic Analysis<T>(byte[] data, T t)
        {
            throw new NotImplementedException();
        }

        public override byte[] Package<T>(string code, T t)
        {
            throw new NotImplementedException();
        }
    }
}
