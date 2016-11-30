namespace ServerSuperIO.Common.Assembly
{
    /// <summary>
    /// 根据类型名称生成类型实例
    /// </summary>
    public interface IObjectBuilder
    {
        /// <summary>
        /// 创建类型实例
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="args">构造参数</param>
        /// <returns>指定类型 T 的实例</returns>
        T BuildUp<T>(object[] args);

        /// <summary>
        /// 创建类型实例
        /// <remarks>该方法仅适用于有无参构造函数的的类型</remarks>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T BuildUp<T>() where T : new();

        /// <summary>
        /// 按照目标返回的类型，加工指定类型名称对应的类型实例。
        /// 目标类型可以为接口、抽象类等抽象类型，typeName一般为实体类名称。
        /// </summary>
        /// <typeparam name="T">目标返回的类型。</typeparam>
        /// <param name="typeName">类型名称<</param>
        /// <returns>按照目标返回类型加工好的一个实例</returns>
        T BuildUp<T>(string typeName);

        /// <summary>
        /// 按照目标类型，通过调用指定名称类型的构造函数，生成目标类型实例。
        /// </summary>
        /// <typeparam name="T">目标返回的类型</typeparam>
        /// <param name="typeName">类型名称</param>
        /// <param name="args">构造函数参数</param>
        /// <returns>按照目标返回类型加工好的一个实例</returns>
        T BuildUp<T>(string typeName, object[] args);

        /// <summary>
        /// 指定程序集名称和实例的名称，动态创建实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyname"></param>
        /// <param name="instancename"></param>
        /// <returns></returns>
        T BuildUp<T>(string assemblyname, string instancename);
    }
}
