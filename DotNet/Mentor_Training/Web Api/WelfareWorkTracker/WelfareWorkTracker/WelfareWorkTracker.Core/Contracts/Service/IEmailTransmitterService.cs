namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface IEmailTransmitterService
    {
        Task<bool> Publish<T>(T customerData) where T : class;
        Task<bool> PublishBatch<T>(IList<T> customerData) where T : class;
    }
}
