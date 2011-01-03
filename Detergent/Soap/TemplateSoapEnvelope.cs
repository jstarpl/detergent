using System.Collections;
using Detergent.Templating;

namespace Detergent.Soap
{
    public class TemplateSoapEnvelope : TemplateHttpResponse, ISoapEnvelope
    {
        public TemplateSoapEnvelope(string templateText, IDictionary properties)
            : base(templateText, HttpConstants.ContentTypeXml, properties)
        {
        }

        public bool HasFault
        {
            get { return false; }
        }
    }
}