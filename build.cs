// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

// Cake.Sdk build of the GUI. Run with: dotnet run --file build.cs -- --target=Package

#:sdk Cake.Sdk@7.0.0
#:package TestCentric.Cake.Recipe@2.0.0-dev00002.4
// The GUI uses Central Package Management, which this build script must not.
#:property ManagePackageVersionsCentrally=false
#:property Nullable=disable
// The GUI's own build files. To test against a local recipe checkout instead of the
// package, replace the recipe's #:package line above with #:package lines for the
// Cake.Git and Cake.MinVer versions the recipe uses, and add
// ../TestCentric.Cake.Recipe/recipe/*.cs here.
#:property IncludeAdditionalFiles=PackageTests.cs;KnownExtensions.cs

//////////////////////////////////////////////////////////////////////
// INITIALIZATION
//////////////////////////////////////////////////////////////////////

BuildSettings.Initialize(
	context: Context,
	title: "TestCentric.GuiRunner",
	solutionFile: "TestCentric-Gui.sln",
	githubRepository: "TestCentric-Gui",
	exemptFiles: ["Resource.cs", "TextCode.cs"]
);

//////////////////////////////////////////////////////////////////////
// DEFINE PACKAGES
//////////////////////////////////////////////////////////////////////

var NuGetGuiPackage = new NuGetPackage(
	id: "TestCentric.GuiRunner",
	description: GUI_DESCRIPTION,
	packageContent: new PackageContent()
		.WithRootFiles("../../LICENSE.txt", "../../NOTICES.txt", "../../CHANGES.txt", "../../testcentric.png")
		.WithDirectories(
			new DirectoryContent("tools").WithFiles(
				"testcentric.exe", "testcentric.exe.config", "TestCentric.Gui.Runner.dll",
				"nunit.uiexception.dll", "TestCentric.Gui.Model.dll",
				"TestCentric.Metadata.dll", "NUnit.Extensibility.dll", "NUnit.Extensibility.Api.dll",
				"nunit.common.dll", "nunit.engine.api.dll", "nunit.engine.dll"),
			new DirectoryContent("tools/Images/Tree").WithFiles(
				"Images/Tree/Running.png", "Images/Tree/Pending.png", "Images/Tree/Running_Failure.png", "Images/Tree/Running_Success.png", "Images/Tree/Running_Warning.png", "Images/Tree/Running_Ignored.png"),
			new DirectoryContent("tools/Images/Tree/Circles").WithFiles(
				"Images/Tree/Circles/Success.png", "Images/Tree/Circles/Failure.png", "Images/Tree/Circles/Warning.png", "Images/Tree/Circles/Ignored.png", "Images/Tree/Circles/Inconclusive.png",
				"Images/Tree/Circles/Success_NotLatestRun.png", "Images/Tree/Circles/Failure_NotLatestRun.png", "Images/Tree/Circles/Warning_NotLatestRun.png", "Images/Tree/Circles/Ignored_NotLatestRun.png", "Images/Tree/Circles/Inconclusive_NotLatestRun.png",
				"Images/Tree/Circles/Skipped.png"),
			new DirectoryContent("tools/Images/Tree/Classic").WithFiles(
				"Images/Tree/Classic/Success.png", "Images/Tree/Classic/Failure.png", "Images/Tree/Classic/Warning.png", "Images/Tree/Classic/Ignored.png", "Images/Tree/Classic/Inconclusive.png",
				"Images/Tree/Classic/Success_NotLatestRun.png", "Images/Tree/Classic/Failure_NotLatestRun.png", "Images/Tree/Classic/Warning_NotLatestRun.png", "Images/Tree/Classic/Ignored_NotLatestRun.png", "Images/Tree/Classic/Inconclusive_NotLatestRun.png",
				"Images/Tree/Classic/Skipped.png"),
			new DirectoryContent("tools/Images/Tree/Visual Studio").WithFiles(
				"Images/Tree/Visual Studio/Success.png", "Images/Tree/Visual Studio/Failure.png", "Images/Tree/Visual Studio/Warning.png", "Images/Tree/Visual Studio/Ignored.png", "Images/Tree/Visual Studio/Inconclusive.png",
				"Images/Tree/Visual Studio/Success_NotLatestRun.png", "Images/Tree/Visual Studio/Failure_NotLatestRun.png", "Images/Tree/Visual Studio/Warning_NotLatestRun.png", "Images/Tree/Visual Studio/Ignored_NotLatestRun.png", "Images/Tree/Visual Studio/Inconclusive_NotLatestRun.png",
				"Images/Tree/Visual Studio/Skipped.png"))
		.WithDependencies(KnownExtensions.BundledNuGetAgents),
	testRunner: new GuiSelfTester(BuildSettings.PackageTestDirectory + "TestCentric.GuiRunner/TestCentric.GuiRunner." + BuildSettings.PackageVersion + "/tools/testcentric.exe"),
	checks: new PackageCheck[] {
		HasFiles("CHANGES.txt", "LICENSE.txt", "NOTICES.txt", "testcentric.png"),
		HasDirectory("tools").WithFiles(GUI_FILES).AndFile("TestCentric.Metadata.dll"),
        HasDirectory("tools/Images/Tree").WithFiles(COMMMON_ICONS_PNG),
        HasDirectory("tools/Images/Tree/Circles").WithFiles(IMG_SET_ICONS_PNG),
		HasDirectory("tools/Images/Tree/Classic").WithFiles(IMG_SET_ICONS_PNG),
		HasDirectory("tools/Images/Tree/Visual Studio").WithFiles(IMG_SET_ICONS_PNG)
	},
	tests: PackageTests.GuiTests
);

