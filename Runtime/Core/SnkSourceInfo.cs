using System.Collections.Generic;

namespace SnkFramework.PatchBuilder.Runtime.Core
{
    /// <summary>
    /// 资源信息
    /// </summary>
    [System.Serializable]
    public class SnkSourceInfo
    {
        /// <summary>
        /// 资源名（相对路径）
        /// </summary>
        public string name;
        
        /// <summary>
        /// 资源大小
        /// </summary>
        public long size;
        
        /// <summary>
        /// 资源版本
        /// </summary>
        public int version;
        
        /// <summary>
        /// 资源MD5
        /// </summary>
        public string md5;

        /// <summary>
        /// 从字符串解析出资源信息对象（SourceInfo）
        /// </summary>
        /// <param name="content">资源对象的序列化字符串</param>
        /// <returns>资源对象</returns>
        public static SnkSourceInfo Parse(string content)
        {
            var strings = content.Trim().Split(",");
            if (strings.Length != 4)
                throw new System.Exception("解析SourceInfo失败. content:" + content);

            var sourceInfo = new SnkSourceInfo();
            sourceInfo.name = strings[0];
            sourceInfo.size = long.Parse(strings[1]);
            sourceInfo.version = int.Parse(strings[2]);
            sourceInfo.md5 = strings[3];
            return sourceInfo;
        }

        public override string ToString()
            => string.Format($"{name},{size},{version},{md5}");
    }
    
    /// <summary>
    /// 资源信息比较器
    /// </summary>
    public class SourceInfoComparer : IEqualityComparer<SnkSourceInfo>
    {
        public bool Equals(SnkSourceInfo x, SnkSourceInfo y)
            => y != null && x != null && x.name == y.name && x.md5 == y.md5;

        public int GetHashCode(SnkSourceInfo obj) => obj.GetType().GetHashCode();
    }
}