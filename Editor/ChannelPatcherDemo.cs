using SnkFramework.PatchBuilder.Editor;
using SnkFramework.PatchBuilder.Runtime.Base;
using UnityEditor;

namespace SnkFramework.PatchBuilder
{
    namespace Demo
    {
        static public class ChannelPatcherDemo
        {
            [MenuItem("SnkPatcher/Demo")]
            public static void Test()
            {
                string repoName = "windf_iOS";
                SnkPatchBuilder snkPatcher;

                var sourcePaths = new SnkSourceFinder[]
                {
                    new() 
                        { 
                            sourceDirPath = "ProjectSettings", 
                            //filters = new [] {"FSTimeGet"},
                            //ignores = new [] {"aebe", "407"},
                        },
                };

                snkPatcher = SnkPatchBuilder.Load(repoName);
                var patcher = snkPatcher.Build(sourcePaths, false, true);
            }
        }
    }
}
