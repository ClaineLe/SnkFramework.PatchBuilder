using System.Collections.Generic;

namespace SnkFramework.PatchBuilder.Runtime.Core
{
    /// <summary>
    /// 资源差异清单
    /// </summary>
    [System.Serializable]
    public class SnkDiffManifest
    {
        /// <summary>
        /// 新增、更新的资源列表
        /// </summary>
        public List<SnkSourceInfo> addList;
        
        /// <summary>
        /// 删除的资源名列表
        /// </summary>
        public List<string> delList;
    }
}