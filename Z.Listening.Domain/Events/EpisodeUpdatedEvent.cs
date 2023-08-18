using MediatR;
using Z.Listening.Domain.Entities;

namespace Z.Listening.Domain.Events
{
    public record EpisodeUpdatedEvent(Episode Value) : INotification;
}
