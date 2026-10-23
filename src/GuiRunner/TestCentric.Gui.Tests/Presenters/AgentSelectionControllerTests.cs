// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

namespace TestCentric.Gui.Presenters
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using NSubstitute;
    using NUnit.Common;
    using NUnit.Engine;
    using NUnit.Framework;
    using TestCentric.Gui.Model;
    using TestCentric.Gui.Views;

    [TestFixture]
    internal class AgentSelectionControllerTests : PresenterTestBase<IMainView>
    {
        private ToolStripDropDownButton _runOnButton = null!;
        private AgentSelectionController _controller = null!;

        [SetUp]
        public void SetUp()
        {
            // Ensure that model always has at least an empty package with no settings
            _model.IsProjectLoaded.Returns(true);
            _model.TestCentricProject.Returns(new TestCentricProject("MyProject"));
            _model.TopLevelPackage.Returns(new NUnit.Engine.TestPackage());
            
            // Create a real ToolStripDropDownButton to get a real collection
            var toolStrip = new ToolStrip();
            _runOnButton = new ToolStripDropDownButton();
            toolStrip.Items.Add(_runOnButton);

            _view.RunOnButton.MenuItems.Returns(_runOnButton.DropDownItems);
            
            _controller = new AgentSelectionController(_model, _view);
        }

        [TearDown]
        public void TearDown()
        {
            _runOnButton?.Dispose();
        }

        #region Constructor Tests

        [Test]
        public void Constructor_InitializesController()
        {
            // 1. Arrange & Act
            var controller = new AgentSelectionController(_model, _view);

            // 2. Assert
            Assert.That(controller, Is.Not.Null);
        }

        #endregion

        #region AllowAgentSelection Tests

        [Test]
        public void AllowAgentSelection_ProjectLoadedWithNoAgents_ReturnsFalse()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string>());

            // 2. Act
            bool result = _controller.AllowAgentSelection();

            // 3. Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void AllowAgentSelection_ProjectLoadedWithOneAgent_ReturnsFalse()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> { "Agent1" });

            // 2. Act
            bool result = _controller.AllowAgentSelection();

            // 3. Assert
            Assert.That(result, Is.False);
        }

        [Test, Ignore("Needs work")] // TODO: Find a way to test this more narrowly, without involving main presenter
        public void AllowAgentSelection_ProjectLoadedWithMultipleAgents_ReturnsTrue()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> { "Agent1", "Agent2" });

            // 2. Act
            bool result = _controller.AllowAgentSelection();

            // 3. Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region PopulateMenu Tests

        [Test]
        public void PopulateMenu_NoAgentsAvailable_DefaultItemIsAdded()
        {
            // 1. Arrange
            _model.AvailableAgents.Returns(new List<string>());

            // 2. Act
            _controller.PopulateMenu();

            // 3. Assert
            Assert.That(_view.RunOnButton.MenuItems.Count, Is.EqualTo(1));

            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            Assert.That(defaultMenuItem!.Enabled, Is.True);
            Assert.That(defaultMenuItem.Tag, Is.EqualTo("DEFAULT"));
            Assert.That(defaultMenuItem.Checked, Is.True);
            Assert.That(defaultMenuItem.Text, Is.EqualTo("Default"));
        }

        [Test]
        public void PopulateMenu_WithAvailableAgents_AddsAllAgents()
        {
            // 1. Arrange
            _model.AvailableAgents.Returns(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2",
                "NUnit.TestAdapter.Agent3"
            });

            // 2. Act
            _controller.PopulateMenu();

            // 3. Assert
            Assert.That(_view.RunOnButton.MenuItems.Count, Is.EqualTo(4)); // Default + 3 agents

            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            Assert.That(defaultMenuItem!.Text, Is.EqualTo("Default"));
            Assert.That(defaultMenuItem.Tag, Is.EqualTo("DEFAULT"));

            var agent1MenuItem = _view.RunOnButton.MenuItems[1] as ToolStripMenuItem;
            Assert.That(agent1MenuItem!.Text, Is.EqualTo("Agent1"));
            Assert.That(agent1MenuItem.Tag, Is.EqualTo("NUnit.TestAdapter.Agent1"));

            var agent2MenuItem = _view.RunOnButton.MenuItems[2] as ToolStripMenuItem;
            Assert.That(agent2MenuItem!.Text, Is.EqualTo("Agent2"));
            Assert.That(agent2MenuItem.Tag, Is.EqualTo("NUnit.TestAdapter.Agent2"));

            var agent3MenuItem = _view.RunOnButton.MenuItems[3] as ToolStripMenuItem;
            Assert.That(agent3MenuItem!.Text, Is.EqualTo("Agent3"));
            Assert.That(agent3MenuItem.Tag, Is.EqualTo("NUnit.TestAdapter.Agent3"));
        }

        [Test]
        public void PopulateMenu_AgentWithoutDot_UsesFullNameAsDisplayName()
        {
            // 1. Arrange
            _model.AvailableAgents.Returns(new List<string> { "SimpleAgent" });

            // 2. Act
            _controller.PopulateMenu();

            // 3. Assert
            Assert.That(_view.RunOnButton.MenuItems.Count, Is.EqualTo(2));

            var agentMenuItem = _view.RunOnButton.MenuItems[1] as ToolStripMenuItem;
            Assert.That(agentMenuItem!.Text, Is.EqualTo("SimpleAgent"));
            Assert.That(agentMenuItem.Tag, Is.EqualTo("SimpleAgent"));
        }

        [Test]
        public void PopulateMenu_ClearsExistingItems()
        {
            // 1. Arrange
            _model.AvailableAgents.Returns(new List<string> { "Agent1" });
            _controller.PopulateMenu(); // First call

            _model.AvailableAgents.Returns(new List<string> { "Agent2", "Agent3" });

            // 2. Act
            _controller.PopulateMenu(); // Second call

            // 3. Assert
            Assert.That(_view.RunOnButton.MenuItems.Count, Is.EqualTo(3)); // Default + 2 new agents

            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            Assert.That(defaultMenuItem!.Text, Is.EqualTo("Default"));

            var agent2MenuItem = _view.RunOnButton.MenuItems[1] as ToolStripMenuItem;
            Assert.That(agent2MenuItem!.Text, Is.EqualTo("Agent2"));

            var agent3MenuItem = _view.RunOnButton.MenuItems[2] as ToolStripMenuItem;
            Assert.That(agent3MenuItem!.Text, Is.EqualTo("Agent3"));
        }

        #endregion

        #region UpdateMenuItems Tests

        [Test]
        public void UpdateMenuItems_NoProject_DisablesMenu()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string>());
            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            _view.RunOnButton.Received().Enabled = false;
        }

        [Test]
        public void UpdateMenuItems_OneAgentAvailable_DisablesMenu()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> { "Agent1" });
            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            _view.RunOnButton.Received().Enabled = false;
        }

        [Test]
        public void UpdateMenuItems_MultipleAgentsAvailable_EnablesMenu()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _model.AvailableAgents.Returns(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            _view.RunOnButton.Received().Enabled = true;
        }

        [Test]
        public void UpdateMenuItems_NoSpecificAgentIsSelected_ChecksDefaultAgentMenuItem()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _model.AvailableAgents.Returns(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            Assert.That(defaultMenuItem!.Checked, Is.True);
            Assert.That(defaultMenuItem.Enabled, Is.True);
        }

        [Test]
        public void UpdateMenuItems_SpecificAgentIsSelected_ChecksAgentMenuItem()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _model.TopLevelPackage!.Settings.Add(SettingDefinitions.SelectedAgentName.WithValue("NUnit.TestAdapter.Agent2"));
            
            _model.AvailableAgents.Returns(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            Assert.That(defaultMenuItem!.Checked, Is.False);

            var agent1MenuItem = _view.RunOnButton.MenuItems[1] as ToolStripMenuItem;
            Assert.That(agent1MenuItem!.Checked, Is.False);

            var agent2MenuItem = _view.RunOnButton.MenuItems[2] as ToolStripMenuItem;
            Assert.That(agent2MenuItem!.Checked, Is.True);
        }

        [Test]
        public void UpdateMenuItems_AgentNotSupportedByPackage_DisablesAgentMenuItem()
        {
            // 1. Arrange
            var project = new TestCentricProject("MyProject");
            _model.TestCentricProject.Returns(project);
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2"

            });
            
            _model.AvailableAgents.Returns(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2",
                "NUnit.TestAdapter.Agent3",
            });
            
            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            Assert.That(defaultMenuItem!.Enabled, Is.True);

            var agent1MenuItem = _view.RunOnButton.MenuItems[1] as ToolStripMenuItem;
            Assert.That(agent1MenuItem!.Enabled, Is.True);

            var agent2MenuItem = _view.RunOnButton.MenuItems[2] as ToolStripMenuItem;
            Assert.That(agent2MenuItem!.Enabled, Is.True);

            var agent3MenuItem = _view.RunOnButton.MenuItems[3] as ToolStripMenuItem;
            Assert.That(agent3MenuItem!.Enabled, Is.False);
        }

        [Test]
        public void UpdateMenuItems_DefaultAlwaysEnabled()
        {
            // 1. Arrange
            var project = new TestCentricProject("MyProject");
            _model.TestCentricProject.Returns(project);
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string>()); // No agents supported
            
            _model.AvailableAgents.Returns(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            Assert.That(defaultMenuItem!.Enabled, Is.True);
        }

        #endregion

        #region MenuItem Click Tests

        [Test]
        public void ClickingAgentMenuItem_UnchecksOtherItems()
        {
            // 1. Arrange
            var project = new TestCentricProject("MyProject");
            _model.TestCentricProject.Returns(project);
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _model.AvailableAgents.Returns(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _controller.PopulateMenu();
            _controller.UpdateMenuItems();

            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            var agent1MenuItem = _view.RunOnButton.MenuItems[1] as ToolStripMenuItem;

            // 2. Act
            agent1MenuItem!.PerformClick();

            // 3. Assert
            Assert.That(defaultMenuItem!.Checked, Is.False);
            Assert.That(agent1MenuItem!.Checked, Is.True);
        }

        [Test]
        public void ClickingAgentMenuItem_SetsProjectSettings()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _model.AvailableAgents.Returns(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _controller.PopulateMenu();

            var agent1MenuItem = _view.RunOnButton.MenuItems[1] as ToolStripMenuItem;

            // 2. Act
            agent1MenuItem!.PerformClick();

            // 3. Assert
            Assert.That(_model?.TopLevelPackage?.Settings.HasSetting(SettingDefinitions.SelectedAgentName.Name), Is.True);
            Assert.That(_model?.TopLevelPackage?.Settings.HasSetting(SettingDefinitions.RequestedAgentName.Name), Is.True);
        }

        [Test]
        public void ClickingDefaultMenuItem_RemovesAgentSettings()
        {
            // 1. Arrange
            _model.TopLevelPackage?.AddSetting(SettingDefinitions.SelectedAgentName.WithValue("SomeAgent"), recursive: true);
            _model.TopLevelPackage?.AddSetting(SettingDefinitions.RequestedAgentName.WithValue("SomeAgent"), recursive: true);
            
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> 
            { 
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2" 
            });
            
            _model.AvailableAgents.Returns(new List<string>());
            _controller.PopulateMenu();

            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            defaultMenuItem!.Checked = false; // Simulate it's not checked

            // 2. Act
            defaultMenuItem.PerformClick();

            // 3. Assert
            Assert.That(_model.TopLevelPackage?.Settings?.HasSetting(SettingDefinitions.SelectedAgentName.Name), Is.False);
            Assert.That(_model.TopLevelPackage?.Settings?.HasSetting(SettingDefinitions.RequestedAgentName.Name), Is.False);
        }

        [Test]
        public void ClickingAlreadyCheckedItem_DoesNothing()
        {
            // 1. Arrange
            var project = new TestCentricProject("MyProject");
            _model.TestCentricProject.Returns(project);
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string>());
            _model.AvailableAgents.Returns(new List<string>());
            
            _controller.PopulateMenu();

            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            Assert.That(defaultMenuItem!.Checked, Is.True);

            // 2. Act
            defaultMenuItem.PerformClick();

            // 3. Assert
            // When clicking an already checked item, LoadTests should not be called
            _model.DidNotReceive().LoadTests(Arg.Any<IList<string>>());
        }

        #endregion

        #region Button Text and ToolTip Tests

        [Test]
        public void PopulateMenu_SetsButtonTextToDefault()
        {
            // 1. Arrange
            _model.AvailableAgents.Returns(new List<string> { "NUnit.TestAdapter.Agent1" });

            // 2. Act
            _controller.PopulateMenu();

            // 3. Assert
            Assert.That(_view.RunOnButton.Text, Is.EqualTo("Run on: Default"));
        }

        [Test]
        public void PopulateMenu_DefaultItem_HasExplanatoryToolTip()
        {
            // 1. Arrange
            _model.AvailableAgents.Returns(new List<string>());

            // 2. Act
            _controller.PopulateMenu();

            // 3. Assert
            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            Assert.That(defaultMenuItem!.ToolTipText, Is.EqualTo(AgentSelectionController.DefaultAgentToolTip));
        }

        [Test]
        public void PopulateMenu_AgentItems_HaveFullLauncherNameAsToolTip()
        {
            // 1. Arrange
            _model.AvailableAgents.Returns(new List<string>
            {
                "NUnit.TestAdapter.Agent1",
                "SimpleAgent"
            });

            // 2. Act
            _controller.PopulateMenu();

            // 3. Assert
            var agent1MenuItem = _view.RunOnButton.MenuItems[1] as ToolStripMenuItem;
            Assert.That(agent1MenuItem!.ToolTipText, Is.EqualTo("NUnit.TestAdapter.Agent1"));

            var simpleAgentMenuItem = _view.RunOnButton.MenuItems[2] as ToolStripMenuItem;
            Assert.That(simpleAgentMenuItem!.ToolTipText, Is.EqualTo("SimpleAgent"));
        }

        [Test]
        public void UpdateMenuItems_NoProject_SetsButtonTextToDefault()
        {
            // 1. Arrange
            _model.IsProjectLoaded.Returns(false);
            _controller.PopulateMenu();
            _view.RunOnButton.Text = "Run on: Agent1";

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            Assert.That(_view.RunOnButton.Text, Is.EqualTo("Run on: Default"));
        }

        [Test]
        public void UpdateMenuItems_NoSpecificAgentIsSelected_SetsButtonTextToDefault()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string>
            {
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2"
            });

            _model.AvailableAgents.Returns(new List<string>
            {
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2"
            });

            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            Assert.That(_view.RunOnButton.Text, Is.EqualTo("Run on: Default"));
        }

        [TestCase("NUnit.TestAdapter.Agent2", "Run on: Agent2")]
        [TestCase("SimpleAgent", "Run on: SimpleAgent")]
        public void UpdateMenuItems_SpecificAgentIsSelected_SetsButtonText(string selectedAgent, string expectedText)
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string>
            {
                "NUnit.TestAdapter.Agent1",
                selectedAgent
            });

            _model.TopLevelPackage!.Settings.Add(SettingDefinitions.SelectedAgentName.WithValue(selectedAgent));

            _model.AvailableAgents.Returns(new List<string>
            {
                "NUnit.TestAdapter.Agent1",
                selectedAgent
            });

            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            Assert.That(_view.RunOnButton.Text, Is.EqualTo(expectedText));
        }

        [Test]
        public void UpdateMenuItems_OnlyOneAgentAvailable_ButtonTextStillShowsSelectedAgent()
        {
            // 1. Arrange
            _model.GetAgentsForPackage(null!).ReturnsForAnyArgs(new List<string> { "NUnit.TestAdapter.Agent1" });
            _model.TopLevelPackage!.Settings.Add(SettingDefinitions.SelectedAgentName.WithValue("NUnit.TestAdapter.Agent1"));
            _model.AvailableAgents.Returns(new List<string> { "NUnit.TestAdapter.Agent1" });

            _controller.PopulateMenu();

            // 2. Act
            _controller.UpdateMenuItems();

            // 3. Assert
            _view.RunOnButton.Received().Enabled = false;
            Assert.That(_view.RunOnButton.Text, Is.EqualTo("Run on: Agent1"));
        }

        [Test]
        public void ClickingAgentMenuItem_SetsButtonText()
        {
            // 1. Arrange
            _model.AvailableAgents.Returns(new List<string>
            {
                "NUnit.TestAdapter.Agent1",
                "NUnit.TestAdapter.Agent2"
            });

            _controller.PopulateMenu();

            var agent2MenuItem = _view.RunOnButton.MenuItems[2] as ToolStripMenuItem;

            // 2. Act
            agent2MenuItem!.PerformClick();

            // 3. Assert
            Assert.That(_view.RunOnButton.Text, Is.EqualTo("Run on: Agent2"));
        }

        [Test]
        public void ClickingDefaultMenuItem_SetsButtonTextToDefault()
        {
            // 1. Arrange
            _model.AvailableAgents.Returns(new List<string> { "NUnit.TestAdapter.Agent1" });

            _controller.PopulateMenu();

            var defaultMenuItem = _view.RunOnButton.MenuItems[0] as ToolStripMenuItem;
            var agent1MenuItem = _view.RunOnButton.MenuItems[1] as ToolStripMenuItem;
            agent1MenuItem!.PerformClick();

            // 2. Act
            defaultMenuItem!.PerformClick();

            // 3. Assert
            Assert.That(_view.RunOnButton.Text, Is.EqualTo("Run on: Default"));
            Assert.That(_model.TopLevelPackage?.Settings?.HasSetting(SettingDefinitions.SelectedAgentName.Name), Is.False);
        }

        #endregion
    }
}
