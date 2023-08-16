using Microsoft.Extensions.Options;
using System.IO;
using Z.FileService.Domain;

namespace Z.FileService.Infrastructure.Services
{
    /// <summary>
    /// 用局域网内共享文件夹或者本机磁盘当备份服务器的实现类
    /// </summary>
    public class SMBStorageClient : IStorageClient
    {
        private IOptionsSnapshot<SMBStorageOptions> options;

        public SMBStorageClient(IOptionsSnapshot<SMBStorageOptions> options)
        {
            this.options = options;
        }

        public StorageType StorageType => StorageType.Backup;

        /// <summary>
        /// 异步保存文件，key中上级文件夹不存在则创建，文件存在时则删除再创建并将内容写入。
        /// 成功后返回file:///开头的url
        /// </summary>
        /// <param name="key">文件的名称/键</param>
        /// <param name="content">文件/流</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
        {
            if (key.StartsWith('/'))
            {
                throw new ArgumentException("key should not start with /", nameof(key));
            }
            string workingDir = options.Value.WorkingDir;
            string fullPath = Path.Combine(workingDir, key);
            string? fullDir = Path.GetDirectoryName(fullPath);//get the directory
            if (!Directory.Exists(fullDir))//automatically create dir
            {
                Directory.CreateDirectory(fullDir);
            }
            if (File.Exists(fullPath))//如果已经存在，则尝试删除
            {
                File.Delete(fullPath);
            }
            using Stream outStream = File.OpenWrite(fullPath);
            await content.CopyToAsync(outStream, cancellationToken);
            return new Uri(fullPath);
        }
    }
}
