namespace SnkFramework.PatchBuilder.Runtime.Core
{
    [System.Serializable]
    public class SnkPatcher
    {
        /// <summary>
        /// 补丁包名字
        /// </summary>
        public string name;
        
        /// <summary>
        /// 补丁包版本
        /// </summary>
        public int version;

        /// <summary>
        /// 新增、更新的资源总个数
        /// </summary>
        public int sourceCount;
        
        /// <summary>
        /// 新增、更新的资源总大小
        /// </summary>
        public long totalSize;

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool isForce;
        
        /// <summary>
        /// 是否压缩
        /// </summary>
        public bool isCompress;
    }
}