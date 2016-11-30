using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Log;

namespace TestLog
{
    class Program
    {
        static void Main(string[] args)
        {
            new LogFactory().GetLog("ddd").Debug(true,"ddd");
            Console.Read();
        }
    }
}
