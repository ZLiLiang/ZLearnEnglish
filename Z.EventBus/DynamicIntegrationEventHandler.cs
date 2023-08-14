using Dynamic.Json;

namespace Z.EventBus
{
    public abstract class DynamicIntegrationEventHandler : IIntegrationEventHandler
    {
        /// <summary>
        /// 动态类型整合事件柄
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public Task Handle(string eventName, string eventData)
        {
            dynamic dynamicEventData = DJson.Parse(eventData);
            return HandleDynamic(eventName, dynamicEventData);
        }

        public abstract Task HandleDynamic(string eventName, dynamic eventData);
    }
}
