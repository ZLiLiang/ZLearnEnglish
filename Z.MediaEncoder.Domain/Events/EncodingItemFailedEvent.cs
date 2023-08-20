using MediatR;

namespace Z.MediaEncoder.Domain.Events
{
    public record EncodingItemFailedEvent(Guid Id, string SourceSystem, string ErrorMessage) : INotification;
}
