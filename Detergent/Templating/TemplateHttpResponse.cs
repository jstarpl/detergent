using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using NVelocity;
using NVelocity.App;

namespace Detergent.Templating
{
    public class TemplateHttpResponse : IHttpResponse
    {
        public TemplateHttpResponse(
            string templateText, 
            string contentType,
            IDictionary properties)
        {
            this.template = new StringTemplate(templateText);
            this.contentType = contentType;
            this.properties = properties;
        }

        public TemplateHttpResponse (
            ITemplateSource templateSource,
            string contentType,
            IDictionary properties)
        {
            template = templateSource;
            this.contentType = contentType;
            this.properties = properties;
        }

        public virtual void Send(IHttpContext context)
        {
            VelocityEngine velocity = new VelocityEngine();
            velocity.Init();

            VelocityContext velocityContext = new VelocityContext();

            if (properties != null)
            {
                foreach (DictionaryEntry entry in properties)
                    velocityContext.Put(entry.Key.ToString(), entry.Value);
            }

            AddStuffToVelocityContext(velocityContext, context);

            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                if (false == velocity.Evaluate(velocityContext, stringWriter, null, template.GetTemplate()))
                    throw new InvalidOperationException("Template expansion failed");

                context.SetResponse(stringWriter.GetStringBuilder().ToString(), contentType, Encoding.UTF8);
            }
        }

        [CLSCompliant(false)]
        protected virtual void AddStuffToVelocityContext(
            VelocityContext velocityContext, 
            IHttpContext httpContext)
        {
        }

        protected TemplateHttpResponse()
        {
            properties = new Hashtable();
        }

        protected ITemplateSource Template
        {
            get { return template; }
            set { template = value; }
        }

        protected IDictionary Properties
        {
            get { return properties; }
        }

        private ITemplateSource template;
        private readonly string contentType;
        private readonly IDictionary properties;
    }
}