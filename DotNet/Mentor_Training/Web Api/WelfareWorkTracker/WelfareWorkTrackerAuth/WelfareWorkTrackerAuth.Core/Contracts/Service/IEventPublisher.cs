namespace WelfareWorkTrackerAuth.Core.Contracts.Service;

public interface IEventPublisher
{
    Task PublishEventAsync<TEvent>(TEvent eventMessage) where TEvent : class;
}