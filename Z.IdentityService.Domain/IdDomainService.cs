using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Z.IdentityService.Domain.Entities;
using Z.JWT;

namespace Z.IdentityService.Domain
{
    public class IdDomainService
    {
        private readonly IIdRepository repository;
        private readonly ITokenService tokenService;
        private readonly IOptions<JWTOptions> optJWT;

        public IdDomainService(IIdRepository repository, ITokenService tokenService, IOptions<JWTOptions> optJWT)
        {
            this.repository = repository;
            this.tokenService = tokenService;
            this.optJWT = optJWT;
        }

        /// <summary>
        /// 验证用户与密码是否存在
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private async Task<SignInResult> CheckUserNameAndPwdAsync(string userName, string password)
        {
            var user = await repository.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            //CheckPasswordSignInAsync会对于多次重复失败进行账号禁用
            var result = await repository.CheckForSignInAsync(user, password, true);
            return result;
        }

        /// <summary>
        /// 验证手机号码与密码是否存在
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<SignInResult> CheckPhoneNumAndPwdAsync(string phoneNum, string password)
        {
            var user = await repository.FindByPhoneNumberAsync(phoneNum);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            var result = await repository.CheckForSignInAsync(user, password, true);
            return result;
        }

        /// <summary>
        /// 异步通过用户构建token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> BuildTokenAsync(User user)
        {
            var roles = await repository.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return tokenService.BuildToken(claims, optJWT.Value);
        }

        /// <summary>
        /// 通过手机号码与密码登陆，成功返回token
        /// </summary>
        /// <param name="phoneNum">手机号码</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<(SignInResult Result, string? Token)> LoginByPhoneAndPwdAsync(string phoneNum, string password)
        {
            var checkResult = await CheckPhoneNumAndPwdAsync(phoneNum, password);
            if (checkResult.Succeeded)
            {
                var user = await repository.FindByPhoneNumberAsync(phoneNum);
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
            else
            {
                return (checkResult, null);
            }
        }

        /// <summary>
        /// 通过用户名与密码登陆，成功返回token
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<(SignInResult Result, string? Token)> LoginByUserNameAndPwdAsync(string userName, string password)
        {
            var checkResult = await CheckUserNameAndPwdAsync(userName, password);
            if (checkResult.Succeeded)
            {
                var user = await repository.FindByNameAsync(userName);
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
            else
            {
                return (checkResult, null);
            }
        }
    }
}
