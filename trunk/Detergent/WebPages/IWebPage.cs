using System;
using System.Diagnostics.CodeAnalysis;

namespace Detergent.WebPages
{
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WebPage")]
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
    public interface IWebPage : IHttpResponse
    {
    }
}