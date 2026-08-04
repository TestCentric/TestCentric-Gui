// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Windows.Forms;
using NSubstitute;
using NUnit.Common;
using NUnit.Engine;
using NUnit.Framework;
using TestCentric.Gui.Model;
using TestCentric.Gui.Views;

namespace TestCentric.Gui.Presenters
{
    [TestFixture]
    internal class ConfigurationSelectionControllerTests
    {
        private ITestModel _model;
        private IMainView _view;
        private ToolStripMenuItem _selectConfigurationMenuItem;
        private TestPackage _subPackage;

        [SetUp]
        public void SetUp()
        {
            _model = Substitute.For<ITestModel>();
            _view = Substitute.For<IMainView>();

            // Create a real ToolStripMenuItem to get a real collection
            var menuStrip = new MenuStrip();
            _selectConfigurationMenuItem = new ToolStripMenuItem();
            menuStrip.Items.Add(_selectConfigurationMenuItem);
            _view.SelectConfigurationMenu.MenuItems.Returns(_selectConfigurationMenuItem.DropDownItems);

            // Set up project
            _model.IsProjectLoaded.Returns(true);
            _model.TestCentricProject.Returns(new TestCentricProject("MyProject"));
            _model.TopLevelPackage.Returns(new TestPackage());

            _subPackage = new TestPackage("SubPackage");
            _model.TopLevelPackage.AddSubPackage(_subPackage);
        }

        [TearDown]
        public void TearDown()
        {
            _selectConfigurationMenuItem?.Dispose();
        }

        [Test]
        public void PopulateMenu_NoProjectIsLoaded_MenuIsDisabled()
        {
            // Arrange
            var controller = new ConfigurationSelectionController(_model, _view);
            _model.IsProjectLoaded.Returns(false);

            // Act
            controller.PopulateMenu();

            // Assert
            Assert.That(_view.SelectConfigurationMenu.Enabled, Is.False);
        }

        [Test]
        public void PopulateMenu_ProjectContainsNoConfiguration_MenuIsDisabled()
        {
            // Arrange
            var controller = new ConfigurationSelectionController(_model, _view);

            // Act
            controller.PopulateMenu();

            // Assert
            Assert.That(_view.SelectConfigurationMenu.Enabled, Is.False);
        }

        [Test]
        public void PopulateMenu_ProjectContainsOneConfiguration_MenuItemsAreCreated()
        {
            // Arrange
            var controller = new ConfigurationSelectionController(_model, _view);
            _subPackage.Settings.Add(SettingDefinitions.ConfigNames.WithValue("Debug"));

            // Act
            controller.PopulateMenu();

            // Assert
            Assert.That(_view.SelectConfigurationMenu.Enabled, Is.True);
            Assert.That(_selectConfigurationMenuItem.DropDownItems.Count, Is.EqualTo(1));
            Assert.That(_selectConfigurationMenuItem.DropDownItems[0].Text, Is.EqualTo("Debug"));
        }

        [Test]
        public void PopulateMenu_ProjectContainsTwoConfigurations_MenuItemsAreCreated()
        {
            // Arrange
            var controller = new ConfigurationSelectionController(_model, _view);
            _subPackage.Settings.Add(SettingDefinitions.ConfigNames.WithValue("Debug;Release"));

            // Act
            controller.PopulateMenu();

            // Assert
            Assert.That(_view.SelectConfigurationMenu.Enabled, Is.True);
            Assert.That(_selectConfigurationMenuItem.DropDownItems.Count, Is.EqualTo(2));
            Assert.That(_selectConfigurationMenuItem.DropDownItems[0].Text, Is.EqualTo("Debug"));
            Assert.That(_selectConfigurationMenuItem.DropDownItems[1].Text, Is.EqualTo("Release"));
        }

        [Test]
        public void PopulateMenu_ProjectContainsMultipleSubPackages_MenuItemsAreCreated()
        {
            // Arrange
            var controller = new ConfigurationSelectionController(_model, _view);

            var subPackage2 = new TestPackage("SubPackage2");
            _model.TopLevelPackage.AddSubPackage(subPackage2);

            _subPackage.Settings.Add(SettingDefinitions.ConfigNames.WithValue("Debug;Release"));
            subPackage2.Settings.Add(SettingDefinitions.ConfigNames.WithValue("Debug;CustomConfig"));

            // Act
            controller.PopulateMenu();

            // Assert
            Assert.That(_view.SelectConfigurationMenu.Enabled, Is.True);
            Assert.That(_selectConfigurationMenuItem.DropDownItems.Count, Is.EqualTo(3));
            Assert.That(_selectConfigurationMenuItem.DropDownItems[0].Text, Is.EqualTo("Debug"));
            Assert.That(_selectConfigurationMenuItem.DropDownItems[1].Text, Is.EqualTo("Release"));
            Assert.That(_selectConfigurationMenuItem.DropDownItems[2].Text, Is.EqualTo("CustomConfig"));
        }
    }
}
