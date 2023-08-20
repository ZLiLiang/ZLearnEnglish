using MediatR;

namespace Z.MediaEncoder.Domain.Events
{
    public record EncodingItemCompletedEvent(Guid Id, string SourceSystem, Uri OutputUrl) : INotification;
}
