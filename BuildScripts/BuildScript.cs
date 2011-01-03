using System;
using System.IO;
using Flubu;
using Flubu.Builds;

//css_ref bin\Debug\Flubu.dll;
//css_ref ..\lib\ICSharpCode.SharpZipLib.2.0\ICSharpCode.SharpZipLib.dll;
//css_ref ProjectPilot.BuildScripts.dll;
//css_import DetergentBuildRunner.cs;
//css_import PackageMaster.cs;

namespace BuildScripts
{
    public class BuildScript
    {
        public static int Main(string[] args)
        {
            using (DetergentBuildRunner runner = new DetergentBuildRunner ("Detergent"))
            {
                try
                {
                    runner
                        .SetLibrariesDirectory(@"lib")
                        .AddTarget("load.solution").SetAsHidden().Do(TargetLoadSolution);
                    runner.AddTarget("compile")
                        .SetDescription("Compiles the VS solution and runs FxCop analysis on it")
                        .Do(TargetCompile).DependsOn("load.solution");
                    runner.AddTarget ("unit.tests")
                        .SetDescription ("Runs unit tests on the project")
                        .Do (r => { } /*r.RunTests("Detergent.Tests", false)*/).DependsOn ("load.solution");
                    runner.AddTarget("web.tests")
                        .SetDescription("Runs Web tests on the project")
                        .Do(r => { } /*r.RunTests("Detergent.Tests.Web", false)*/).DependsOn("load.solution");
                    runner.AddTarget ("package")
                        .SetDescription("Packages all the build products into ZIP files")
                        .Do(TargetPackage).DependsOn("load.solution");
                    runner.AddTarget("rebuild")
                        .SetDescription("Rebuilds the project, runs tests and packages the build products.")
                        .SetAsDefault().DependsOn("compile", "unit.tests", "web.tests", "package");
                    runner.AddTarget ("update.lib.flubu")
                        .SetDescription ("Updates the lib directory with the latest versions of Flubu libraries")
                        .Do (r => TargetUpdateLib (r, @"D:\MyStuff\BuildArea\Builds\ProjectPilot\packages\Flubu\Flubu-latest.zip", @"lib\Flubu"));
                    //runner.AddTarget("docs")
                    //    .SetDescription("Generates Quiki documentation using Quiki :)")
                    //    .Do(r => TargetDocs(r, false)).DependsOn("load.solution");

                    // actual run
                    if (args.Length == 0)
                        runner.RunTarget(runner.DefaultTarget.TargetName);
                    else
                        runner.RunTarget(args[0]);

                    runner
                        .Complete();

                    return 0;
                }
                catch (RunnerFailedException)
                {
                    return 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return 1;
                }
                finally
                {
                    runner.MergeCoverageReports();
                    runner.CopyBuildLogsToCCNet();
                }
            }
        }

        private static void TargetCompile(ConcreteBuildRunner runner)
        {
            runner
                .PrepareBuildDirectory()
                .SetCompanyInfo(
                "Igor Brejc",
                String.Empty,
                String.Empty)
                .CleanOutput();

            string commonAssemblyInfoFileName = Path.Combine(runner.ProductRootDir, "CommonAssemblyInfo.cs");

            runner.ScriptExecutionEnvironment.LogMessage("CommonAssemblyInfo.cs path: '{0}'", commonAssemblyInfoFileName);
            runner
                .GenerateCommonAssemblyInfo();
            ((DetergentBuildRunner)runner).CompileSolution2(
                runner.ScriptExecutionEnvironment.Net35VersionNumber);

            runner.FxCop();
        }

        private static void TargetLoadSolution(ConcreteBuildRunner runner)
        {
            DetergentBuildRunner extendedRunner = (DetergentBuildRunner)runner;

            extendedRunner
                .LoadSolution("Detergent.2008.sln");
            extendedRunner
                .HudsonFetchBuildVersion ();
        }

        private static void TargetPackage(ConcreteBuildRunner runner)
        {
            //PackageMaster master = new PackageMaster(runner, @"Builds\Packages");
            //master
            //    .CleanPackagingTempDirectory()
            //    .AddDir (GetConsoleOutputPath(runner))
            //        .ToDir("Detergent-{0}", runner.BuildVersion)
            //        .Exclude ("log4net.xml")
            //        .Exclude ("NVelocity.xml")
            //        .Exclude ("PowerCollections.xml")
            //        .Commit ()
            //    //.AddDir("Docs")
            //    //    .ToDir("docs_source")
            //    //    .Commit()
            //    //.AddDir(PathBuilder.New(runner.BuildPackagesDir).Add("Docs"))
            //    //    .ToDir("docs")
            //    //    .Commit()
            //    .Zip ("Detergent-{0}.zip", runner.BuildVersion);
        }

        private static void TargetUpdateLib (
            ConcreteBuildRunner runner,
            string libBuildPackageFileName,
            string libIncludeDirectory)
        {
            PathBuilder tempDir = PathBuilder.New (runner.BuildDir).Add ("Temp");
            runner
                .DeleteDirectory (tempDir, false)
                .EnsureDirectoryPathExists (tempDir, false);

            runner.Unzip (
                libBuildPackageFileName,
                tempDir);

            string[] dirs = Directory.GetDirectories (tempDir);
            string zipDir = Path.GetFileName (dirs[0]);
            runner.CopyDirectoryStructure (tempDir.Add (zipDir), libIncludeDirectory, true);
        }

        //private static void TargetDocs(ConcreteBuildRunner runner, bool useDebugBin)
        //{
        //    runner.ProgramRunner
        //        .AddArgument("html")
        //        .AddArgument(@"-i=Docs")
        //        .AddArgument(@"-t=Docs\template.vm.html")
        //        .AddArgument(@"-o={0}\docs", runner.BuildPackagesDir)
        //        .AddArgument(@"-lf=Quiki.css");

        //    if (useDebugBin)
        //        runner.ProgramRunner.Run
        //            (PathBuilder.New("Quiki.Console").Add(@"bin\Debug").Add("Quiki.Console.exe"));
        //    else
        //        runner.ProgramRunner.Run(GetConsoleOutputPath(runner).Add("Quiki.Console.exe"));
        //}
    }
}