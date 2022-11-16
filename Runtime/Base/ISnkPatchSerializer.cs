namespace SnkFramework.PatchBuilder.Runtime.Base
{
    /// <summary>
    /// 补丁序列化接口
    /// </summary>
    public interface ISnkPatchSerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="target">需要序列化的目标对象</param>
        /// <returns>序列化后的字符串</returns>
        public string Serialize(object target);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="content">序列化的字符串</param>
        /// <typeparam name="T">转换目标对象的类型</typeparam>
        /// <returns>目标对象</returns>
        public T Deserialize<T>(string content);
    }
}