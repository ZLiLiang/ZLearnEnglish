using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using Z.IdentityService.Domain;
using Z.IdentityService.Domain.Entities;

namespace Z.IdentityService.Infrastructure
{
    class IdRepository : IIdRepository
    {
        private readonly IdUserManager userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly ILogger<IdRepository> logger;

        public IdRepository(IdUserManager userManager, RoleManager<Role> roleManager, ILogger<IdRepository> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }

        /// <summary>
        /// 通过手机号码查询
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public Task<User?> FindByPhoneNumberAsync(string phoneNum)
        {
            return userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNum);
        }

        /// <summary>
        /// 通过id查询
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<User?> FindByIdAsync(Guid userId)
        {
            return userManager.FindByIdAsync(userId.ToString());
        }

        /// <summary>
        /// 通过名字查询
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<User?> FindByNameAsync(string userName)
        {
            return userManager.FindByNameAsync(userName);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<IdentityResult> CreateAsync(User user, string password)
        {
            return this.userManager.CreateAsync(user, password);
        }

        /// <summary>
        /// 访问失败
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns></returns>
        public Task<IdentityResult> AccessFailedAsync(User user)
        {
            return userManager.AccessFailedAsync(user);
        }

        /// <summary>
        /// 修改手机号码
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="phoneNum">手机号码</param>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<SignInResult> ChangePhoneNumAsync(Guid userId, string phoneNum, string token)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ArgumentException($"{userId}的用户不存在");
            }
            var changeResult = await this.userManager.ChangePhoneNumberAsync(user, phoneNum, token);
            if (!changeResult.Succeeded)
            {
                await this.userManager.AccessFailedAsync(user);
                string errMsg = changeResult.Errors.SumErrors();
                this.logger.LogWarning($"{phoneNum}ChangePhoneNumberAsync失败，错误信息{errMsg}");
                return SignInResult.Failed;
            }
            else
            {
                await ConfirmPhoneNumberAsync(user.Id);//确认手机号
                return SignInResult.Success;
            }
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string password)
        {
            if (password.Length < 6)
            {
                IdentityError error = new IdentityError();
                error.Code = "Password Invalid";
                error.Description = "密码长度不能少于6";
                return IdentityResult.Failed(error);

            }
            var user = await userManager.FindByIdAsync(userId.ToString());
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetPwdResult = await userManager.ResetPasswordAsync(user, token, password);
            return resetPwdResult;
        }

        /// <summary>
        /// 生成修改手机号码的token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber)
        {
            return this.userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        }

        /// <summary>
        /// 返回角色
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<string>> GetRolesAsync(User user)
        {
            return userManager.GetRolesAsync(user);
        }

        /// <summary>
        /// 为用户增添角色
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                Role role = new Role { Name = roleName };
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded == false)
                {
                    return result;
                }
            }
            return await userManager.AddToRoleAsync(user, roleName);
        }

        /// <summary>
        /// 尝试登录，如果lockoutOnFailure为true，则登录失败还会自动进行lockout计数
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="lockoutOnFailure"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure)
        {
            if (await userManager.IsLockedOutAsync(user))
            {
                return SignInResult.LockedOut;
            }
            var success = await userManager.CheckPasswordAsync(user, password);
            if (success)
            {
                return SignInResult.Success;
            }
            else
            {
                if (lockoutOnFailure)
                {
                    var r = await AccessFailedAsync(user);
                    if (!r.Succeeded)
                    {
                        throw new ApplicationException("AccessFailed failed");
                    }
                }
                return SignInResult.Failed;
            }
        }

        /// <summary>
        /// 通过id查询用户是否存在，当存在时，将其手机号码验证为true
        /// </summary>
        /// <param name="id">guid</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task ConfirmPhoneNumberAsync(Guid id)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new ArgumentException($"用户找不到，id={id}", nameof(id));
            }
            user.PhoneNumberConfirmed = true;
            await userManager.UpdateAsync(user);
        }

        /// <summary>
        /// 根据id更新电话号码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task UpdatePhoneNumberAsync(Guid id, string phoneNum)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new ArgumentException($"用户找不到，id={id}", nameof(id));
            }
            user.PhoneNumber = phoneNum;
            await userManager.UpdateAsync(user);
        }

        /// <summary>
        /// 软删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveUserAsync(Guid id)
        {
            var user = await FindByIdAsync(id);
            var userLoginStore = userManager.UserLoginStore;
            var noneCT = default(CancellationToken);
            //一定要删除aspnetuserlogins表中的数据，否则再次用这个外部登录登录的话
            //就会报错：The instance of entity type 'IdentityUserLogin<Guid>' cannot be tracked because another instance with the same key value for {'LoginProvider', 'ProviderKey'} is already being tracked.
            //而且要先删除aspnetuserlogins数据，再软删除User
            var logins = await userLoginStore.GetLoginsAsync(user, noneCT);
            foreach (var login in logins)
            {
                await userLoginStore.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey, noneCT);
            }
            user.SoftDelete();
            var result = await userManager.UpdateAsync(user);
            return result;
        }

        /// <summary>
        /// 错误结果
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static IdentityResult ErrorResult(string msg)
        {
            IdentityError idError = new IdentityError { Description = msg };
            return IdentityResult.Failed(idError);
        }

        /// <summary>
        /// 增加管理员用户（测试使用）
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public async Task<(IdentityResult, User?, string? password)> AddAdminUserAsync(string userName, string phoneNum)
        {
            if (await FindByNameAsync(userName) != null)
            {
                return (ErrorResult($"已经存在用户名{userName}"), null, null);
            }
            if (await FindByPhoneNumberAsync(phoneNum) != null)
            {
                return (ErrorResult($"已经存在手机号{phoneNum}"), null, null);
            }
            User user = new User(userName);
            user.PhoneNumber = phoneNum;
            user.PhoneNumberConfirmed = true;
            string password = GeneratePassword();
            var result = await CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return (result, null, null);
            }
            result = await AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
            {
                return (result, null, null);
            }
            return (IdentityResult.Success, user, password);
        }

        /// <summary>
        /// 根据id重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(IdentityResult, User?, string? password)> ResetPasswordAsync(Guid id)
        {
            var user = await FindByIdAsync(id);
            if (user == null)
            {
                return (ErrorResult("用户没找到"), null, null);
            }
            string password = GeneratePassword();
            string token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
            {
                return (result, null, null);
            }
            return (IdentityResult.Success, user, password);
        }

        /// <summary>
        /// 生成密码
        /// </summary>
        /// <returns></returns>
        private string GeneratePassword()
        {
            var options = userManager.Options.Password;
            int length = options.RequiredLength;
            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;
            StringBuilder password = new StringBuilder();
            Random random = new Random();
            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);
                password.Append(c);
                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));
            return password.ToString();
        }
    }
}
