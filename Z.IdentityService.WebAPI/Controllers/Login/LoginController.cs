using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using Z.IdentityService.Domain;
using Z.IdentityService.Domain.Entities;

namespace Z.IdentityService.WebAPI.Controllers.Login
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IIdRepository repository;
        private readonly IdDomainService idService;

        public LoginController(IIdRepository repository, IdDomainService idService)
        {
            this.repository = repository;
            this.idService = idService;
        }

        /// <summary>
        /// 创建admin用户，并为用户添加user和admin角色。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateWorld()
        {
            if (await repository.FindByNameAsync("admin") != null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, "已经初始化过了");
            }
            User user = new User("admin");
            var r = await repository.CreateAsync(user, "123456");
            Debug.Assert(r.Succeeded);
            var token = await repository.GenerateChangePhoneNumberTokenAsync(user, "18918999999");
            var cr = await repository.ChangePhoneNumAsync(user.Id, "18918999999", token);
            Debug.Assert(cr.Succeeded);
            r = await repository.AddToRoleAsync(user, "User");
            Debug.Assert(r.Succeeded);
            r = await repository.AddToRoleAsync(user, "Admin");
            Debug.Assert(r.Succeeded);
            return Ok();
        }

        /// <summary>
        /// 返回用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserResponse>> GetUserInfo()
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await repository.FindByIdAsync(Guid.Parse(userId));
            if (user == null)//可能用户注销了
            {
                return NotFound();
            }
            //出于安全考虑，不要机密信息传递到客户端
            //除非确认没问题，否则尽量不要直接把实体类对象返回给前端
            return new UserResponse(user.Id, user.PhoneNumber, user.CreationTime);
        }

        /// <summary>
        /// 手机号码+密码登陆验证
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string?>> LoginByPhoneAndPwd(LoginByPhoneAndPwdRequest req)
        {
            //todo：要通过行为验证码、图形验证码等形式来防止暴力破解
            (var checkResult, string? token) = await idService.LoginByPhoneAndPwdAsync(req.PhoneNum, req.Password);
            if (checkResult.Succeeded)
            {
                return token;
            }
            else if (checkResult.IsLockedOut)
            {
                //尝试登录次数太多
                return StatusCode((int)HttpStatusCode.Locked, "此账号已经锁定");
            }
            else
            {
                string msg = "登录失败";
                return StatusCode((int)HttpStatusCode.BadRequest, msg);
            }
        }

        /// <summary>
        /// 用户名+密码登陆验证
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> LoginByUserNameAndPwd(
            LoginByUserNameAndPwdRequest req)
        {
            (var checkResult, var token) = await idService.LoginByUserNameAndPwdAsync(req.UserName, req.Password);
            if (checkResult.Succeeded) return token!;
            else if (checkResult.IsLockedOut)//尝试登录次数太多
                return StatusCode((int)HttpStatusCode.Locked, "用户已经被锁定");
            else
            {
                string msg = checkResult.ToString();
                return BadRequest("登录失败" + msg);
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ChangeMyPassword(ChangeMyPasswordRequest req)
        {
            Guid userId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var resetPwdResult = await repository.ChangePasswordAsync(userId, req.Password);
            if (resetPwdResult.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(resetPwdResult.Errors.SumErrors());
            }
        }
    }
}
