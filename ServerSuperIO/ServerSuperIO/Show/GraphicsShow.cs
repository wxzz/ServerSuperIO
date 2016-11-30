using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServerSuperIO.Show
{
    public abstract class GraphicsShow:IGraphicsShow
    {
        protected GraphicsShow()
        {
        }

        public abstract string ShowKey { get;  }

        public abstract string ShowName { get; }

        public abstract void ShowGraphics(IWin32Window windows);

        public abstract void CloseGraphics();

        public abstract void UpdateDevice(string devCode, object obj);

        public abstract void RemoveDevice(string devCode);

        public event GraphicsShowClosedHandler GraphicsShowClosed;

        protected void OnGraphicsShowClosed(object key)
        {
            if (GraphicsShowClosed != null)
            {
                GraphicsShowClosed(key);
            }
        }

        public event MouseRightContextMenuHandler MouseRightContextMenu;

        protected void OnMouseRightContextMenu(string devCode)
        {
            if (MouseRightContextMenu != null)
            {
                MouseRightContextMenu(devCode);
            }
        }

        public bool IsDisposed { get; protected set; }

        public virtual void Dispose()
        {
            GraphicsShowClosed = null;
            MouseRightContextMenu = null;
            IsDisposed = true;
        }
    }
}
