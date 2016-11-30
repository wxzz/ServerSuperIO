using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.COM;

namespace ServerSuperIO.Device
{
    internal static class DeviceExtansion
    {
        internal static IDeviceContainerItem GetDeviceContainerItem(this IRunDevice dev)
        {
            DeviceContainerItem item = new DeviceContainerItem();

            item.DeviceID = dev.DeviceParameter.DeviceID;
            item.DeviceCode = dev.DeviceParameter.DeviceCode;
            item.DeviceAddr = dev.DeviceParameter.DeviceAddr;
            item.DeviceName = dev.DeviceParameter.DeviceName;
            item.CommunicateType = dev.CommunicateType;
            if (dev.CommunicateType == CommunicateType.COM)
            {
                item.IoParameter1 = ComUtils.PortToString(dev.DeviceParameter.COM.Port);
                item.IoParameter2 = dev.DeviceParameter.COM.Baud;
            }
            else if (dev.CommunicateType == CommunicateType.NET)
            {
                item.IoParameter1 = dev.DeviceParameter.NET.RemoteIP;
                item.IoParameter2 = dev.DeviceParameter.NET.RemotePort;
            }
            item.ChannelState = dev.DeviceDynamic.ChannelState;
            item.CommunicateState = dev.DeviceDynamic.CommunicateState;
            item.RunState = dev.DeviceDynamic.RunState;
            try
            {
                item.AlertState = dev.GetAlertState();
            }
            catch 
            {
                //
            }

            item.DeviceType = dev.DeviceType;
            item.ModelNumber = dev.ModelNumber;
            return item;
        }
    }
}
