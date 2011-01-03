using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Detergent.Mime
{
    public class HeaderField
    {
        public HeaderField(string fieldName, string fieldValue)
        {
            this.fieldName = fieldName;
            this.fieldValue = fieldValue;
        }

        public string FieldName
        {
            get { return fieldName; }
        }

        public string FieldValue
        {
            get { return fieldValue; }
            set { fieldValue = value; }
        }

        public IDictionary<string, string> Parameters
        {
            get { return parameters; }
        }

        public void AddParameter (string parameterName, string parameterValue)
        {
            parameters.Add(parameterName, parameterValue);
        }

        public static HeaderField Parse (string value)
        {
            Match match = headerRegex.Match(value);
            if (!match.Success)
                throw new FormatException();

            string fieldName = match.Groups["name"].Value.Trim();
            string fieldValue = match.Groups["value"].Value.Trim();
            HeaderField field = new HeaderField(fieldName, fieldValue);

            for (int i = 0; i < match.Groups["pname"].Captures.Count; i++)
            {
                string parameterName = match.Groups["pname"].Captures[i].Value.Trim();
                string parameterValue = match.Groups["pvalue"].Captures[i].Value.Trim();
                field.AddParameter(parameterName, parameterValue);
            }

            return field;
        }

        public static HeaderField Parse(byte[] data, Encoding encoding)
        {
            return Parse(encoding.GetString(data));
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1}", fieldName, fieldValue);
            foreach (KeyValuePair<string, string> parameter in parameters)
                s.AppendFormat(CultureInfo.InvariantCulture, "; {0}={1}", parameter.Key, parameter.Value);

            return s.ToString();
        }

        private string fieldName;
        private string fieldValue;
        private Dictionary<string, string> parameters = new Dictionary<string, string>();
        private static Regex headerRegex = new Regex(
            @"(?<name>[^:]+) \: (?<value>[^;]+) (\;(?<pname>[^=;]+)\=(?<pvalue>[^=;]+))*",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    }
}