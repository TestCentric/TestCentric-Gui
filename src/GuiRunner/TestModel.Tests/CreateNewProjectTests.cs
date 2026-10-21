// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Common;
using NUnit.Framework;
using TestCentric.Gui.Model.Fakes;

using SettingDefinitions = NUnit.Common.SettingDefinitions;

namespace TestCentric.Gui.Model
{
    using NUnit.Engine;

    public class CreateNewProjectTests
    {
        private TestModel _model = null!;

        [SetUp]
        public void CreateModel()
        {
            _model = new TestModel(new MockTestEngine());
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists("temp.tcproj"))
                File.Delete("temp.tcproj");

            _model.Dispose();
        }

        [TestCase("my.test.assembly.dll")]
        [TestCase("one.dll", "two.dll", "three.dll")]
        [TestCase("tests.nunit")]
        public void PackageContainsOneSubPackagePerTestFile(params string[] testFiles)
        {
            _model.CreateNewProject("MyProject", testFiles);

            Assert.That(_model.TestCentricProject?.TestFiles, Is.EqualTo(testFiles));
        }

        [Test]
        public void DisplayShadowCopySettings()
        {
            Console.WriteLine($"Current AppDomain has ShadowCopyFiles = {AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles}");
        }

        [Test]
        public void DisplayTestParameters()
        {
            Console.WriteLine("Test Parameters for this run:");
            foreach (string name in TestContext.Parameters.Names)
                Console.WriteLine($"{name}={TestContext.Parameters[name]}");
        }

        [TestCase("my.dll")]
        // TODO: FIX
        //[TestCase("my.sln")]
        //[TestCase("my.dll", "my.sln")]
        //[TestCase("my.sln", "my.dll")]
        //[TestCase("my.sln", "another.sln")]
        public void PackageForSolutionFileHasSkipNonTestAssemblies(params string[] files)
        {
            _model.CreateNewProject("MyProject", files);
            string skipKey = SettingDefinitions.SkipNonTestAssemblies.Name;

            foreach (var subpackage in _model.TopLevelPackage!.SubPackages)
            {
                if (subpackage.Name!.EndsWith(".sln"))
                {
                    Assert.That(subpackage.Settings.HasSetting(skipKey));
                    Assert.That(subpackage.Settings.GetSetting(skipKey), Is.True);
                }
                else
                    Assert.That(subpackage.Settings.HasSetting(skipKey), Is.False);
            }
        }

        [Test]
        public void RemoveSubPackage_PackagesIsDecreased()
        {
            var project = _model.CreateNewProject("MyProject", "dummy.dll", "dummy2.dll");
            project.SaveAs("temp.tcproj");

            var subPackage = project.TopLevelPackage.SubPackages[0];
            project.RemoveSubPackage(subPackage);
            Assert.That(project.TopLevelPackage.SubPackages.Count, Is.EqualTo(1));
        }
    }
}
