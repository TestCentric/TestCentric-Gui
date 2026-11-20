// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

namespace TestCentric.Gui.Presenters
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Windows.Forms;
    using NSubstitute;
    using NUnit.Framework;
    using TestCentric.Gui.Elements;
    using TestCentric.Gui.Model;
    using TestCentric.Gui.Model.Settings;

    [TestFixture]
    internal class RecentFileMenuControllerTests
    {
        private ITestModel _model = null!;
        private IUserSettings _settings = null!;
        private IGuiSettings _guiSettings = null!;
        private IRecentFiles _recentFiles = null!;
        private IPopup _popup = null!;
        private ToolStripMenuItem _menuItem = null!;
        private string _testDirectory = null!;

        [SetUp]
        public void SetUp()
        {
            _model = Substitute.For<ITestModel>();
            _settings = Substitute.For<IUserSettings>();
            _guiSettings = Substitute.For<IGuiSettings>();
            _recentFiles = Substitute.For<IRecentFiles>();
            _popup = Substitute.For<IPopup>();
            _menuItem = new ToolStripMenuItem();
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _model.Settings.Returns(_settings);
            _settings.Gui.Returns(_guiSettings);
            _guiSettings.RecentFiles.Returns(_recentFiles);
            _recentFiles.Entries = new StringCollection();
            _popup.MenuItems.Returns(_menuItem.DropDownItems);
        }

        [TearDown]
        public void TearDown()
        {
            _menuItem.Dispose();
            Directory.Delete(_testDirectory, true);
        }

        [Test]
        public void Constructor_NullModel_ThrowsArgumentNullException()
        {
            Assert.That(() => new RecentFileMenuController(null!, _popup, true),
                Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_NullMenuItem_ThrowsArgumentNullException()
        {
            Assert.That(() => new RecentFileMenuController(_model, null!, true),
                Throws.ArgumentNullException);
        }

        [Test]
        public void PopulateMenu_ProjectFilterOnlyAddsProjectFiles()
        {
            string projectFile = CreateFile("project.tcproj");
            string assemblyFile = CreateFile("tests.dll");
            _recentFiles.Entries.Add(projectFile);
            _recentFiles.Entries.Add(assemblyFile);

            new RecentFileMenuController(_model, _popup, true).PopulateMenu();

            Assert.That(_menuItem.DropDownItems.Count, Is.EqualTo(3));
            Assert.That(_menuItem.DropDownItems[0].Text, Is.EqualTo($"1 {projectFile}"));
            Assert.That(_menuItem.DropDownItems[1], Is.TypeOf<ToolStripSeparator>());
            Assert.That(_menuItem.DropDownItems[2].Text, Is.EqualTo("Clear recent file list"));
        }

        [Test]
        public void PopulateMenu_FileFilterOnlyAddsNonProjectFiles()
        {
            string projectFile = CreateFile("project.tcproj");
            string assemblyFile = CreateFile("tests.dll");
            _recentFiles.Entries.Add(projectFile);
            _recentFiles.Entries.Add(assemblyFile);

            new RecentFileMenuController(_model, _popup, false).PopulateMenu();

            Assert.That(_menuItem.DropDownItems.Count, Is.EqualTo(3));
            Assert.That(_menuItem.DropDownItems[0].Text, Is.EqualTo($"1 {assemblyFile}"));
        }

        [Test]
        public void PopulateMenu_RemovesFilesThatNoLongerExist()
        {
            string existingFile = CreateFile("existing.dll");
            _recentFiles.Entries.Add(Path.Combine(_testDirectory, "missing.dll"));
            _recentFiles.Entries.Add(existingFile);

            new RecentFileMenuController(_model, _popup, false).PopulateMenu();

            Assert.That(_recentFiles.Entries, Is.EqualTo(new StringCollection { existingFile }));
            Assert.That(_menuItem.DropDownItems[0].Text, Is.EqualTo($"1 {existingFile}"));
        }

        [Test]
        public void RecentFileMenuItem_Clicks_OpenTheSelectedFile()
        {
            string fileName = CreateFile("tests.dll");
            _recentFiles.Entries.Add(fileName);

            new RecentFileMenuController(_model, _popup, false).PopulateMenu();
            ((ToolStripMenuItem)_menuItem.DropDownItems[0]).PerformClick();

            _model.Received(1).OpenExistingFile(fileName);
        }

        [Test]
        public void ClearRecentFileList_RemovesOnlyEntriesDisplayedByTheController()
        {
            string projectFile = CreateFile("project.tcproj");
            string assemblyFile = CreateFile("tests.dll");
            _recentFiles.Entries.Add(projectFile);
            _recentFiles.Entries.Add(assemblyFile);

            new RecentFileMenuController(_model, _popup, true).PopulateMenu();
            ((ToolStripMenuItem)_menuItem.DropDownItems[2]).PerformClick();

            Assert.That(_recentFiles.Entries, Is.EqualTo(new StringCollection { assemblyFile }));
        }

        [Test]
        public void OnKeyDown_Delete_RemovesSelectedFileAndSelectsPreviousFile()
        {
            string firstFile = CreateFile("first.dll");
            string secondFile = CreateFile("second.dll");
            _recentFiles.Entries.Add(firstFile);
            _recentFiles.Entries.Add(secondFile);
            var controller = new RecentFileMenuController(_model, _popup, false);

            controller.PopulateMenu();
            var selectedItem = (ToolStripMenuItem)_menuItem.DropDownItems[1];
            selectedItem.Select();
            var keyEventArgs = new KeyEventArgs(Keys.Delete);

            controller.OnKeyDown(null!, keyEventArgs);

            Assert.That(_recentFiles.Entries, Is.EqualTo(new StringCollection { firstFile }));
            Assert.That(_menuItem.DropDownItems[0].Text, Is.EqualTo($"1 {firstFile}"));
            Assert.That(((ToolStripMenuItem)_menuItem.DropDownItems[0]).Selected, Is.True);
            Assert.That(keyEventArgs.Handled, Is.True);
            Assert.That(keyEventArgs.SuppressKeyPress, Is.True);
        }

        [Test]
        public void OnKeyDown_Delete_RemovesLastFileAndClearsMenu()
        {
            string fileName = CreateFile("tests.dll");
            _recentFiles.Entries.Add(fileName);
            var controller = new RecentFileMenuController(_model, _popup, false);

            controller.PopulateMenu();
            ((ToolStripMenuItem)_menuItem.DropDownItems[0]).Select();
            var keyEventArgs = new KeyEventArgs(Keys.Delete);

            controller.OnKeyDown(null!, keyEventArgs);

            Assert.That(_recentFiles.Entries, Is.Empty);
            Assert.That(_menuItem.DropDownItems, Is.Empty);
            Assert.That(keyEventArgs.Handled, Is.True);
            Assert.That(keyEventArgs.SuppressKeyPress, Is.True);
        }

        private string CreateFile(string fileName)
        {
            string path = Path.Combine(_testDirectory, fileName);
            File.WriteAllText(path, string.Empty);
            return path;
        }
    }
}
