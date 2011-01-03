namespace Detergent.Templating
{
    public class StringTemplate : ITemplateSource
    {
        public StringTemplate(string contents)
        {
            this.contents = contents;
        }

        public string GetTemplate()
        {
            return contents;
        }

        private readonly string contents;
    }
}