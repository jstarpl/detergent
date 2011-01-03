namespace Detergent.Templating
{
    public class CacheableFileTemplate : ITemplateSource
    {
        public CacheableFileTemplate(string templateFileName, IFileCache fileCache)
        {
            this.templateFileName = templateFileName;
            this.fileCache = fileCache;
        }

        public string GetTemplate()
        {
            return fileCache.GetFile(templateFileName);
        }

        private readonly string templateFileName;
        private readonly IFileCache fileCache;
    }
}