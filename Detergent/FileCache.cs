using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using log4net;

namespace Detergent
{
    public interface IFileCache 
    {
        string GetFile(string fileName);
        bool HasFile(string fileName);
    }

    public class FileCache : IFileCache
    {
        public string GetFile(string fileName)
        {
            string fileNameLower = fileName.ToLowerInvariant();

            lock (this)
            {
                if (!cachedFiles.ContainsKey(fileNameLower))
                {
                    if (log.IsDebugEnabled)
                        log.DebugFormat("FileCache: reading file '{0}' from disk, caching it.", fileName);

                    if (!File.Exists(fileName))
                    {
                        string message = string.Format(
                            CultureInfo.InvariantCulture,
                            "File '{0}' does not exist",
                            fileName);
                        throw new InvalidOperationException(message);
                    }

                    string contents = File.ReadAllText(fileNameLower);
                    cachedFiles.Add(fileNameLower, contents);
                }

                return cachedFiles[fileNameLower];
            }
        }

        public bool HasFile(string fileName)
        {
            string fileNameLower = fileName.ToLowerInvariant();

            lock (this)
            {
                return cachedFiles.ContainsKey(fileNameLower);
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<string, string> cachedFiles = new Dictionary<string, string>();
    }
}