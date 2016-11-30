using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerSuperIO.Communicate;
using ServerSuperIO.Device;
using ServerSuperIO.Device.Connector;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;
using ServerSuperIO.Service.Connector;

namespace TransFileDriver
{
    public class ReceiveFileDriver:RunDevice
    {
        private Dynamic _Dyn;
        private Parameter _Parameter;
        private Protocol _Protocol;
        public ReceiveFileDriver() : base()
        {
            _Dyn = new Dynamic();
            _Parameter = new Parameter();
            _Protocol = new Protocol();
        }


        public override void Initialize(string devid)
        {
            this.Protocol.InitDriver(this.GetType(), new FixedHeadAndEndReceiveFliter(TransFileDriver.Protocol.Head, TransFileDriver.Protocol.End));
        }

        public override byte[] GetConstantCommand()
        {
            return null;
        }

        public override void Communicate(IRequestInfo info)
        {
            object obj = this.Protocol.DriverAnalysis<String,String>("writefile", info.Data, null,null);
            if (obj.ToString() == "0")
            {
                OnDeviceRuningLog("写入文件成功");
            }
            else
            {
                OnDeviceRuningLog("写入文件失败");
            }
        }

        public override void CommunicateInterrupt(IRequestInfo info)
        {
            OnDeviceRuningLog("通讯中断");
        }

        public override void CommunicateError(IRequestInfo info)
        {
            OnDeviceRuningLog("通讯干扰");
        }

        public override void CommunicateNone()
        {
            //throw new NotImplementedException();
        }

        public override void Alert()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

        public override void Show()
        {
            //throw new NotImplementedException();
        }

        public override void UnknownIO()
        {
            //throw new NotImplementedException();
        }

        public override void CommunicateStateChanged(CommunicateState comState)
        {
            //throw new NotImplementedException();
        }

        public override void ChannelStateChanged(ChannelState channelState)
        {
            //throw new NotImplementedException();
        }

        public override void Exit()
        {
            //throw new NotImplementedException();
        }

        public override void Delete()
        {
            //throw new NotImplementedException();
        }

        public override object GetObject()
        {
            throw new NotImplementedException();
        }

        public override string GetAlertState()
        {
            throw new NotImplementedException();
        }

        public override void ShowContextMenu()
        {
            //throw new NotImplementedException();
        }

        public override IDeviceDynamic DeviceDynamic {
            get { return _Dyn;
            }
        }
        public override IDeviceParameter DeviceParameter {
            get { return _Parameter; }
        }
        public override IProtocolDriver Protocol {
            get { return _Protocol; }
        }
        public override DeviceType DeviceType {
            get { return DeviceType.Common;}
        }
        public override string ModelNumber {
            get { return "TransFile"; }
        }
        public override object RunDeviceConnector(IFromDevice fromDevice, IDeviceToDevice toDevice)
        {
            throw new NotImplementedException();
        }

        public override void DeviceConnectorCallback(object obj)
        {
            throw new NotImplementedException();
        }

        public override void DeviceConnectorCallbackError(Exception ex)
        {
            throw new NotImplementedException();
        }

        public override object RunServiceConnector(IFromService fromService, IServiceToDevice toDevice)
        {
            throw new NotImplementedException();
        }
    }
}
