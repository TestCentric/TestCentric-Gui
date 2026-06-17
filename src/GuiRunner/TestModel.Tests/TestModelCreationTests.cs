// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.IO;
using NSubstitute;
using NUnit.Engine;
using NUnit.Framework;
using TestCentric.Gui.Model.Fakes;
using TestCentric.Tests;
using PackageSettings = NUnit.Engine.PackageSettings;
using SettingDefinitions = NUnit.Common.SettingDefinitions;

namespace TestCentric.Gui.Model
{
    public class TestModelCreationTests
    {
        private static readonly string PROJECT_PATH = Path.GetFullPath("dummy.tcproj");

        private ITestModel _model;
        private GuiOptions _options;
        private ITestEngine _engine;

        [TestCaseSource(nameof(TestCases))]
        public void CreateTestModelForAssembly(params string[] args)
        {
            CreateTestModel(args);

            // Create Project in memory, without saving
            _model.CreateNewProject(PROJECT_PATH, _options);

            CheckProjectAndPackageSettings();
        }

        [TestCaseSource(nameof(TestCases))]
        public void CreateTestModelForTestProject(params string[] args)
        {
            CreateTestModel(args);

            // Create, Save and then Open Project
            _model.CreateNewProject("MyProject", _options).SaveAs(PROJECT_PATH);
            _model.OpenExistingProject(PROJECT_PATH);

            CheckProjectAndPackageSettings();
        }

        private void CreateTestModel(string[] args)
        {
            _options = new GuiOptions(args);
            _engine = new MockTestEngine();
            _model = TestModel.CreateTestModel(_engine, _options);

            Assert.That(_model, Is.Not.Null, "Unable to create TestModel");
        }

        private void CheckProjectAndPackageSettings()
        {
            Assert.That(_model.TestCentricProject.ProjectPath, Is.EqualTo(PROJECT_PATH));
            Assert.That(_engine.WorkDirectory, Is.EqualTo(_options.WorkDirectory));
            Assert.That(_engine.InternalTraceLevel.ToString(), Is.EqualTo(_options.InternalTraceLevel ?? "Off"));

            var packageChecker = new PackageSettingsChecker(_model.TopLevelPackage.Settings);

            packageChecker.CheckSetting(_options.MaxAgents, SettingDefinitions.MaxAgents.Name);
            packageChecker.CheckSetting(_options.RunAsX86, SettingDefinitions.RunAsX86.Name);

            if (_options.TestParameters.Count > 0)
            {
                string[] parms = new string[_options.TestParameters.Count];
                int index = 0;
                foreach (string key in _options.TestParameters.Keys)
                    parms[index++] = $"{key}={_options.TestParameters[key]}";

                packageChecker.CheckSetting(_options.TestParameters, SettingDefinitions.TestParametersDictionary.Name);
            }
        }

        private class PackageSettingsChecker
        {
            PackageSettings _settings;

            public PackageSettingsChecker(PackageSettings settings)
            {
                _settings = settings;
            }

            public void CheckSetting(bool option, string key)
            {
                if (option || _settings.HasSetting(key))
                {
                    Assert.That(_settings.GetSetting(key), Is.EqualTo(option));
                }
            }

            public void CheckSetting<T>(T option, string key)
            {
                if (option != null)
                {
                    Assert.That(_settings.HasSetting(key));
                    Assert.That(_settings.GetSetting(key), Is.EqualTo(option));
                }
                else
                    Assert.That(_settings.HasSetting(key), Is.False);
            }
        }

        static string[][] TestCases = [
            ["dummy.dll"],
            ["dummy.dll", "--work=/Path/To/Directory"],
            ["dummy.dll", "--trace=Off"],
            ["dummy.dll", "--trace=Error", "--work=/Path/To/Directory"],
            ["dummy.dll", "--trace=Warning"],
            ["dummy.dll", "--trace=Info", "--work=/Path/To/Directory"],
            ["dummy.dll", "--trace=Debug"],
            ["dummy.dll", "--work=/Some/Directory", "--agents:32"],
            ["dummy.dll", "--agents:5"],
            ["dummy.dll", "--X86"],
            ["dummy.dll", "--param:X=5"],
            ["dummy.dll", "--param:X=5", "-p:Y=7"]
        ];
    }
}
