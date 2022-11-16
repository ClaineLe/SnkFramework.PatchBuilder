using System.Collections.Generic;

namespace SnkFramework.PatchBuilder.Runtime.Core
{
    /// <summary>
    /// 补丁包清单
    /// </summary>
    [System.Serializable]
    public class SnkPatcherManifest
    {
        /// <summary>
        /// 最新版本号
        /// </summary>
        public int lastVersion;
        
        /// <summary>
        /// 最新补丁包名字
        /// </summary>
        public string lastPatcherName;
        
        /// <summary>
        /// 补丁包列表
        /// </summary>
        public List<SnkPatcher> patcherList = new List<SnkPatcher>();
    }
}