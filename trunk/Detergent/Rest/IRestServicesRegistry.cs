namespace Detergent.Rest
{
    public interface IRestServicesRegistry
    {
        void EnsureIsRegistered<TService>() where TService : IRestService;
        TService FetchService<TService>() where TService : IRestService;
        void ReleaseService<TService>(TService controller) where TService : IRestService;
    }
}