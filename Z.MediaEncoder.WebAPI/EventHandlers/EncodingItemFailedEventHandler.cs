using MediatR;
using Z.EventBus;
using Z.MediaEncoder.Domain.Events;

namespace Z.MediaEncoder.WebAPI.EventHandlers
{
    class EncodingItemFailedEventHandler : INotificationHandler<EncodingItemFailedEvent>
    {
        private readonly IEventBus eventBus;

        public EncodingItemFailedEventHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public Task Handle(EncodingItemFailedEvent notification, CancellationToken cancellationToken)
        {
            eventBus.Publish("MediaEncoding.Failed", notification);
            return Task.CompletedTask;
        }
    }
}
