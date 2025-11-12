namespace WelfareWorkTrackerAuth.Infrastructure.Services;
public class EventPublisher : IEventPublisher
{
    private readonly IBus _bus;

    public EventPublisher(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishEventAsync<TEvent>(TEvent eventMessage) where TEvent : class
    {
        await _bus.Publish(eventMessage);
    }
}
