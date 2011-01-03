using System;
using System.Collections;
using Detergent.Templating;
using NVelocity;

namespace Detergent.Mvc
{
    public class MvcTemplateHttpResponse : TemplateHttpResponse
    {
        public MvcTemplateHttpResponse(
            ITemplateSource templateSource,
            string contentType,
            IDictionary properties) : base(templateSource, contentType, properties)
        {
        }

        public static MvcTemplateHttpResponse ReturnHtml(
            string htmlTemplateFileName, 
            IFileCache fileCache,
            IDictionary properties)
        {
            ITemplateSource template = new CacheableFileTemplate(
                htmlTemplateFileName,
                fileCache);

            return new MvcTemplateHttpResponse(
                template,
                HttpConstants.ContentTypeHtml,
                properties);
        }
    }
}