var ChocolateyGuiPackage = new ChocolateyPackage(
	id: "testcentric-gui",
	description: GUI_DESCRIPTION,
	summary: "A GUI runner for NUnit tests on .NET Framework and .NET.",
	packageContent: new PackageContent()
		.WithDirectories(
			new DirectoryContent("tools").WithFiles(
				"../../LICENSE.txt", "../../NOTICES.txt", "../../CHANGES.txt", "../../testcentric.png",
				"../../choco/VERIFICATION.txt",
				"../../choco/testcentric-agent.exe.ignore",	"../../choco/testcentric-agent-x86.exe.ignore",
				"testcentric.exe", "testcentric.exe.config", "TestCentric.Gui.Runner.dll",
				"nunit.uiexception.dll", "TestCentric.Gui.Model.dll", "nunit.engine.api.dll", "nunit.engine.dll",
				"TestCentric.Metadata.dll", "NUnit.Extensibility.dll", "NUnit.Extensibility.Api.dll", "NUnit.Common.dll"),
            new DirectoryContent("tools/Images/Tree").WithFiles(
				"Images/Tree/Running.png", "Images/Tree/Pending.png", "Images/Tree/Running_Failure.png", "Images/Tree/Running_Success.png", "Images/Tree/Running_Warning.png", "Images/Tree/Running_Ignored.png"),
            new DirectoryContent("tools/Images/Tree/Circles").WithFiles(
                "Images/Tree/Circles/Success.png", "Images/Tree/Circles/Failure.png", "Images/Tree/Circles/Warning.png", "Images/Tree/Circles/Ignored.png", "Images/Tree/Circles/Inconclusive.png", 
				"Images/Tree/Circles/Success_NotLatestRun.png", "Images/Tree/Circles/Failure_NotLatestRun.png", "Images/Tree/Circles/Warning_NotLatestRun.png", "Images/Tree/Circles/Ignored_NotLatestRun.png", "Images/Tree/Circles/Inconclusive_NotLatestRun.png", 
				"Images/Tree/Circles/Skipped.png"),
            new DirectoryContent("tools/Images/Tree/Classic").WithFiles(
                "Images/Tree/Classic/Success.png", "Images/Tree/Classic/Failure.png", "Images/Tree/Classic/Warning.png", "Images/Tree/Classic/Ignored.png", "Images/Tree/Classic/Inconclusive.png", 
				"Images/Tree/Classic/Success_NotLatestRun.png", "Images/Tree/Classic/Failure_NotLatestRun.png", "Images/Tree/Classic/Warning_NotLatestRun.png", "Images/Tree/Classic/Ignored_NotLatestRun.png", "Images/Tree/Classic/Inconclusive_NotLatestRun.png",
				"Images/Tree/Classic/Skipped.png"),
            new DirectoryContent("tools/Images/Tree/Visual Studio").WithFiles(
                "Images/Tree/Visual Studio/Success.png", "Images/Tree/Visual Studio/Failure.png", "Images/Tree/Visual Studio/Warning.png", "Images/Tree/Visual Studio/Ignored.png", "Images/Tree/Visual Studio/Inconclusive.png", 
				"Images/Tree/Visual Studio/Success_NotLatestRun.png", "Images/Tree/Visual Studio/Failure_NotLatestRun.png", "Images/Tree/Visual Studio/Warning_NotLatestRun.png", "Images/Tree/Visual Studio/Ignored_NotLatestRun.png", "Images/Tree/Visual Studio/Inconclusive_NotLatestRun.png", 
				"Images/Tree/Visual Studio/Skipped.png"))
        .WithDependencies( KnownExtensions.BundledChocolateyAgents ),
    testRunner: new GuiSelfTester(BuildSettings.PackageTestDirectory + "testcentric-gui/testcentric-gui." + BuildSettings.PackageVersion + "/tools/testcentric.exe"),
	checks: new PackageCheck[] {
		HasDirectory("tools").WithFiles("CHANGES.txt", "LICENSE.txt", "NOTICES.txt", "VERIFICATION.txt").AndFiles(GUI_FILES).AndFile("TestCentric.Metadata.dll"),
        HasDirectory("tools/Images/Tree").WithFiles(COMMMON_ICONS_PNG),
        HasDirectory("tools/Images/Tree/Circles").WithFiles(IMG_SET_ICONS_PNG),
		HasDirectory("tools/Images/Tree/Classic").WithFiles(IMG_SET_ICONS_PNG),
		HasDirectory("tools/Images/Tree/Visual Studio").WithFiles(IMG_SET_ICONS_PNG),
	},
	tests: PackageTests.GuiTests
);

