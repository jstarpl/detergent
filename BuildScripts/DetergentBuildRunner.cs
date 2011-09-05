using System.IO;
using Flubu.Builds;
using Flubu.Builds.VSSolutionBrowsing;

namespace BuildScripts
{
    public class DetergentBuildRunner : ConcreteBuildRunner
    {
        public DetergentBuildRunner(string productId) : base(productId)
        {
        }

        public DetergentBuildRunner CompileSolution2(string dotNetVersion)
        {
            ScriptExecutionEnvironment.LogTaskStarted("Compiling the solution");
            string msbuildPath = ScriptExecutionEnvironment.GetDotNetFWDir(dotNetVersion);

            ProgramRunner
                .AddArgument(MakePathFromRootDir(ProductId) + ".2010.sln")
                .AddArgument("/p:Configuration={0}", BuildConfiguration)
                .AddArgument("/consoleloggerparameters:NoSummary")
                .Run(Path.Combine(msbuildPath, @"msbuild.exe"));

            ScriptExecutionEnvironment.LogTaskFinished();
            return this;
        }

        public DetergentBuildRunner RunTests2(string projectName, bool collectCoverageData)
        {
            Log("Runs tests on '{0}' assembly ({1})", new object[] { projectName, collectCoverageData ? "with coverage" : "without coverage" });
            string buildLogsDir = EnsureBuildLogsTestDirectoryExists();
            VSProjectWithFileInfo testProjectWithFileInfo = (VSProjectWithFileInfo)Solution.FindProjectByName(projectName);
            string projectTargetPath = GetProjectOutputPath(testProjectWithFileInfo.ProjectName);
            projectTargetPath = Path.Combine(testProjectWithFileInfo.ProjectDirectoryPath, projectTargetPath) + testProjectWithFileInfo.ProjectName + ".dll";
            projectTargetPath = Path.GetFullPath(MakePathFromRootDir(projectTargetPath));
            string gallioEchoExePath = MakePathFromRootDir(Path.Combine(LibDir, @"Gallio\bin\Gallio.Echo.exe"));
            try
            {
                //if (collectCoverageData)
                //{
                //    ProgramRunner.AddArgument(Path.Combine(LibDir, @"NCover v1.5.8\CoverLib.dll")).AddArgument("/s").Run("regsvr32");
                //    ProgramRunner.AddArgument(gallioEchoExePath);
                //}
                ProgramRunner
                    .AddArgument(projectTargetPath)
                    .AddArgument("/runner:NCover")
                    .AddArgument("/report-directory:{0}", new object[] { buildLogsDir })
                    .AddArgument("/report-name-format:TestResults-{0}", new object[] { TestRuns })
                    .AddArgument("/report-type:xml")
                    .AddArgument("/verbosity:verbose");
                //if (collectCoverageData)
                //{
                //    ProgramRunner.AddArgument("//x").AddArgument(@"{0}\Coverage-{1}.xml", new object[] { buildLogsDir, TestRuns }).AddArgument("//ea").AddArgument("MbUnit.Framework.TestFixtureAttribute").AddArgument("//w").AddArgument(Path.GetDirectoryName(projectTargetPath)).AddArgument("//v").Run(MakePathFromRootDir(Path.Combine(libDir, @"NCover v1.5.8\NCover.Console.exe")));
                //    CoverageResultsExist = true;
                //}
                //else
                //{
                    ProgramRunner.Run(gallioEchoExePath);
                //}
                IncrementTestRunsCounter();
            }
            finally
            {
                //if (collectCoverageData)
                //{
                //    ProgramRunner.AddArgument(Path.Combine(LibDir, @"NCover v1.5.8\CoverLib.dll")).AddArgument("/s").AddArgument("/u").Run("regsvr32");
                //}
            }

            return this;
        }        
    }
}