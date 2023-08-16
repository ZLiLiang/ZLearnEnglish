using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Z.FileService.Domain;

namespace Z.FileService.Infrastructure.Services
{
    /// <summary>
    /// 把FileService.WebAPI当成一个云存储服务器，是一个Mock。文件保存在wwwroot文件夹下。
    /// 这仅供开发、演示阶段使用，在生产环境中，一定要用专门的云存储服务器来代替。
    /// </summary>
    public class MockCloudStorageClient : IStorageClient
    {
        private readonly IWebHostEnvironment hostEnv;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MockCloudStorageClient(IWebHostEnvironment hostEnv, IHttpContextAccessor httpContextAccessor)
        {
            this.hostEnv = hostEnv;
            this.httpContextAccessor = httpContextAccessor;
        }

        public StorageType StorageType => StorageType.Public;

        /// <summary>
        /// 异步保存文件，key中上级文件夹不存在则创建，文件存在时则删除再创建并将内容写入。
        /// 成功后返回包含Scheme、Host的url
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
            string workingDir = Path.Combine(hostEnv.ContentRootPath, "wwwroot");
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
            var req = httpContextAccessor.HttpContext.Request;
            string url = req.Scheme + "://" + req.Host + "/FileService/" + key;
            return new Uri(url);
        }
    }
}
