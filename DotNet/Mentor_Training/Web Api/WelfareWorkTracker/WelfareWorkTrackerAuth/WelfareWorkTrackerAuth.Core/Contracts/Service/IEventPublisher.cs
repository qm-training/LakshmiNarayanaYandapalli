namespace WelfareWorkTrackerAuth.Core.Contracts.Services;

public interface IEventPublisher
{
    Task PublishEventAsync<TEvent>(TEvent eventMessage) where TEvent : class;
}