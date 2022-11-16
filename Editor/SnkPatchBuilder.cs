using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using SnkFramework.PatchBuilder.Runtime;
using SnkFramework.PatchBuilder.Runtime.Base;
using SnkFramework.PatchBuilder.Runtime.Core;

namespace SnkFramework.PatchBuilder
{
    namespace Editor
    {
        /// <summary>
        /// 渠道补丁包构建器
        /// </summary>
        public class SnkPatchBuilder
        {
            /// <summary>
            /// 渠道名
            /// </summary>
            private readonly string _channelName;
            
            /// <summary>
            /// 设置文件
            /// </summary>
            private readonly SnkBuilderSettings _settings;
            
            /// <summary>
            /// 补丁包清单
            /// </summary>
            private readonly SnkPatcherManifest _patcherManifest;
            
            /// <summary>
            /// 最新资源列表
            /// </summary>
            private List<SnkSourceInfo> _lastSourceInfoList;

            /// <summary>
            /// 渠道仓库根目录
            /// </summary>
            private string mChannelRepoPath => Path.Combine(SNK_BUILDER_CONST.REPO_ROOT_PATH, _channelName);

            /// <summary>
            /// 序列化器
            /// </summary>
            private ISnkPatchSerializer _serializer;
            
            /// <summary>
            /// 序列化器(访问起)
            /// </summary>
            private ISnkPatchSerializer mSerializer => _serializer ?? new SnkPatchSerializer();
            
            /// <summary>
            /// 压缩器
            /// </summary>
            private ISnkPatchCompressor _compressor;
            private ISnkPatchCompressor mCompressor=> _compressor ?? new SnkPatchCompressor();
         
            /// <summary>
            /// 从渠道名加载渠道补丁包构建器
            /// </summary>
            /// <param name="channelName">渠道名字</param>
            /// <typeparam name="TSerializer">序列器</typeparam>
            /// <returns>补丁包构建器</returns>
            public static SnkPatchBuilder Load(string channelName)
            {
                SnkPatchBuilder builder = new SnkPatchBuilder(channelName);
                return builder;
            } 
            
            /// <summary>
            /// 构建方法
            /// </summary>
            /// <param name="channelName"></param>
            private SnkPatchBuilder(string channelName)
            {
                this._channelName = channelName;

                this._settings = LoadSettings();
                this._patcherManifest = LoadPatcherManifest();
                this._lastSourceInfoList = LoadLastSourceInfoList();
            }

            /// <summary>
            /// 安装序列器
            /// </summary>
            /// <typeparam name="TSerializer">序列器类型</typeparam>
            public void SetupSerializer<TSerializer>() where TSerializer : class, ISnkPatchSerializer, new()
                => this._serializer = new TSerializer();
            
            /// <summary>
            /// 安装压缩器
            /// </summary>
            /// <typeparam name="TCompressor">压缩器类型</typeparam>
            public void SetupCompressor<TCompressor>() where TCompressor : class, ISnkPatchCompressor, new()
                => this._compressor = new TCompressor();
            

            /// <summary>
            /// 加载设置文件
            /// </summary>
            /// <returns>设置文件</returns>
            private SnkBuilderSettings LoadSettings()
            {
                var fileInfo = new FileInfo(Path.Combine(mChannelRepoPath, SNK_BUILDER_CONST.SETTING_FILE_NAME));
                if (fileInfo.Exists == false)
                    this.SaveSettings(new SnkBuilderSettings());
                string jsonString = File.ReadAllText(fileInfo.FullName);
                return this.mSerializer.Deserialize<SnkBuilderSettings>(jsonString);
            }

            /// <summary>
            /// 保存设置文件
            /// </summary>
            /// <param name="settings">设置文件</param>
            private void SaveSettings(SnkBuilderSettings settings)
            {
                var fileInfo = new FileInfo(Path.Combine(mChannelRepoPath, SNK_BUILDER_CONST.SETTING_FILE_NAME));
                if (fileInfo.Directory!.Exists == false)
                    fileInfo.Directory.Create();
                File.WriteAllText(fileInfo.FullName, mSerializer.Serialize(settings));
            }

