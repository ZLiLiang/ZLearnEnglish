using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Z.Commons
{
    public static class HttpHelper
    {
        /// <summary>
        /// 异步保存文件
        /// </summary>
        /// <param name="respMsg"></param>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task SaveToFileAsync(this HttpResponseMessage respMsg, string file, CancellationToken cancellationToken = default)
        {
            if (respMsg.IsSuccessStatusCode == false)
            {
                throw new ArgumentException($"StatusCode of HttpResponseMessage is {respMsg.StatusCode}", nameof(respMsg));
            }
            using FileStream fs = new FileStream(file, FileMode.Create);
            await respMsg.Content.CopyToAsync(fs, cancellationToken);
        }

        /// <summary>
        /// 异步下载文件
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <param name="localFile"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<HttpStatusCode> DownloadFileAsync(this HttpClient httpClient, Uri url, string localFile, CancellationToken cancellationToken = default)
        {
            var resp = await httpClient.GetAsync(url, cancellationToken);
            if (resp.IsSuccessStatusCode)
            {
                await SaveToFileAsync(resp, localFile, cancellationToken);
                return resp.StatusCode;
            }
            else
            {
                return HttpStatusCode.OK;
            }
        }

        /// <summary>
        /// 以json形式返回请求连接内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> GetJsonAsync<T>(this HttpClient httpClient, Uri uri, CancellationToken cancellationToken = default)
        {
            string json = await httpClient.GetStringAsync(uri, cancellationToken);
            return json.ParseJson<T>();
        }
    }
}
