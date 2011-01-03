using System.IO;

namespace Detergent.Templating
{
    public class FileTemplate : ITemplateSource
    {
        public FileTemplate(string fileName)
        {
            this.fileName = fileName;
        }

        public string GetTemplate()
        {
            return File.ReadAllText(fileName);
        }

        private readonly string fileName;
    }
}