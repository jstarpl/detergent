using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;

namespace Detergent.WebPages
{
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WebPage")]
    public abstract class WebPage : IWebPage
    {
        public string PageTitle
        {
            get;
            set;
        }

        public void Send(IHttpContext context)
        {
            string response = ConstructPageHtml(context);
            context.SetResponse(response, HttpConstants.ContentTypeHtml, Encoding.UTF8);
        }

        public virtual string ConstructPageHtml(IHttpContext context)
        {
            httpContext = context;
            StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            AssignHtmlWriter(context, new XhtmlTextWriter(stringWriter));
            RenderHtml();
            return stringWriter.ToString();
        }

        public WebPage IncludeCss(string cssFileName)
        {
            includedCssFiles.Add(cssFileName);
            return this;
        }

        protected IHttpContext HttpContext
        {
            get { return httpContext; }
        }

        protected XhtmlTextWriter Writer
        {
            get { return writer; }
        }

        protected virtual void AssignHtmlWriter(
            IHttpContext httpContext,
            XhtmlTextWriter xhtmlTextWriter)
        {
            writer = xhtmlTextWriter;
        }

        protected virtual void RenderHtml()
        {
            RenderPageHeader();
            RenderPageBody();
            RenderPageFooter();
        }

        protected virtual void RenderPageHeader()
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Html);
            writer.RenderBeginTag(HtmlTextWriterTag.Head);

            RenderPageTitle();
            RenderBaseElement();
            RenderMetaTags();
            RenderCssLinks();

            writer.RenderEndTag(); // head
            writer.RenderBeginTag(HtmlTextWriterTag.Body);
        }

        protected virtual void RenderPageTitle()
        {
            if (!String.IsNullOrEmpty(PageTitle))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Title);
                writer.Write(PageTitle);
                writer.RenderEndTag();
            }
        }

        protected virtual void RenderBaseElement()
        {
        }

        protected virtual void RenderMetaTags()
        {
            writer.AddAttribute("HTTP-EQUIV", "Content-Type");
            writer.AddAttribute("CONTENT", "text/html; charset=utf-8");
            writer.RenderBeginTag(HtmlTextWriterTag.Meta);
            writer.RenderEndTag();
            writer.WriteLine();
        }

        protected virtual void RenderCssLinks()
        {
            foreach (string includedCssFile in includedCssFiles)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
                writer.AddAttribute(HtmlTextWriterAttribute.Href, includedCssFile);
                writer.RenderBeginTag(HtmlTextWriterTag.Link);
                writer.RenderEndTag();
                writer.WriteLine();
            }
        }

        protected abstract void RenderPageBody();

        protected virtual void RenderPageFooter()
        {
            writer.RenderEndTag(); // body
            writer.RenderEndTag(); // html
        }

        private IHttpContext httpContext;
        private List<string> includedCssFiles = new List<string>();
        private XhtmlTextWriter writer;
    }
}