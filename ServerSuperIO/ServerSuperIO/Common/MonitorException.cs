using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ServerSuperIO.Log;

namespace ServerSuperIO.Common
{
    public class MonitorException
    {
        private ILog _log;
        public MonitorException()
        {
            _log=new Log.Log("monitorexception",null);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public void Monitor()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(MainThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        public void UnMonitor()
        {
            Application.ThreadException -= new ThreadExceptionEventHandler(MainThreadException);
            AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void MainThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                _log.Error(true, "", e.Exception);
            }
            catch 
            {
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                _log.Error(true, "", (Exception)e.ExceptionObject);
            }
            catch 
            {
            }
        }
    }
}
