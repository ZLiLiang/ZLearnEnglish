using Microsoft.AspNetCore.Mvc;

namespace Z.ASPNETCore
{
    public static class CacheKeyHelper
    {
        /// <summary>
        /// 获取和这个Action请求相关的CacheKey，主要考虑Controller名字、Action名字、参数等
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static string CalcCacheKeyFromAction(this ControllerBase controller)
        {
            return GetCacheKey(controller.ControllerContext);
        }

        /// <summary>
        /// 获取缓存key，以路由路径作为key
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        public static string GetCacheKey(this ControllerContext controllerContext)
        {
            var routeValues = controllerContext.RouteData.Values;
            string cacheKey = string.Join(".", routeValues);
            return cacheKey;
        }
    }
}
