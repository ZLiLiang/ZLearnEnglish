using Z.EventBus;
using Z.IdentityService.Domain;

namespace Z.IdentityService.WebAPI.Events
{
    [EventName("IdentityService.User.PasswordReset")]
    public class ResetPasswordEventHandler : JsonIntegrationEventHandler<ResetPasswordEvent>
    {
        private readonly ILogger<ResetPasswordEventHandler> logger;
        private readonly ISmsSender smsSender;

        public ResetPasswordEventHandler(ILogger<ResetPasswordEventHandler> logger, ISmsSender smsSender)
        {
            this.logger = logger;
            this.smsSender = smsSender;
        }

        /// <summary>
        /// 给用户发送短信，内容为重置后的密码
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public override Task HandleJson(string eventName, ResetPasswordEvent? eventData)
        {
            //发送密码给被用户的手机
            return smsSender.SendAsync(eventData.PhoneNum, eventData.Password);
        }
    }
}
