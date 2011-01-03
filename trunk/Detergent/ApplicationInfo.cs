using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Detergent
{
    public interface IApplicationInfo
    {
        string AppRootDirectory { get; }
        Version AppVersion { get; }

        string AbsolutizePath(string path);
    }

    public class ApplicationInfo : IApplicationInfo
    {
        public string AppRootDirectory
        {
            get
            {
                string applicationFullPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                string applicationDirectory = Path.GetDirectoryName(Path.GetFullPath(applicationFullPath));
                return applicationDirectory;
            }
        }

        public Version AppVersion
        {
            get
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                string fileVersion = version.FileVersion;
                return new Version(fileVersion);
            }
        }

        public string AbsolutizePath(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (Path.IsPathRooted(path))
                return path;

            return Path.Combine(AppRootDirectory, path);
        }
    }
}