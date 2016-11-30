using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServerSuperIO.Base;

namespace ServerSuperIO.Show
{
    public interface IGraphicsShow : IPlugin
    {
        /// <summary>
        ///     服务Key,要求唯一
        /// </summary>
        string ShowKey { get;}
        
        /// <summary>
        ///     服务名称
        /// </summary>
        string ShowName { get; }

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="windows"></param>
        void ShowGraphics(IWin32Window windows);

        /// <summary>
        /// 
        /// </summary>
        void CloseGraphics();
        /// <summary>
        ///     更新设备
        /// </summary>
        /// <param name="devCode">设备ID</param>
        /// <param name="obj">设备对象</param>
        void UpdateDevice(string devCode, object obj);

        /// <summary>
        ///     移除设备
        /// </summary>
        /// <param name="devCode">设备ID</param>
        void RemoveDevice(string devCode);

        /// <summary>
        ///     关闭窗体事件时发生
        /// </summary>
        event GraphicsShowClosedHandler GraphicsShowClosed;

        /// <summary>
        ///     单击右键
        /// </summary>
        event MouseRightContextMenuHandler MouseRightContextMenu;

        /// <summary>
        /// 是否被释放
        /// </summary>
        bool IsDisposed { get; }
    }
}
