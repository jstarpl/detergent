using System.Text;

namespace Detergent.Mvc
{
    public class StaticFileController : ControllerBase
    {
        public StaticFileController(IFileCache fileCache) : base(fileCache)
        {
        }

        public IHttpResponse Css()
        {
            return ReturnFile(HttpContext.RequestPath.Substring(1), HttpConstants.ContentTypeCss, Encoding.ASCII);
        }
    }
}