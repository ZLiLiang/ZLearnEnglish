using Z.EventBus;
using Z.IdentityService.Domain;

namespace Z.IdentityService.WebAPI.Events
{
    [EventName("IdentityService.User.Created")]
    public class UserCreatedEventHandler : JsonIntegrationEventHandler<UserCreatedEvent>
    {
        private readonly ISmsSender smsSender;

        public UserCreatedEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }

        /// <summary>
        /// 发送初始密码给被创建用户的手机
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public override Task HandleJson(string eventName, UserCreatedEvent? eventData)
        {
            //发送初始密码给被创建用户的手机
            return smsSender.SendAsync(eventData.PhoneNum, eventData.Password);
        }
    }
}
