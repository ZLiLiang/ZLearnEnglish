using MediatR;

namespace Z.MediaEncoder.Domain.Events
{
    public record EncodingItemStartedEvent(Guid Id, string SourceSystem) : INotification;
}
