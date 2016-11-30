using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Service.Connector;

namespace ServerSuperIO.Service
{
    public abstract class Service:IService
    {
        protected Service()
        {
            
        }

        public abstract string ServiceKey { get;}

        public abstract string ServiceName { get; }

        public bool IsAutoStart { get; set; }

        public abstract void OnClick();

        public abstract void UpdateDevice(string devCode, object obj);

        public abstract void RemoveDevice(string devCode);

        public abstract void StartService();

        public abstract void StopService();

        public event ServiceLogHandler ServiceLog;

        protected void OnServiceLog(string log)
        {
            if (ServiceLog != null)
            {
                ServiceLog(this.ServiceName+","+log);
            }
        }

        public bool IsDisposed { get; protected set; }

        public virtual void Dispose()
        {
            ServiceConnector = null;
            ServiceLog = null;
            IsDisposed = true;
        }

        public abstract void ServiceConnectorCallback(object obj);

        public abstract void ServiceConnectorCallbackError(Exception ex);

        public event ServiceConnectorHandler ServiceConnector;
        public void OnServiceConnector(IFromService fromService, IServiceToDevice toDevice)
        {
            if (ServiceConnector == null) return;

            ServiceConnector(this,new ServiceConnectorArgs(fromService,toDevice));
        }
    }
}
