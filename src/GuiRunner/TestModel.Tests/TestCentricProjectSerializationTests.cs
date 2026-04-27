// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.IO;
using System.Xml;
using NUnit.Engine;
using NUnit.Framework;
using Org.XmlUnit.Constraints;
using TestCentric.Gui.Model.Fakes;

namespace TestCentric.Gui.Model
{
    public class TestCentricProjectSerializationTests
    {
        private const string CR = "\r";
        private const string LF = "\n";

        private const string PROJ_NAME = "MyProject.tcproj";
        private static readonly string PROJ_DIR = RandomDirectory();
        private static readonly string NEW_PROJ_DIR = RandomDirectory();
        private static readonly string PROJ_PATH = Path.Combine(PROJ_DIR, PROJ_NAME);
        private static readonly string NEW_PROJ_PATH = Path.Combine(NEW_PROJ_DIR, PROJ_NAME);

        private static readonly string FILE1_PATH = Path.Combine(PROJ_DIR, "test1.dll");
        private static readonly string FILE2_PATH = Path.Combine(PROJ_DIR, "bin", "test2.dll");
        private static readonly string NEW_FILE1_PATH = Path.Combine(NEW_PROJ_DIR, "test1.dll");
        private static readonly string NEW_FILE2_PATH = Path.Combine(NEW_PROJ_DIR, "bin", "test2.dll");

        private TestCentricProject _testProject;
        private string _testProjectXml;

        [OneTimeSetUp]
        public void CreateProjectData()
        {
            Console.WriteLine($"PROJ_PATH = {PROJ_PATH}");
            Console.WriteLine($"NEW_PROJ_PATH = {NEW_PROJ_PATH}");

            _testProject = new TestCentricProject(new TestModel(new MockTestEngine()), PROJ_PATH, FILE1_PATH, FILE2_PATH);
            Directory.CreateDirectory(Path.Combine(PROJ_DIR, "bin"));
            Directory.CreateDirectory(Path.Combine(NEW_PROJ_DIR, "bin"));

            var package = _testProject.TopLevelPackage;
            var subPackages = package.SubPackages;
            Assert.That(subPackages.Count, Is.EqualTo(2));

            // Add settings intended for top level and both subpackages.
            // All these settings are non-standard, defined by the user.
            package.AddSetting("foo", "bar");
            package.AddSetting("num", 42);
            package.AddSetting("critical", true);

            // Add a setting to the first subpackage only
            subPackages[0].AddSetting("cpu", "x86");

            // Multi-line for ease of editing only, CR & LF removed
            _testProjectXml = $"""
                <?xml version = "1.0" encoding="utf-8"?>
                <TestCentricProject OriginalPath="{PROJ_PATH}">
                <TestPackage id="{package.ID}">
                <Settings foo="bar" num="42" critical="True" />
                <TestPackage id="{subPackages[0].ID}" fullname="{FILE1_PATH}">
                <Settings foo="bar" num="42" critical="True" cpu="x86" /></TestPackage>
                <TestPackage id="{subPackages[1].ID}" fullname="{FILE2_PATH}">
                <Settings foo="bar" num="42" critical="True" /></TestPackage></TestPackage>
                </TestCentricProject>
                """.Replace(CR, string.Empty).Replace(LF, string.Empty);

        }

        [SetUp]
        public void CreateTestFiles()
        {
            File.Create(FILE1_PATH).Close();
            File.Create(FILE2_PATH).Close();
        }

        [TearDown]
        public void CleanUp()
        {
            string[] files = [PROJ_PATH, NEW_PROJ_PATH, FILE1_PATH, FILE2_PATH, NEW_FILE1_PATH, NEW_FILE2_PATH];
            foreach (string file in files)
                if (File.Exists(file))
                    File.Delete(file);
        }

        [Test]
        public void SaveAndReloadTestProject()
        {
            // Save the project
            _testProject.Save();

            // Load the saved file as a document and check content directly
            var doc = new XmlDocument();
            doc.Load(PROJ_PATH);
            Assert.That(doc.OuterXml,
                CompareConstraint.IsIdenticalTo(_testProjectXml));

            CheckSavedProject(PROJ_PATH);
        }

        [Test]
        public void SaveAsAndReloadTestProject()
        {
            // Save the project to a new path
            _testProject.SaveAs(NEW_PROJ_PATH);

            // Load the saved file as a document and check content directly
            var doc = new XmlDocument();
            doc.Load(NEW_PROJ_PATH);
            Assert.That(doc.OuterXml,
                CompareConstraint.IsIdenticalTo(_testProjectXml.Replace(PROJ_PATH, NEW_PROJ_PATH)));

            CheckSavedProject(NEW_PROJ_PATH);
        }

