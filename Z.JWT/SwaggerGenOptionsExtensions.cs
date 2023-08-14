using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Z.JWT
{
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// 添加swagger的头验证
        /// </summary>
        /// <param name="options"></param>
        public static void AddAuthenticationHeader(this SwaggerGenOptions options)
        {
            //配置jwt验证并注册到swagger中
            options.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
            {
                Description = "Authorization header. \r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Authorization"
            });

            //添加安全要求并注册到swagger中
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference= new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Authorization"
                        },
                        Scheme="oauth2",
                        Name="Authorization",
                        In=ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        }
    }
}