BuildSettings.Packages.Add(NuGetGuiPackage);
// Temporarily pausing packaging for chocolatey
//BuildSettings.Packages.Add(ChocolateyGuiPackage);

//////////////////////////////////////////////////////////////////////
// ADDITIONAL TARGETS USED IN DEVELOPMENT
//////////////////////////////////////////////////////////////////////

DefineKnownExtensionTasks();

Task("PackageNuGet")
	.IsDependentOn("Build")
	.Does(() =>
	{
		NuGetGuiPackage.BuildVerifyAndTest();
	});

Task("PackageChocolatey")
	.IsDependentOn("Build")
	.Does(() =>
	{
		ChocolateyGuiPackage.BuildVerifyAndTest();
	});

// The following tasks install individual extensions in the bin directory
// where it will be found by both Debug and Release builds.

Task("InstallNUnitV2ResultWriter")
	.Description("Installs the NUnitV2ResultWriter extension.")
	.Does(() =>
	{
		KnownExtensions.NUnitV2ResultWriter.NuGetPackage.Install(BuildSettings.ProjectDirectory + BIN_DIR);
	});

Task("InstallNUnitProjectLoader")
	.Description("Installs the NUnitProjectLoader extension.")
	.Does(() =>
	{
		KnownExtensions.NUnitProjectLoader.NuGetPackage.Install(BuildSettings.ProjectDirectory + BIN_DIR);
	});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Build.Run();

//////////////////////////////////////////////////////////////////////
// DEFINITIONS USED ABOVE
//////////////////////////////////////////////////////////////////////

public static partial class Program
{
    // Common definitions used in both packages
    static readonly FilePath[] GUI_FILES = {
            "testcentric.exe", "testcentric.exe.config", "nunit.uiexception.dll",
            "TestCentric.Gui.Runner.dll", "TestCentric.Gui.Model.dll" };
    static readonly FilePath[] IMG_SET_ICONS_PNG = {
            "Success.png", "Failure.png", "Warning.png", "Ignored.png", "Inconclusive.png", "Success_NotLatestRun.png", "Failure_NotLatestRun.png", "Warning_NotLatestRun.png", "Ignored_NotLatestRun.png", "Inconclusive_NotLatestRun.png", "Skipped.png" };
    static readonly FilePath[] COMMMON_ICONS_PNG = { "Pending.png", "Running.png", "Running_Failure.png", "Running_Success.png", "Running_Warning.png", "Running_Ignored.png" };

    private const string GUI_DESCRIPTION = """
        The TestCentric Runner for NUnit (**TestCentric**) is a GUI runner aimed at supporting a range of .NET testing frameworks. In the 2.x release series, we are concentrating on support of NUnit tests.
        
        ### Features

        The V2 GUI is a completely new design from both V1 and the legacy NUnit GUI.

        The TestCentric engine, which was forked from NUnit, has been retired and we now rely entirely on the NUnit engine.

        In order to run tests, at least one NUnit agent must be available. The initial V2 release will include agents to support the .NET Framework as well as .NET 8.0, 9.0 and 10.0.
        
        Beyond agents, other types of NUnit engine extensions are supported, but are not bundled with the GUI itself and must be installed separately. In particular, to run NUnit V2 tests, install the **NUnit V2 Framework Driver Extension**.

        **Warning:** When using the GUI chocolatey package, **only** chocolatey-packaged extensions will be available. This is by design.

        ### Prerequisites

        **TestCentric** requires .NET Framework 4.6.2 or later. Tests targeting .NET Framework 2.0 through 4.8.1 run under the bundled .NET Framework agent, and tests targeting .NET Core or .NET 5 and later run under the bundled .NET agents.

        Test projects must reference the NUnit framework themselves; it is not included with **TestCentric**.
        """;


    // Package test runner
    public class GuiSelfTester : TestRunner, IPackageTestRunner
    {
        private FilePath _executablePath;

        // NOTE: When constructed as an argument to BuildSettings.Initialize(),
        // the executable path is not yet known and should not be provided.
        public GuiSelfTester(string executablePath = null)
        {
            _executablePath = executablePath;
        }

        public int RunPackageTest(string arguments)
        {
            if (!arguments.Contains(" --run"))
                arguments += " --run";
            if (!arguments.Contains(" --unattended"))
                arguments += " --unattended";

            if (_executablePath == null)
                _executablePath = BuildSettings.OutputDirectory + "testcentric.exe";

            Console.WriteLine($"Running {_executablePath} with arguments {arguments}");
            return base.RunTest(_executablePath, arguments);
        }
    }
}
