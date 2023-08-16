using System.Net.Http.Headers;
using System.Security.Claims;
using Z.Commons;
using Z.JWT;

namespace Z.FileService.SDK.NETCore
{
    public class FileServiceClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly Uri serverRoot;
        private readonly JWTOptions optionsSnapshot;
        private readonly ITokenService tokenService;

        public FileServiceClient(IHttpClientFactory httpClientFactory, Uri serverRoot, JWTOptions optionsSnapshot, ITokenService tokenService)
        {
            this.httpClientFactory = httpClientFactory;
            this.serverRoot = serverRoot;
            this.optionsSnapshot = optionsSnapshot;
            this.tokenService = tokenService;
        }

        /// <summary>
        /// 向api发送请求，验证文件是否存在（异步操作）
        /// </summary>
        /// <param name="fileSize">文件大小</param>
        /// <param name="sha256Hash">哈希字符串</param>
        /// <param name="stoppingToken">停止令牌</param>
        /// <returns></returns>
        public Task<FileServiceClient> FileExistsAsync(long fileSize, string sha256Hash, CancellationToken stoppingToken = default)
        {
            string relativeUrl = FormattableStringHelper.BuildUrl($"api/Uploader/FileExists?fileSize={fileSize}&sha256Hash={sha256Hash}");
            Uri requestUri = new Uri(serverRoot, relativeUrl);
            var httpClient = httpClientFactory.CreateClient();
            return httpClient.GetJsonAsync<FileServiceClient>(requestUri, stoppingToken);
        }

        /// <summary>
        /// 上传文件，使用FileInfo类进行业务（异步方法）
        /// </summary>
        /// <param name="file">FileInfo对象</param>
        /// <param name="stoppingToken">取消令牌</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<Uri> UploadAsync(FileInfo file, CancellationToken stoppingToken = default)
        {
            string token = BuildToken();
            using MultipartFormDataContent content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(file.OpenRead());
            content.Add(fileContent, "file", file.Name);
            var httpClient = httpClientFactory.CreateClient();
            Uri requestUri = new Uri(serverRoot + "/Uploader/Upload");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respMsg = await httpClient.PostAsync(requestUri, content, stoppingToken);
            if (!respMsg.IsSuccessStatusCode)
            {
                string respString = await respMsg.Content.ReadAsStringAsync(stoppingToken);
                throw new HttpRequestException($"上传失败，状态码：{respMsg.StatusCode}，响应报文：{respString}");
            }
            else
            {
                string respString = await respMsg.Content.ReadAsStringAsync(stoppingToken);
                return respString.ParseJson<Uri>()!;
            }
        }

        /// <summary>
        /// 构建token
        /// </summary>
        /// <returns></returns>
        private string BuildToken()
        {
            //因为JWT的key等机密信息只有服务器端知道，因此可以这样非常简单的读到配置
            Claim claim = new Claim(ClaimTypes.Role, "Admin");
            return tokenService.BuildToken(new Claim[] { claim }, optionsSnapshot);
        }
    }
}