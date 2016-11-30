namespace ServerSuperIO.Device
{
    /// <summary>
    /// 发送数据事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void SendDataHandler(object source, SendDataArgs e);

    /// <summary>
    /// 更新运行监视器事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void DeviceRuningLogHandler(object source, DeviceRuningLogArgs e);

    /// <summary>
    /// 改变串口参数
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void ComParameterExchangeHandler(object source, ComParameterExchangeArgs e);

    /// <summary>
    /// 设备显示数据事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void DeviceObjectChangedHandler(object source, DeviceObjectChangedArgs e);

    /// <summary>
    /// 删除设备事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void DeleteDeviceHandler(object source, DeleteDeviceArgs e);

    /// <summary>
    /// 更新设备运行器事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void UpdateContainerHandler(object source, UpdateContainerArgs e);

}
