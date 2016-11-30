using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Show;

namespace ServerSuperIO.Device
{
    public interface IDeviceContainer:IDisposable
    {
        /// <summary>
        /// 查找设备
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        object FindDevice(string devid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="item"></param>
        void UpdateDevice(string devid, IDeviceContainerItem item);

        /// <summary>
        /// 移除设备
        /// </summary>
        /// <param name="devid"></param>
        void RemoveDevice(string devid);

        /// <summary>
        /// 移除所有设备
        /// </summary>
        void RemoveAllDevice();

        /// <summary>
        /// 单击右键
        /// </summary>
        event MouseRightContextMenuHandler MouseRightContextMenu;
    }
}
