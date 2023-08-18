using MediatR;
using Z.Listening.Domain.Entities;

namespace Z.Listening.Domain.Events
{
    public record EpisodeCreatedEvent(Episode Value) : INotification;
}
