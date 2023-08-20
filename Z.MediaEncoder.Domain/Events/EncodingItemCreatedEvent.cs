using MediatR;
using Z.MediaEncoder.Domain.Entities;

namespace Z.MediaEncoder.Domain.Events
{
    public record EncodingItemCreatedEvent(EncodingItem Value) : INotification;
}
