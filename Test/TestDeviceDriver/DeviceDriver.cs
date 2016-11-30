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
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;
using ServerSuperIO.Service.Connector;

namespace TestDeviceDriver
{
    public class DeviceDriver : RunDevice
    {
        private DeviceDyn _deviceDyn;
        private DevicePara _devicePara;
        private DeviceProtocol _protocol;
        private ContextMenuComponent _contextMenuComponent;
        public DeviceDriver() : base()
        {
            _devicePara = new DevicePara();
            _deviceDyn = new DeviceDyn();
            _protocol = new DeviceProtocol();
            _contextMenuComponent=new ContextMenuComponent();
        }

        public override void Initialize(string devid)
        {
            this.Protocol.InitDriver(this.GetType(), new FixedHeadAndEndReceiveFliter(new byte[] { 0x55, 0xaa }, new byte[] { 0x0d }));

            //this.Protocol.InitDriver(this, new FixedLengthReceiveFliter(2));

            //this.Protocol.InitDriver(this, new FixedHeadReceiveFliter(new byte[] { 0x55, 0xaa }));

            //this.Protocol.InitDriver(this, new FixedHeadAndLengthReceiveFliter(new byte[] { 0x55, 0xaa },12));

            // this.Protocol.InitDriver(this, new FixedEndReceiveFliter(new byte[] { 0x0d }));


            //this.Protocol.InitDriver(this, null);

            //初始化设备参数信息

            _devicePara.DeviceID = devid;//设备的ID必须先赋值，因为要查找对应的参数文件。
            if (System.IO.File.Exists(_devicePara.SavePath))
            {
                //如果参数文件存在，则获得参数实例
                _devicePara = _devicePara.Load<DevicePara>();
            }
            else
            {
                //如果参数文件不存在，则序列化一个文件
                _devicePara.Save<DevicePara>(_devicePara);
            }

            //初始化设备实时数据信息
            _deviceDyn.DeviceID = devid;//设备的ID必须先赋值，因为要查找对应的实时数据文件。
            if (System.IO.File.Exists(_deviceDyn.SavePath))
            {
                //如果参数文件存在，则获得参数实例
                _deviceDyn = _deviceDyn.Load<DeviceDyn>();
            }
            else
            {
                //如果参数文件不存在，则序列化一个文件
                _deviceDyn.Save<DeviceDyn>(_deviceDyn);
            }

            this.DeviceDynamic.ChannelState = ChannelState.Close;
            this.DeviceDynamic.CommunicateState = CommunicateState.Interrupt;
        }

        public override byte[] GetConstantCommand()
        {
            byte[] data = this.Protocol.DriverPackage<String>(this.DeviceParameter.DeviceCode, CommandArray.RealTimeData.ToString(), null);
            string hexs = BinaryUtil.ByteToHex(data);
            OnDeviceRuningLog("发送>>" + hexs);
            return data;
        }

        public override void Communicate(ServerSuperIO.Communicate.IRequestInfo info)
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
                    _deviceDyn.Dyn = (Dyn)obj;
                    OnDeviceRuningLog("通讯正常");
                    if (this.DeviceParameter.DeviceCode != "1")
                    {
                        Console.WriteLine(">>>>模拟控制命令开始");
                        this.OnDeviceConnector(
                            new FromDevice(this.DeviceParameter.DeviceID, this.DeviceParameter.DeviceCode,
                                this.DeviceParameter.DeviceAddr, this.DeviceParameter.DeviceName),
                            new DeviceToDevice("1", this.DeviceParameter.DeviceName + "问：大哥，朴大妈为什么还不下课？", null, null));
                    }
                }
                else if (cr == CommandArray.FileData)
                {
                    OnDeviceRuningLog("文件存储路径：" + obj.ToString());
                }
            }
        }

        public override void CommunicateInterrupt(ServerSuperIO.Communicate.IRequestInfo info)
        {
            OnDeviceRuningLog("通讯中断");
        }

        public override void CommunicateError(ServerSuperIO.Communicate.IRequestInfo info)
        {
            //UDP
            //info.Channel.Write(System.Text.Encoding.ASCII.GetBytes("aaa"));
            OnDeviceRuningLog("通讯干扰");
        }

        public override void CommunicateNone()
        {
            OnDeviceRuningLog("通讯未知");
        }

        public override void Alert()
        {
            return;
        }

        public override void Save()
        {
            try
            {
                _deviceDyn.Save<DeviceDyn>(_deviceDyn);
            }
            catch (Exception ex)
            {
                OnDeviceRuningLog(ex.Message);
            }
        }

        public override void Show()
        {
            OnUpdateContainer(null);
            List<string> list = new List<string>();
            list.Add(_devicePara.DeviceCode);
            list.Add(_devicePara.DeviceName);
            list.Add(_deviceDyn.Dyn.Flow.ToString());
            list.Add(_deviceDyn.Dyn.Signal.ToString());
            OnDeviceObjectChanged(list.ToArray());
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

        public override string GetAlertState()
        {
            return "";
        }

        public override void ShowContextMenu()
        {
            this._contextMenuComponent.ContextMenuStrip.Show(Cursor.Position);
        }

        public override IDeviceDynamic DeviceDynamic
        {
            get { return _deviceDyn; }
        }

        public override IDeviceParameter DeviceParameter
        {
            get { return _devicePara; }
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


        public override object RunDeviceConnector(IFromDevice fromDevice, IDeviceToDevice toDevice)
        {
            Console.WriteLine(toDevice.Text);//输出其他设备传来的数据。
            return this.DeviceParameter.DeviceName + "答：你不觉得这才是真正的韩剧吗？傻小子";
        }

        public override void DeviceConnectorCallback(object obj)
        {
            Console.WriteLine(obj.ToString());//输出返回结果
            Console.WriteLine(this.DeviceParameter.DeviceName+ "说：奥黑也真够坑爹的！");
            Console.WriteLine(">>>>模拟控制命令结束");
        }

        public override void DeviceConnectorCallbackError(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        public override object RunServiceConnector(IFromService fromService, IServiceToDevice toDevice)
        {
            Console.WriteLine(this.DeviceParameter.DeviceName+",接收到云端指令:"+toDevice.Text);
            return this.DeviceParameter.DeviceName+",执行完成";
        }
    }
}
