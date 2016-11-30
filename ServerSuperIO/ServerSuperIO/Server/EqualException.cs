using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Server
{
    public class EqualException : Exception
    {
        public EqualException(string msg) : base(msg)
        {

        }
    }
}
