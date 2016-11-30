using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Show
{
    /// <summary>
    /// 当单击右键触发这个事件
    /// </summary>
    /// <param name="devid"></param>
    public delegate void MouseRightContextMenuHandler(string devid);

    /// <summary>
    /// 显示视图，关闭事件委托
    /// </summary>
    /// <param name="key"></param>
    public delegate void GraphicsShowClosedHandler(object key);
}
