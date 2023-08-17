using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Z.EventBus;
using Z.IdentityService.Domain;
using Z.IdentityService.Infrastructure;
using Z.IdentityService.WebAPI.Events;

namespace Z.IdentityService.WebAPI.Controllers.UserAdmin
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserAdminController : ControllerBase
    {
        private readonly IdUserManager userManager;
        private readonly IIdRepository repository;
        private readonly IEventBus eventBus;

        public UserAdminController(IdUserManager userManager, IIdRepository repository, IEventBus eventBus)
        {
            this.userManager = userManager;
            this.repository = repository;
            this.eventBus = eventBus;
        }

        /// <summary>
        /// 查找所有用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<UserDTO[]> FindAllUsers()
        {
            return userManager.Users.Select(u => UserDTO.Create(u)).ToArrayAsync();
        }

        /// <summary>
        /// 根据id查找用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<UserDTO> FindById(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            return UserDTO.Create(user);
        }

        /// <summary>
        /// 新增管理员用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddAdminUser(AddAdminUserRequest req)
        {
            (var result, var user, var password) = await repository.AddAdminUserAsync(req.UserName, req.PhoneNum);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.SumErrors());
            }
            //生成的密码短信发给对方
            //可以同时或者选择性的把新增用户的密码短信/邮件/打印给用户
            //体现了领域事件对于代码“高内聚、低耦合”的追求
            var userCreatedEvent = new UserCreatedEvent(user.Id, req.UserName, password, req.PhoneNum);
            eventBus.Publish("IdentityService.User.Created", userCreatedEvent);
            return Ok();
        }

        /// <summary>
        /// 删除管理员用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteAdminUser(Guid id)
        {
            await repository.RemoveUserAsync(id);
            return Ok();
        }

        /// <summary>
        /// 更新用户的手机号码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAdminUser(Guid id, EditAdminUserRequest req)
        {
            var user = await repository.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("用户没找到");
            }
            user.PhoneNumber = req.PhoneNum;
            await userManager.UpdateAsync(user);
            return Ok();
        }

        /// <summary>
        /// 重置管理员用户密码，并把新密码发送给用户
        /// tip：发送新密码的动作并不在该方法实现，而是通过消息队列告知相应的服务实现
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}")]
        public async Task<ActionResult> ResetAdminUserPassword(Guid id)
        {
            (var result, var user, var password) = await repository.ResetPasswordAsync(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.SumErrors());
            }
            //生成的密码短信发给对方
            var eventData = new ResetPasswordEvent(user.Id, user.UserName, password, user.PhoneNumber);
            eventBus.Publish("IdentityService.User.PasswordReset", eventData);
            return Ok();
        }
    }
}