            /// <summary>
            /// 加载补丁包清单
            /// </summary>
            /// <returns></returns>
            private SnkPatcherManifest LoadPatcherManifest()
            {
                var fileInfo = new FileInfo(Path.Combine(mChannelRepoPath, SNK_BUILDER_CONST.PATCHER_FILE_NAME));
                if (fileInfo.Exists == false)
                    return new SnkPatcherManifest();
                string jsonString = File.ReadAllText(fileInfo.FullName);
                return this.mSerializer.Deserialize<SnkPatcherManifest>(jsonString);
            }

            /// <summary>
            /// 保存补丁包清单
            /// </summary>
            /// <param name="patcherManifest"></param>
            private void SavePatcherManifest(SnkPatcherManifest patcherManifest)
            {
                string patcherManifestPath = Path.Combine(mChannelRepoPath, SNK_BUILDER_CONST.PATCHER_FILE_NAME);
                File.WriteAllText(patcherManifestPath, this.mSerializer.Serialize(patcherManifest));
            }

            /// <summary>
            /// 加载最新资源信息列表
            /// </summary>
            /// <returns>最新资源信息列表</returns>
            private List<SnkSourceInfo> LoadLastSourceInfoList()
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(mChannelRepoPath, SNK_BUILDER_CONST.SOURCE_FILE_NAME));
                if (fileInfo.Exists == false)
                    return null;
                string[] lines = File.ReadAllLines(fileInfo.FullName);
                List<SnkSourceInfo> list = new List<SnkSourceInfo>();
                foreach (var lineString in lines)
                {
                    if (string.IsNullOrEmpty(lineString))
                        continue;
                    var sourceInfo = SnkSourceInfo.Parse(lineString);
                    list.Add(sourceInfo);
                }

                return list;
            }

            /// <summary>
            /// 保存最新资源信息列表
            /// </summary>
            /// <param name="sourceInfoList">最新资源信息列表</param>
            private void SaveLastSourceInfoList(List<SnkSourceInfo> sourceInfoList)
            {
                string manifestPath = Path.Combine(mChannelRepoPath, SNK_BUILDER_CONST.SOURCE_FILE_NAME);
                string[] lines = new string[sourceInfoList.Count];
                for (int i = 0; i < sourceInfoList.Count; i++)
                    lines[i] = sourceInfoList[i].ToString();
                File.WriteAllLines(manifestPath, lines);
            }

            /// <summary>
            /// 生成资源信息列表
            /// </summary>
            /// <param name="version">信息列表的版本号</param>
            /// <param name="sourceFinder">资源探测器</param>
            /// <returns>资源信息列表</returns>
            private List<SnkSourceInfo> GenerateSourceInfoList(int version, ISnkSourceFinder sourceFinder)
            {
                List<SnkSourceInfo> sourceInfoList = new List<SnkSourceInfo>();

                bool result = sourceFinder.TrySurvey(out var fileInfos,  out var dirFullPath);
                if(result == false)
                    return null;

                foreach (var fileInfo in fileInfos)
                {
                    var sourceInfo = new SnkSourceInfo();
                    sourceInfo.name = fileInfo.FullName.Replace(dirFullPath, string.Empty).Substring(1);
                    sourceInfo.version = version;
                    sourceInfo.md5 = fileInfo.FullName.GetHashCode().ToString();
                    sourceInfo.size = fileInfo.Length;
                    sourceInfoList.Add(sourceInfo);
                }

                return sourceInfoList;
            }

            /// <summary>
            /// 生成资源差异清单
            /// </summary>
            /// <param name="prevSourceInfoList">上一个版本的资源信息列表</param>
            /// <param name="currSourceInfoList">当前版本的资源列表</param>
            /// <returns>资源差异清单</returns>
            private SnkDiffManifest GenerateDiffManifest(List<SnkSourceInfo> prevSourceInfoList, List<SnkSourceInfo> currSourceInfoList)
            {
                if (prevSourceInfoList == null)
                    return null;
                SourceInfoComparer comparer = new SourceInfoComparer();

                var diffManifest = new SnkDiffManifest();
                diffManifest.delList = prevSourceInfoList.Except(currSourceInfoList, comparer).Select(a => a.name).ToList();
                diffManifest.addList = currSourceInfoList.Except(prevSourceInfoList, comparer).ToList();
                return diffManifest;
            }

