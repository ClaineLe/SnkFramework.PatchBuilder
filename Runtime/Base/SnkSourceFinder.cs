using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SnkFramework.PatchBuilder.Runtime.Base
{
    /// <summary>
    /// 资源探测器
    /// </summary>
    [System.Serializable]
    public class SnkSourceFinder : ISnkSourceFinder
    {
        /// <summary>
        /// 资源目录路径
        /// </summary>
        public string sourceDirPath;

        /// <summary>
        /// 筛选关键字
        /// </summary>
        public string[] filters;

        /// <summary>
        /// 忽略关键字
        /// </summary>
        public string[] ignores;

        /// <summary>
        /// 尝试探测资源
        /// </summary>
        /// <param name="fileInfos">构建出的资源文件信息</param>
        /// <param name="dirFullPath">资源目录的根路径</param>
        /// <returns>操作结果：true：成功， false：失败</returns>
        public bool TrySurvey(out FileInfo[] fileInfos, out string dirFullPath)
        {
            fileInfos = null;
            dirFullPath = String.Empty;
            DirectoryInfo dirInfo = new DirectoryInfo(sourceDirPath);
            if (dirInfo.Exists == false)
                return false;
            dirFullPath = dirInfo.Parent!.FullName;
            fileInfos = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            fileInfos = FiltersProcess(fileInfos);
            fileInfos = IgnoreProcess(fileInfos);
            return true;
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <returns></returns>
        private FileInfo[] FiltersProcess(FileInfo[] fileInfos)
        {
            if (filters == null || filters.Length == 0)
                return fileInfos;
            
            return (from fileInfo in fileInfos
                from filter in filters
                where fileInfo.FullName.Contains(filter)
                select fileInfo).ToArray();
        }

        /// <summary>
        /// 忽略
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <returns></returns>
        private FileInfo[] IgnoreProcess(FileInfo[] fileInfos)
        {
            if (ignores == null || ignores.Length == 0)
                return fileInfos;

            var list = new List<FileInfo>(fileInfos);
            list.RemoveAll(fileInfo =>
            {
                foreach (var ignore in ignores)
                {
                    if (fileInfo.FullName.Contains(ignore))
                        return true;
                }
                return false;
            });
            return list.ToArray();
        }
    }
}