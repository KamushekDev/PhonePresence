using System.Threading.Channels;
using PresenceBot.Core.MessageBus;

namespace PresenceBot.Infrastructure.MessageBus;

public class MessageBus<T> : IMessageBus<T>
{
    private readonly Channel<T> _channel;

    public MessageBus(int? capacity = null)
    {
        _channel = capacity.HasValue 
            ? Channel.CreateBounded<T>(capacity.Value) 
            : Channel.CreateUnbounded<T>();
    }

    public async Task Publish(T message, CancellationToken token)
    {
        await _channel.Writer.WriteAsync(message, token);
    }

    public async Task<T> Consume(CancellationToken token)
    {
        return await _channel.Reader.ReadAsync(token);
    }
}