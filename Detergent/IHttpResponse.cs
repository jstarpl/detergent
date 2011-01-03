namespace Detergent
{
    /// <summary>
    /// Represents the HTTP response returned by the <see cref="IDetergentHttpHandler.ProcessRequest"/>
    /// method.
    /// </summary>
    public interface IHttpResponse
    {
        void Send(IHttpContext context);
    }
}