            /// <summary>
            /// 复制资源文件到指定目录下
            /// </summary>
            /// <param name="toDirectoryFullPath">目标目录的绝对路径</param>
            /// <param name="sourceInfoList">需要复制的资源信息列表</param>
            private void CopySourceTo(string toDirectoryFullPath, List<SnkSourceInfo> sourceInfoList)
            {
                foreach (var sourceInfo in sourceInfoList)
                {
                    var fromFileInfo = new FileInfo(sourceInfo.name);
                    var toFileInfo = new FileInfo(Path.Combine(toDirectoryFullPath, sourceInfo.name));
                    if (toFileInfo.Directory!.Exists == false)
                        toFileInfo.Directory.Create();
                    fromFileInfo.CopyTo(toFileInfo.FullName);
                }
            }

            /// <summary>
            /// 构建
            /// </summary>
            /// <param name="sourceFinderList">资源探测器列表</param>
            /// <param name="isForce">是否是强更补丁包</param>
            /// <param name="isCompress">是否进行压缩</param>
            /// <returns>补丁包信息</returns>
            public SnkPatcher Build(IEnumerable<ISnkSourceFinder> sourceFinderList, bool isForce = false, bool isCompress = false)
            {
                var prevVersion = _patcherManifest.lastVersion;
                var currVersion = prevVersion + 1;

                var patcher = new SnkPatcher();
                patcher.version = currVersion;
                patcher.isForce = isForce;
                patcher.isCompress = isCompress;

                //生成当前目标目录的资源信息列表
                var currSourceInfoList = new List<SnkSourceInfo>();
                foreach (var sourcePath in sourceFinderList)
                {
                    var list = GenerateSourceInfoList(currVersion, sourcePath);
                    if (list == null)
                        continue;
                    currSourceInfoList.AddRange(list);
                }

                //拼接补丁包名字
                patcher.name = string.Format(SNK_BUILDER_CONST.PATCHER_NAME_FORMATER, prevVersion,currVersion, ++_settings.buildNum);
                string currPatcherPath = Path.Combine(mChannelRepoPath, patcher.name);

                List<SnkSourceInfo> willMoveSourceList;
                if (_lastSourceInfoList == null)
                {
                    //生成初始资源包（拓展包）
                    _lastSourceInfoList = new List<SnkSourceInfo>();
                    _lastSourceInfoList = currSourceInfoList;
                    willMoveSourceList = currSourceInfoList;
                }
                else
                {
                    //生成差异列表
                    var diffManifest = GenerateDiffManifest(_lastSourceInfoList, currSourceInfoList);

                    //移除所有变化的资源
                    _lastSourceInfoList.RemoveAll(a => diffManifest.addList.Exists(b => a.name == b.name));
                    _lastSourceInfoList.RemoveAll(a => diffManifest.delList.Exists(b => a.name == b));

                    //添加新增或者更新的资源
                    _lastSourceInfoList.AddRange(diffManifest.addList);

                    //将要复制的资源
                    willMoveSourceList = diffManifest.addList;

                    //保存差异清单
                    var diffManifestFileInfo = new FileInfo(Path.Combine(currPatcherPath, SNK_BUILDER_CONST.DIFF_FILE_NAME));
                    if (diffManifestFileInfo.Directory!.Exists == false)
                        diffManifestFileInfo.Directory.Create();
                    File.WriteAllText(diffManifestFileInfo.FullName, this.mSerializer.Serialize(diffManifest));
                }
                
                //保存最新的资源清单
                this.SaveLastSourceInfoList(_lastSourceInfoList);
                
                //复制资源文件
                CopySourceTo(currPatcherPath, willMoveSourceList);
                patcher.sourceCount = willMoveSourceList.Count;
                
                //压缩
                if (patcher.isCompress && mCompressor != null)
                {
                    FileInfo zipFileInfo = new FileInfo(currPatcherPath + ".zip");
                    mCompressor.Compress(currPatcherPath, zipFileInfo.FullName, CompressionLevel.Optimal, true);
                    Directory.Delete(currPatcherPath, true);
                    patcher.totalSize = zipFileInfo.Length;
                }
                else
                {
                    patcher.totalSize = willMoveSourceList.Sum(a => a.size);
                }

                //刷新补丁包列表
                _patcherManifest.lastVersion = currVersion;
                _patcherManifest.lastPatcherName = patcher.name;
                _patcherManifest.patcherList.Add(patcher);
                this.SavePatcherManifest(_patcherManifest);

                //保存设置文件
                this.SaveSettings(_settings);
                return patcher;
            }
        }
    }
}
