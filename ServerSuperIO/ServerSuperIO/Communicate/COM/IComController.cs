using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Communicate.COM
{
    internal interface IComController:IController
    {
        IComSession ComChannel { get; set; }
    }
}
