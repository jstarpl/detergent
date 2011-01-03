using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Flubu;
using Flubu.Builds;
using ICSharpCode.SharpZipLib.Zip;

//css_import PackageMasterDirectory.cs;

namespace BuildScripts
{
    public class PackageMaster
    {
        public PackageMaster(ConcreteBuildRunner runner, string packagingDir)
        {
            this.runner = runner;
            this.packagingDir = packagingDir;
            packagingDirTemp = PathBuilder.New(packagingDir).Add("Temp");
        }

        public string LastZipFileName
        {
            get { return lastZipFileName; }
        }

        public string PackagingDirTemp
        {
            get { return packagingDirTemp; }
        }

        public PackageMasterDirectory AddDir (string directory)
        {
            PackageMasterDirectory dir = new PackageMasterDirectory(this, directory);
            items.Add(dir);
            return dir;
        }

        public PackageMaster CleanPackagingTempDirectory ()
        {
            if (Directory.Exists(packagingDirTemp))
                Directory.Delete(packagingDirTemp, true);

            Directory.CreateDirectory(packagingDirTemp);

            return this;
        }

        public void Log (string format, params object[] args)
        {
            runner.Log(format, args);
        }

        public PackageMaster Zip(string zipFileNameFormat, params object[] args)
        {
            lastZipFileName = string.Format(
                CultureInfo.InvariantCulture,
                zipFileNameFormat,
                args);

            lastZipFileName = new PathBuilder(packagingDir).Add(lastZipFileName);
 
            FastZip zip = new FastZip();
            zip.CreateZip(lastZipFileName, packagingDirTemp, true, null, null);

            return this;
        }

        private string lastZipFileName;
        private readonly ConcreteBuildRunner runner;
        private string packagingDir;
        private string packagingDirTemp;
        private List<PackageMasterDirectory> items = new List<PackageMasterDirectory>();
    }
}