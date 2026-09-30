public static class PackageTests
{
    // Tests run for the each package
    public static List<PackageTest> GuiTests = new List<PackageTest>();
    public static List<PackageTest> EngineTests = new List<PackageTest>();

    // Define Package Tests
    //   Level 1 tests are run each time we build the packages
    //   Level 2 tests are run for PRs and when packages will be published
    //   Level 3 tests are run only when publishing a release

    static PackageTests()
    {
        //////////////////////////////////////////////////////////////////////
        // Tests of single assemblies targeting each runtime we support
        //////////////////////////////////////////////////////////////////////

        GuiTests.Add(new PackageTest(1, "Net462Test",
            "Run net462/mock-assembly.dll under .NET 4.6.2",
            "net462/mock-assembly.dll",
            MockAssemblyExpectedResult("Net462AgentLauncher")));

        GuiTests.Add(new PackageTest(1, "Net462X86Test",
            "Run net462/mock-assembly-x86.dll under .NET 4.6.2",
            "net462/mock-assembly-x86.dll",
            MockAssemblyX86ExpectedResult("Net462AgentLauncher")));

        if (BuildSettings.IsLocalBuild)
        {
            GuiTests.Add(new PackageTest(1, "NetCore31Test",
                "Run netcoreapp3.1/mock-assembly.dll under .NET 8.0",
                "netcoreapp3.1/mock-assembly.dll",
                MockAssemblyExpectedResult("Net80AgentLauncher")));

            GuiTests.Add(new PackageTest(1, "Net50Test",
                "Run net5.0/mock-assembly.dll under .NET 8.0",
                "net5.0/mock-assembly.dll",
                MockAssemblyExpectedResult("Net80AgentLauncher")));
        }

        GuiTests.Add(new PackageTest(1, "Net60Test",
            "Run net6.0/mock-assembly.dll under .NET 8.0",
            "net6.0/mock-assembly.dll",
            MockAssemblyExpectedResult("Net80AgentLauncher")));

        GuiTests.Add(new PackageTest(1, "Net70Test",
            "Run net7.0/mock-assembly.dll under .NET 8.0",
            "net7.0/mock-assembly.dll",
            MockAssemblyExpectedResult("Net80AgentLauncher")));

        GuiTests.Add(new PackageTest(1, "Net80Test",
            "Run net8.0/mock-assembly.dll under .NET 8.0",
            "net8.0/mock-assembly.dll",
            MockAssemblyExpectedResult("Net80AgentLauncher")));

        GuiTests.Add(new PackageTest(1, "Net90Test",
            "Run net9.0/mock-assembly.dll under .NET 9.0",
            "net9.0/mock-assembly.dll",
            MockAssemblyExpectedResult("Net90AgentLauncher")));

        GuiTests.Add(new PackageTest(1, "Net10Test",
            "Run net10.0/mock-assembly.dll under .NET 10.0",
            "net10.0/mock-assembly.dll",
            MockAssemblyExpectedResult("Net10AgentLauncher")));

        GuiTests.Add(new PackageTest(1, "Net60X86Test",
            "Run net6.0/mock-assembly-x86.dll under .NET 8.0",
            "net6.0/mock-assembly-x86.dll",
            MockAssemblyX86ExpectedResult("Net80AgentLauncher")));

        GuiTests.Add(new PackageTest(1, "Net70X86Test",
            "Run net7.0/mock-assembly-x86.dll under .NET 8.0",
            "net7.0/mock-assembly-x86.dll",
            MockAssemblyX86ExpectedResult("Net80AgentLauncher")));

        GuiTests.Add(new PackageTest(1, "Net80X86Test",
            "Run net8.0/mock-assembly-x86.dll under .NET 8.0",
            "net8.0/mock-assembly-x86.dll",
            MockAssemblyX86ExpectedResult("Net80AgentLauncher")));

        // TODO: Why doesn't this work on GitHub?
        if (BuildSettings.IsLocalBuild)
        {
            GuiTests.Add(new PackageTest(1, "Net90X86Test",
                "Run net9.0/mock-assembly-x86.dll under .NET 9.0",
                "net9.0/mock-assembly-x86.dll",
                MockAssemblyX86ExpectedResult("Net90AgentLauncher")));
        }

        GuiTests.Add(new PackageTest(1, "Net10X86Test",
            "Run net10.0/mock-assembly-x86.dll under .NET 10.0",
            "net10.0/mock-assembly-x86.dll",
            MockAssemblyX86ExpectedResult("Net10AgentLauncher")));

        //////////////////////////////////////////////////////////////////////
        // AspNetCore tests
        //////////////////////////////////////////////////////////////////////

        if (BuildSettings.IsLocalBuild)
        {
            GuiTests.Add(new PackageTest(1, "AspNetCore31Test",
                "Run netcoreapp3.1/aspnetcore-test.dll under .NET 8.0",
                "netcoreapp3.1/aspnetcore-test.dll",
                new ExpectedResult("Passed")
                {
                    Assemblies = new[] { new ExpectedAssemblyResult("aspnetcore-test.dll", "Net80AgentLauncher") }
                }));

            GuiTests.Add(new PackageTest(1, "AspNetCore50Test",
                "Run net5.0/aspnetcore-test.dll under .NET 8.0",
                "net5.0/aspnetcore-test.dll",
                new ExpectedResult("Passed")
                {
                    Assemblies = new[] { new ExpectedAssemblyResult("aspnetcore-test.dll", "Net80AgentLauncher") }
                }));
        }

        GuiTests.Add(new PackageTest(1, "AspNetCore60Test",
            "Run net6.0/aspnetcore-test.dll under .NET 8.0",
            "net6.0/aspnetcore-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("aspnetcore-test.dll", "Net80AgentLauncher") }
            }));

        GuiTests.Add(new PackageTest(1, "AspNetCore70Test",
            "Run net7.0/aspnetcore-test.dll under .NET 8.0",
            "net7.0/aspnetcore-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("aspnetcore-test.dll", "Net80AgentLauncher") }
            }));

        GuiTests.Add(new PackageTest(1, "AspNetCore80Test",
            "Run net8.0/aspnetcore-test.dll under .NET 8.0",
            "net8.0/aspnetcore-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("aspnetcore-test.dll", "Net80AgentLauncher") }
            }));


        GuiTests.Add(new PackageTest(1, "AspNetCore90Test",
            "Run net9.0/aspnetcore-test.dll under .NET 9.0",
            "net9.0/aspnetcore-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("aspnetcore-test.dll", "Net90AgentLauncher") }
            }));

        GuiTests.Add(new PackageTest(1, "AspNetCore10Test",
            "Run net10.0/aspnetcore-test.dll under .NET 10.0",
            "net10.0/aspnetcore-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("aspnetcore-test.dll", "Net10AgentLauncher") }
            }));

        //////////////////////////////////////////////////////////////////////
        // Windows Forms Tests
        //////////////////////////////////////////////////////////////////////

        if (BuildSettings.IsLocalBuild)
            GuiTests.Add(new PackageTest(1, "Net50WindowsFormsTest",
                "Run net5.0-windows/windows-forms-test.dll under .NET 8.0",
                "net5.0-windows/windows-forms-test.dll",
                new ExpectedResult("Passed")
                {
                    Assemblies = new[] { new ExpectedAssemblyResult("windows-forms-test.dll", "Net80AgentLauncher") }
                }));

        GuiTests.Add(new PackageTest(1, "Net60WindowsFormsTest",
            "Run net6.0-windows/windows-forms-test.dll under .NET 8.0",
            "net6.0-windows/windows-forms-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("windows-forms-test.dll", "Net80AgentLauncher") }
            }));

        GuiTests.Add(new PackageTest(1, "Net70WindowsFormsTest",
            "Run net7.0-windows/windows-forms-test.dll under .NET 8.0",
            "net7.0-windows/windows-forms-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("windows-forms-test.dll", "Net80AgentLauncher") }
            }));

        GuiTests.Add(new PackageTest(1, "Net80WindowsFormsTest",
            "Run net8.0-windows/windows-forms-test.dll under .NET 8.0",
            "net8.0-windows/windows-forms-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("windows-forms-test.dll", "Net80AgentLauncher") }
            }));

        GuiTests.Add(new PackageTest(1, "Net90WindowsFormsTest",
            "Run net9.0-windows/windows-forms-test.dll under .NET 9.0",
            "net9.0-windows/windows-forms-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("windows-forms-test.dll", "Net90AgentLauncher") }
            }));

        GuiTests.Add(new PackageTest(1, "Net10WindowsFormsTest",
            "Run net10.0-windows/windows-forms-test.dll under .NET 10.0",
            "net10.0-windows/windows-forms-test.dll",
            new ExpectedResult("Passed")
            {
                Assemblies = new[] { new ExpectedAssemblyResult("windows-forms-test.dll", "Net10AgentLauncher") }
            }));

        //////////////////////////////////////////////////////////////////////
        // Multiple assembly tests
        //////////////////////////////////////////////////////////////////////

        GuiTests.Add(new PackageTest(1, "Net462PlusNet10Test",
            "Run .NET 4.6.2 and .NET 10.0 builds of mock-assembly.dll together",
            "net462/mock-assembly.dll net10.0/mock-assembly.dll",
            MockAssemblyExpectedResult("Net462AgentLauncher", "Net10AgentLauncher")));

        //////////////////////////////////////////////////////////////////////
        // Tests that use extensions
        //////////////////////////////////////////////////////////////////////

        // V2 ResultWriter Tests
        GuiTests.Add(new PackageTest(1, "V2ResultWriterTest_Net462",
            "Run mock-assembly under .NET 4.6.2 and produce V2 output",
            "net462/mock-assembly.dll --result=TestResult.xml --result=NUnit2TestResult.xml;format=nunit2",
            MockAssemblyExpectedResult("Net462AgentLauncher"),
            KnownExtensions.NUnitV2ResultWriter));

        // TODO: Suppress V2 tests until driver is working
        //GuiTests.Add(new PackageTest(1, "NUnitV2Test",
        //    "Run mock-assembly.dll built for NUnit V2",
        //    "v2-tests/mock-assembly.dll",
        //    new ExpectedResult("Failed")
        //    {
        //        Total = 28,
        //        Passed = 18,
        //        Failed = 5,
        //        Warnings = 0,
        //        Inconclusive = 1,
        //        Skipped = 4
        //    },
        //	EngineExtensions.NUnitV2Driver));

        // TODO: Use --config option when it's supported by the extension.
        // Current test relies on the fact that the Release config appears
        // first in the project file.
        //if (BuildSettings.Configuration == "Release")
        //{
            GuiTests.Add(new PackageTest(1, "NUnitProjectTest",
                "Run an NUnit project",
                "../../TestProject.nunit",
                MockAssemblyExpectedResult(
                    "Net462AgentLauncher", "Net80AgentLauncher", "Net80AgentLauncher",
                    "Net80AgentLauncher", "Net90AgentLauncher", "Net10AgentLauncher"),
                KnownExtensions.NUnitProjectLoader));
        //}

        ExpectedResult MockAssemblyExpectedResult(params string[] agentNames)
        {
            int ncopies = agentNames.Length;

            var assemblies = new ExpectedAssemblyResult[ncopies];
            for (int i = 0; i < ncopies; i++)
                assemblies[i] = new ExpectedAssemblyResult("mock-assembly.dll", agentNames[i]);

            return new ExpectedResult("Failed")
            {
                Total = 42 * ncopies,
                Passed = 22 * ncopies,
                Failed = 7 * ncopies,
                Warnings = 1 * ncopies,
                Inconclusive = 5 * ncopies,
                Skipped = 7 * ncopies,
                Assemblies = assemblies
            };
        }

        ExpectedResult MockAssemblyX86ExpectedResult(params string[] agentNames)
        {
            int ncopies = agentNames.Length;

            var assemblies = new ExpectedAssemblyResult[ncopies];
            for (int i = 0; i < ncopies; i++)
                assemblies[i] = new ExpectedAssemblyResult("mock-assembly-x86.dll", agentNames[i]);

            return new ExpectedResult("Failed")
            {
                Total = 31 * ncopies,
                Passed = 18 * ncopies,
                Failed = 5 * ncopies,
                Warnings = 0 * ncopies,
                Inconclusive = 1 * ncopies,
                Skipped = 7 * ncopies,
                Assemblies = assemblies
            };
        }
    }
}
