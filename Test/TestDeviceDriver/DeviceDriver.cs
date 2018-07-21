using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Device;
using ServerSuperIO.Device.Connector;
using ServerSuperIO.Persistence;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;
using ServerSuperIO.Service.Connector;
using ServerSuperIO.Data;

namespace TestDeviceDriver
{
    public class DeviceDriver : RunDevice
    {
        private DeviceProtocol _protocol;
        private ContextMenuComponent _contextMenuComponent;
        private string _channelKey;

        public DeviceDriver() : base()
        {
            _protocol = new DeviceProtocol();
            _contextMenuComponent=new ContextMenuComponent();
        }

        public override void Initialize(object obj)
        {
            base.Initialize(obj);
            
            //this.Protocol.InitDriver(this.GetType(), new FixedHeadAndEndReceiveFliter(new byte[] { 0x55, 0xaa }, new byte[] { 0x0d }));

            //this.Protocol.InitDriver(this, new FixedLengthReceiveFliter(2));

            //this.Protocol.InitDriver(this, new FixedHeadReceiveFliter(new byte[] { 0x55, 0xaa }));

            //this.Protocol.InitDriver(this, new FixedHeadAndLengthReceiveFliter(new byte[] { 0x55, 0xaa },12));

            // this.Protocol.InitDriver(this, new FixedEndReceiveFliter(new byte[] { 0x0d }));


            //this.Protocol.InitDriver(this, null);

            //初始化设备参数信息

            //_devicePara.DeviceID = devid;//设备的ID必须先赋值，因为要查找对应的参数文件。
            //_devicePara.Load();

            ////初始化设备实时数据信息
            //_deviceDyn.DeviceID = devid;//设备的ID必须先赋值，因为要查找对应的实时数据文件。
            //_deviceDyn.Load();

            this.DeviceDynamic.ChannelState = ChannelState.Close;
            this.DeviceDynamic.CommunicateState = CommunicateState.Interrupt;
        }

        public override IList<IRequestInfo> GetConstantCommand()
        {
            byte[] data = this.Protocol.DriverPackage<String>(this.DeviceParameter.DeviceCode, CommandArray.RealTimeData.ToString(), null);
            string hexs = BinaryUtil.ByteToHex(data);
            OnDeviceRuningLog("发送>>" + hexs);

            IList<IRequestInfo> cmdList = new List<IRequestInfo>();
            cmdList.Add(new RequestInfo(data,null));
            return cmdList;
        }

        public override void Communicate(ServerSuperIO.Communicate.IResponseInfo info)
        {

            string hexs = BinaryUtil.ByteToHex(info.Data);
            OnDeviceRuningLog("接收>>" + hexs);

            byte[] cmds = this.Protocol.GetCommand(info.Data);
            CommandArray cr = (CommandArray)cmds[0];

            dynamic obj = this.Protocol.DriverAnalysis<byte[]>(cr.ToString(), info.Data, info.BigData);
            if (obj != null)
            {
                if (cr == CommandArray.RealTimeData)
                {
                    Dyn dyn = (Dyn)obj;

                    this.DeviceDynamic.DynamicData.Write("flow",dyn.Flow);
                    this.DeviceDynamic.DynamicData.Write("signal",dyn.Signal);

                    CrossServerPublisher();

                    OnDeviceRuningLog("通讯正常，流量："+ dyn.Flow+",信号："+dyn.Signal);
                }
                else if (cr == CommandArray.FileData)
                {
                    OnDeviceRuningLog("文件存储路径：" + obj.ToString());
                }
                //else if (cr == CommandArray.BackControlCommand)
                //{
                //    if (_serviceCallback != null)
                //    {
                //        _serviceCallback.BeginInvoke(new string[] { this.DeviceParameter.DeviceCode, _channelKey }, null, null);
                //    }
                //}
            }
        }

        public override void CommunicateInterrupt(ServerSuperIO.Communicate.IResponseInfo info)
        {
            OnDeviceRuningLog("通讯中断");
        }

        public override void CommunicateError(ServerSuperIO.Communicate.IResponseInfo info)
        {
            //UDP
            //info.Channel.Write(System.Text.Encoding.ASCII.GetBytes("aaa"));
            OnDeviceRuningLog("通讯干扰");
        }

        public override void CommunicateNone()
        {
            OnDeviceRuningLog("通讯未知");
        }

        public void CrossServerPublisher()
        {
            object value1 = DeviceDynamic.DynamicData.Read("flow");
            object value2 = DeviceDynamic.DynamicData.Read("signal");

            List<ITag> tags = new List<ITag>();
            tags.Add(new Tag() { Timestamp=DateTime.Now,TagName="flow",TagValue=Convert.ToDouble(value1) });
            tags.Add(new Tag() { Timestamp = DateTime.Now, TagName = "signal", TagValue = Convert.ToDouble(value2) });

            this.DeviceDynamic.Save();
        }

        public override void UnknownIO()
        {
            OnDeviceRuningLog("未知通讯接口");
            OnUpdateContainer(null);
        }

        public override void CommunicateStateChanged(ServerSuperIO.Communicate.CommunicateState comState)
        {
            OnDeviceRuningLog("通讯状态改变");
        }

        public override void ChannelStateChanged(ServerSuperIO.Communicate.ChannelState channelState)
        {
            OnDeviceRuningLog("通道状态改变");
        }

        public override void Exit()
        {
            OnDeviceRuningLog("退出设备");
        }

        public override void Delete()
        {
            OnDeviceRuningLog("删除设备");
        }

        public override object GetObject()
        {
            throw new NotImplementedException();
        }

        public override void ShowContextMenu()
        {
            this._contextMenuComponent.ContextMenuStrip.Show(Cursor.Position);
        }

        public override IProtocolDriver Protocol
        {
            get
            {
                return _protocol;
            }
        }

        public override DeviceType DeviceType
        {
            get { return DeviceType.Common; }
        }

        public override string ModelNumber
        {
            get { return "serversuperio"; }
        }
        public override IDeviceConnectorCallbackResult RunDeviceConnector(IFromDevice fromDevice, IDeviceToDevice toDevice, AsyncDeviceConnectorCallback callback)
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
       
        public override IServiceConnectorCallbackResult RunServiceConnector(IFromService fromService, IServiceToDevice toDevice,AsyncServiceConnectorCallback callback)
        {
            throw new NotImplementedException();
        }
    }
}
