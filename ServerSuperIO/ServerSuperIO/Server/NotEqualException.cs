using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Server
{
    public class NotEqualException:Exception
    {
        public NotEqualException(string msg) : base(msg)
        {
            
        }
    }
}
