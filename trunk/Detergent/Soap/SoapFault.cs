using System.Diagnostics.CodeAnalysis;

namespace Detergent.Soap
{
    public class SoapFault
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
        public SoapFault(SoapFaultCode faultCode, string faultString)
        {
            this.faultCode = faultCode;
            this.faultString = faultString;
        }

        public SoapFaultCode FaultCode
        {
            get { return faultCode; }
        }

        public string FaultString
        {
            get { return faultString; }
        }

        private SoapFaultCode faultCode;
        private string faultString;
    }
}