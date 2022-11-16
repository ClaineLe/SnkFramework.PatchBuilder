using System.IO;

namespace SnkFramework.PatchBuilder.Runtime.Base
{
    /// <summary>
    /// 资源探测器接口
    /// </summary>
    public interface ISnkSourceFinder
    {
        /// <summary>
        /// 尝试探测资源
        /// </summary>
        /// <param name="fileInfos">构建出的资源文件信息</param>
        /// <param name="dirFullPath">资源目录的根路径</param>
        /// <returns>操作结果：true：成功， false：失败</returns>
        public bool TrySurvey(out FileInfo[] fileInfos, out string dirFullPath);
    }
}