using System.Text;

namespace Detergent
{
    public class CachedFileResponse : IHttpResponse
    {
        public CachedFileResponse(
            string fileName, 
            IFileCache fileCache,
            string contentType, 
            Encoding encoding)
        {
            this.fileName = fileName;
            this.fileCache = fileCache;
            this.contentType = contentType;
            this.encoding = encoding;
        }

        public void Send(IHttpContext context)
        {
            string fileContents = fileCache.GetFile(fileName);
            context.SetResponse(fileContents, contentType, encoding);
        }

        private readonly string fileName;
        private readonly IFileCache fileCache;
        private readonly string contentType;
        private readonly Encoding encoding;
    }
}