        [Test]
        public void SaveMoveAndReloadTestProject_TestFilesNotMoved()
        {
            // Fake the ProjectPath and save the project
            _testProject.ProjectPath = PROJ_PATH;
            _testProject.Save();

            File.Move(PROJ_PATH, NEW_PROJ_PATH);

            // Load the saved file as a document and check content directly
            var doc = new XmlDocument();
            doc.Load(NEW_PROJ_PATH);
            Assert.That(doc.OuterXml,
                CompareConstraint.IsIdenticalTo(_testProjectXml));

            var subPackages = _testProject.TopLevelPackage.SubPackages;
            Assert.That(subPackages[0].FullName, Is.EqualTo(FILE1_PATH));

            CheckSavedProject(NEW_PROJ_PATH);
        }

        [Test]
        public void SaveMoveAndReloadTestProject_TestFilesMoved()
        {
            // Fake the ProjectPath and save the project
            _testProject.ProjectPath = PROJ_PATH;
            _testProject.Save();

            File.Move(PROJ_PATH, NEW_PROJ_PATH);
            File.Move(FILE1_PATH, NEW_FILE1_PATH);
            File.Move(FILE2_PATH, NEW_FILE2_PATH);

            // Load the saved file as a document and check content directly
            var doc = new XmlDocument();
            doc.Load(NEW_PROJ_PATH);
            Assert.That(doc.OuterXml,
                CompareConstraint.IsIdenticalTo(_testProjectXml));

            var subPackages = _testProject.TopLevelPackage.SubPackages;
            Assert.That(subPackages[0].FullName, Is.EqualTo(FILE1_PATH));

            CheckSavedProject(NEW_PROJ_PATH);
        }

        private void CheckSavedProject(string projectPath)
        {
            // Load the saved file into a new project and check that
            var newProject = TestCentricProject.LoadFrom(projectPath);
            Assert.That(newProject.ProjectPath, Is.EqualTo(projectPath));
            Assert.That(newProject.TopLevelPackage, Is.Not.Null);

            var subPackages = newProject.TopLevelPackage.SubPackages;

            Assert.That(subPackages.Count, Is.EqualTo(2));
            Assert.That(subPackages[0].Settings.GetSetting("foo"), Is.EqualTo("bar"));
            Assert.That(subPackages[0].Settings.GetSetting("num"), Is.EqualTo(42));
            Assert.That(subPackages[0].Settings.GetSetting("critical"), Is.EqualTo(true));
            Assert.That(subPackages[0].Settings.GetSetting("cpu"), Is.EqualTo("x86"));

            Assert.That(subPackages[1].Settings.GetSetting("foo"), Is.EqualTo("bar"));
            Assert.That(subPackages[1].Settings.GetSetting("num"), Is.EqualTo(42));
            Assert.That(subPackages[1].Settings.GetSetting("critical"), Is.EqualTo(true));

            Assert.That(File.Exists(subPackages[0].FullName), $"File not found: {subPackages[0].FullName}");
            Assert.That(File.Exists(subPackages[1].FullName), $"File not found: {subPackages[1].FullName}");
        }

        [Test]
        public void SaveAndReloadEmptyTestProject()
        {
            var project = new TestCentricProject(new TestModel(new MockTestEngine()), "MyProject");
            project.SaveAs(PROJ_PATH);

            var expectedXml = $"""
                <?xml version="1.0" encoding="utf-8"?>
                <TestCentricProject OriginalPath="{PROJ_PATH}">
                <TestPackage id="{project.TopLevelPackage.ID}" />
                </TestCentricProject>
                """.Replace(CR, string.Empty).Replace(LF, string.Empty);

            // Load the saved file as a document and check content directly
            var doc = new XmlDocument();
            doc.Load(PROJ_PATH);
            Assert.That(doc.OuterXml, CompareConstraint.IsIdenticalTo(expectedXml));

            // Load the saved file into a new project and check that
            var newProject = TestCentricProject.LoadFrom(PROJ_PATH);
            Assert.That(newProject.ProjectPath, Is.EqualTo(PROJ_PATH));
            Assert.That(newProject.TopLevelPackage.ID, Is.EqualTo(project.TopLevelPackage.ID));
            Assert.That(newProject.TopLevelPackage.HasSubPackages, Is.False);
        }

        private static string RandomDirectory(string parent = null)
        {
            if (parent == null) parent = Path.GetTempPath();
            return Directory.CreateDirectory(Path.Combine(parent, Path.GetRandomFileName())).FullName;
        }
    }
}
