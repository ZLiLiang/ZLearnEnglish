using MediatR;

namespace Z.Listening.Domain.Events
{
    public record EpisodeDeletedEvent(Guid Id) : INotification;
}
