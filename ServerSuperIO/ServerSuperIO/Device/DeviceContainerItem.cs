using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Communicate;

namespace ServerSuperIO.Device
{
    public class DeviceContainerItem:IDeviceContainerItem
    {
        public DeviceContainerItem()
        { }
        public string DeviceID { get; set; }
        public string DeviceCode { get; set; }
        public int DeviceAddr { get; set; }
        public string DeviceName { get; set; }
        public CommunicateType CommunicateType { get; set; }
        public string IoParameter1 { get; set; }
        public int IoParameter2 { get; set; }
        public ChannelState ChannelState { get; set; }
        public CommunicateState CommunicateState { get; set; }
        public RunState RunState { get; set; }
        public string AlertState { get; set; }
        public DeviceType DeviceType { get; set; }
        public string ModelNumber { get; set; }
    }
}
