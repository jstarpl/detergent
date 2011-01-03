namespace Detergent
{
    /// <summary>
    /// A handler which can process HTTP requests and return HTTP responses.
    /// </summary>
    public interface IDetergentHttpHandler
    {
        IDetergentRequestCleanup RequestCleanup { get; set; }

        /// <summary>
        /// Processes a HTTP request contained in the recieved HTTP context
        /// and returns a HTTP response.
        /// </summary>
        /// <param name="context">HTTP context which contains the request.</param>
        /// <returns>A HTTP response to the HTTP request.</returns>
        IHttpResponse ProcessRequest(IHttpContext context);

        void DumpDiagnostics();
    }
}