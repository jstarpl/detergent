namespace Detergent
{
    public interface IDetergentRequestCleanup
    {
        void AfterRequest(IHttpContext context);
    }
}