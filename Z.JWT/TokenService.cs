using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Z.JWT
{
    public class TokenService : ITokenService
    {
        /// <summary>
        /// 构建token
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string BuildToken(IEnumerable<Claim> claims, JWTOptions options)
        {
            TimeSpan ExpiryDuration = TimeSpan.FromSeconds(options.ExpireSeconds);  //获取存活时间
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));    //获取密钥
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);  //根据密钥签署证书
            var tokenDescriptor = new JwtSecurityToken(options.Issuer, options.Audience, claims, expires: DateTime.Now.Add(ExpiryDuration), signingCredentials: credentials);   //生成token描述对象
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);   //生成token
        }
    }
}
