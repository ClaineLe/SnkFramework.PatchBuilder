namespace SnkFramework.PatchBuilder.Runtime
{
    public class SNK_BUILDER_CONST
    {
        /* 补丁包仓库路径 */
        public const string REPO_ROOT_PATH = "PatcherRepository";

        /* 设置文件文件名 */
        public const string SETTING_FILE_NAME = ".setting";
        
        /* 资源清单文件名 */
        public const string SOURCE_FILE_NAME = "source_manifest.json";
        
        /* 差异清单文件名 */
        public const string DIFF_FILE_NAME = "diff_manifest.json";
        
        /* 补丁包清单文件名 */
        public const string PATCHER_FILE_NAME = "patch_manifest.json";

        /* 补丁包名字格式 */
        public const string PATCHER_NAME_FORMATER = "patcher_{0}_{1}({2})";

    }
}