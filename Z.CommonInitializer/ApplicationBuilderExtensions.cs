using Microsoft.AspNetCore.Builder;
using Z.EventBus;

namespace Z.CommonInitializer
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 启用EventBus、Cors、ForwardedHeaders、Authentication、Authorization
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseZDefault(this IApplicationBuilder app)
        {
            app.UseEventBus();
            app.UseCors();//启用Cors
            app.UseForwardedHeaders();
            //app.UseHttpsRedirection();//不能与ForwardedHeaders很好的工作，而且webapi项目也没必要配置这个
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
