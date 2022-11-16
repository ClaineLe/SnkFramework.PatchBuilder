using System.IO.Compression;

namespace SnkFramework.PatchBuilder.Runtime.Base
{
    /// <summary>
    /// 压缩器实现
    /// </summary>
    public sealed class SnkPatchCompressor : ISnkPatchCompressor
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="sourceDirectoryName">源目录名</param>
        /// <param name="destinationArchiveFileName">压缩文件名</param>
        /// <param name="compressionLevel">压缩等级</param>
        /// <param name="includeBaseDirectory">是否包含基础目录</param>
        public void Compress(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory)
        {
            ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, compressionLevel, includeBaseDirectory);
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="sourceArchiveFileName">源压缩文件名</param>
        /// <param name="destinationDirectoryName">解压目录名</param>
        public void Decompress(string sourceArchiveFileName, string destinationDirectoryName)
        {
            ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
        }
    }
}