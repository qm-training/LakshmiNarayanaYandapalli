namespace WelfareWorkTracker.Infrastructure.Service;
public class EventPublisher(IBus bus) : IEventPublisher
{
    private readonly IBus _bus = bus;
    
    public async Task PublishEventAsync<TEvent>(TEvent eventMessage) where TEvent : class
    {
        await _bus.Publish(eventMessage);
    }
}
