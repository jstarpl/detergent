using System;
using System.Globalization;
using System.IO;
using System.Web.UI;
using NVelocity;
using NVelocity.App;

namespace Detergent.Templating
{
    public class NVelocityHtmlRenderHelper
    {
        public NVelocityHtmlRenderHelper(
            IHttpContext httpContext,
            XhtmlTextWriter writer, 
            IApplicationInfo applicationInfo, 
            IFileCache fileCache)
        {
            this.httpContext = httpContext;
            this.fileCache = fileCache;
            this.applicationInfo = applicationInfo;
            this.writer = writer;
        }

        public string AppUrl
        {
            get
            {
                return httpContext.ApplicationRootUrl;
            }
        }

        public string FormGet(string formAction)
        {
            return Format("<form action=\"{0}\" method=\"get\">", formAction);
        }

        public string InputText(string id)
        {
            return Format("<input id=\"{0}\" name=\"{0}\" type=\"text\" />", id);
        }

        public void RenderTemplate(string templateName, object model)
        {
            string templateFullPath = applicationInfo.AbsolutizePath("Web/Pages/Templates/" + templateName + ".vm.html");

            ITemplateSource template = new CacheableFileTemplate(
                templateFullPath,
                fileCache);
            string templateText = template.GetTemplate();

            VelocityEngine velocity = new VelocityEngine();
            velocity.Init();

            VelocityContext velocityContext = new VelocityContext();
            velocityContext.Put("m", model);
            velocityContext.Put("h", this);

            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                if (false == velocity.Evaluate(velocityContext, stringWriter, null, templateText))
                    throw new InvalidOperationException("Template expansion failed");

                writer.InnerWriter.Write(stringWriter.ToString());
            }

            writer.WriteLine();
        }

        public string Submit(string id, string buttonText)
        {
            return Format("<input id=\"{0}\" value=\"{1}\" type=\"submit\" />", id, buttonText);
        }

        private static string Format(string format, params object[] args)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                format,
                args);
        }

        private readonly IHttpContext httpContext;
        private readonly IFileCache fileCache;
        private readonly IApplicationInfo applicationInfo;
        private readonly XhtmlTextWriter writer;
    }
}