using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Flubu;

namespace BuildScripts
{
    public class PackageMasterDirectory
    {
        public PackageMasterDirectory(PackageMaster master, string directory)
        {
            this.master = master;
            sourceDirectory = directory;

            ExcludeFilter(".svn");
        }

        public PackageMaster Commit ()
        {
            string destinationDirTemp = PathBuilder.New(master.PackagingDirTemp).Add(destinationDirectory);

            CopyFilesToTemp(sourceDirectory, destinationDirTemp);

            return master;
        }

        public PackageMasterDirectory Exclude (string fileName)
        {
            excludedFiles.Add (fileName);
            return this;
        }

        public PackageMasterDirectory ExcludeFilter (string filter)
        {
            excludeFilters.Add(new Regex(filter, RegexOptions.IgnoreCase | RegexOptions.Compiled));
            return this;
        }

        public PackageMasterDirectory ToDir(string dirNameFormat, params object[] args)
        {
            destinationDirectory = string.Format(CultureInfo.InvariantCulture, dirNameFormat, args);
            return this;
        }

        private bool MatchesExcludeFilters(string fileName)
        {
            foreach (Regex filter in excludeFilters)
            {
                if (filter.IsMatch(fileName))
                    return true;
            }

            return false;
        }

        private void CopyFilesToTemp(string fromDirectory, string destinationDir)
        {
            this.master.Log("Copying files from '{0}' to '{1}'", fromDirectory, destinationDir);

            if (false == Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            foreach (string fileName in Directory.GetFiles(fromDirectory))
            {
                if (MatchesExcludeFilters(fileName))
                    continue;

                bool excludeFile = false;
                foreach (string excludedFileName in excludedFiles)
                {
                    string shortFileName = Path.GetFileName(fileName);
                    if (0 == String.Compare(shortFileName, excludedFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        excludeFile = true;
                        break;
                    }
                }

                if (excludeFile)
                    continue;

                string destinationFileName = PathBuilder.New (destinationDir).Add (Path.GetFileName (fileName));

                File.Copy(fileName, destinationFileName, true);
            }

            foreach (string subdirectory in Directory.GetDirectories(fromDirectory))
            {
                if (MatchesExcludeFilters(subdirectory))
                    continue;

                string subdirectoryName = Path.GetFileName(subdirectory);
                string subdirectoryDestination = Path.Combine(destinationDir, subdirectoryName);
                CopyFilesToTemp(subdirectory, subdirectoryDestination);
            }
        }

        private string destinationDirectory = String.Empty;
        private readonly PackageMaster master;
        private readonly string sourceDirectory;
        private List<string> excludedFiles = new List<string>();
        private List<Regex> excludeFilters = new List<Regex>();
    }
}