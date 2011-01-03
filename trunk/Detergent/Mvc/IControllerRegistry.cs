namespace Detergent.Mvc
{
    public interface IControllerRegistry
    {
        void EnsureIsRegistered<TController>() where TController : class, IController;
        TController FetchController<TController>() where TController : class, IController;
        void ReleaseController<TController>(TController controller) where TController : class, IController;
    }
}