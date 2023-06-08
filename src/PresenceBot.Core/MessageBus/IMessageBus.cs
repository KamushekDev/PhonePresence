namespace PresenceBot.Core.MessageBus;

// In case of this application this is sufficient solution
// It doesn't need to be reliable and skip prone  
public interface IMessageBus<T>
{
    public Task Publish(T message, CancellationToken token);
    public Task<T> Consume(CancellationToken token);
}