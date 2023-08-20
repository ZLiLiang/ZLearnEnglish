﻿using MediatR;
using Z.EventBus;
using Z.MediaEncoder.Domain.Events;

namespace Z.MediaEncoder.WebAPI.EventHandlers
{
    class EncodingItemStartedEventHandler : INotificationHandler<EncodingItemStartedEvent>
    {
        private readonly IEventBus eventBus;

        public EncodingItemStartedEventHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }
        public Task Handle(EncodingItemStartedEvent notification, CancellationToken cancellationToken)
        {
            eventBus.Publish("MediaEncoding.Started", notification);
            return Task.CompletedTask;
        }
    }
}
