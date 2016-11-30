using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Log
{
    public class ConsoleContainer:ILogContainer
    {
        public void ShowLog(string log)
        {
            Console.WriteLine(log);
        }
    }
}
