// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NUnit;
using TestCentric.Gui.Elements;
using TestCentric.Gui.Model;
using TestCentric.Gui.Views;

using SettingDefinitions = NUnit.Common.SettingDefinitions;

namespace TestCentric.Gui.Presenters
{
    public class AgentSelectionController
    {
        public const string DefaultAgentToolTip =
            "Runs each test assembly in a separate local process, using the runtime it was built for " +
            "or the next higher compatible runtime that has an agent.";

        private const string DEFAULT = "DEFAULT";

        private ITestModel _model;
        private IMainView _view;

        // True when the loaded package can run on more than one agent
        private bool _canSelectAgent;

        public AgentSelectionController(ITestModel model, IMainView view)
        {
            _model = model;
            _view = view;           
        }

        public bool AllowAgentSelection()
        {
            var package = _model.TopLevelPackage;
            return package != null && _model.GetAgentsForPackage(package).Count > 1;
        }

        public void PopulateMenu()
        {
            IPopup agentMenu = _view.RunOnButton;
            agentMenu.MenuItems.Clear();

            var defaultItem = new ToolStripMenuItem("Default");
            defaultItem.Tag = DEFAULT;
            defaultItem.ToolTipText = DefaultAgentToolTip;
            defaultItem.Checked = true;
            defaultItem.Enabled = true; // Should always remain enabled
            agentMenu.MenuItems.Add(defaultItem);
            defaultItem.Click += OnAgentMenuItemClicked;

            foreach (var agentFullName in _model.AvailableAgents)
            {
                var agentItem = new ToolStripMenuItem(GetDisplayName(agentFullName));
                agentItem.Tag = agentFullName;
                agentItem.ToolTipText = agentFullName;
                agentMenu.MenuItems.Add(agentItem);
                agentItem.Click += OnAgentMenuItemClicked;
            }

            UpdateToolTip(DEFAULT);
        }

        public void UpdateMenuItems()
        {
            IPopup agentMenu = _view.RunOnButton;
            _canSelectAgent = false;
            agentMenu.Enabled = false;
            if (!_model.IsProjectLoaded)
            {
                UpdateToolTip(DEFAULT);
                return;
            }

            IList<string> agentsToEnable = _model.GetAgentsForPackage(_model.TopLevelPackage);

            // "Requested" here only means what the user picked in the GUI, saved as
            // RequestedAgentName: that is all the GUI knows. It is not the engine's
            // distinction between the requested and the selected agent.
            string requestedAgent = _model.TopLevelPackage.Settings.GetValueOrDefault(SettingDefinitions.RequestedAgentName);
            if (string.IsNullOrEmpty(requestedAgent))
                requestedAgent = DEFAULT;

            UpdateToolTip(requestedAgent);

            _canSelectAgent = agentsToEnable.Count > 1;
            UpdateButtonState();
            if (_canSelectAgent)
            {
                foreach (ToolStripMenuItem item in agentMenu.MenuItems)
                {
                    if (item.Tag is string itemTag)
                    {
                        item.Enabled = itemTag == DEFAULT || agentsToEnable.Contains(itemTag);
                        item.Checked = itemTag == requestedAgent;
                    }
                }
            }
        }

        /// <summary>
        /// Enables the button only when an agent can be chosen and no tests are running.
        /// It only sets the button's Enabled state, so it is cheap. It is called on engine
        /// events too: the view element marshals the change to the UI thread.
        /// </summary>
        public void UpdateButtonState()
        {
            _view.RunOnButton.Enabled = _canSelectAgent && !_model.IsTestRunning;
        }

        private void UpdateToolTip(string requestedAgent)
        {
            string displayName = requestedAgent == DEFAULT ? "Default (local process)" : GetDisplayName(requestedAgent);
            _view.RunOnButton.ToolTipText = $"Run on: {displayName}";
        }

        private static string GetDisplayName(string agentFullName)
        {
            // The full name is not always provided in AvailableAgents
            int lastDot = agentFullName.LastIndexOf('.');
            return lastDot == -1
                ? agentFullName
                : agentFullName.Substring(lastDot + 1);
        }

        private void OnAgentMenuItemClicked(object? sender, EventArgs e)
        {
            Guard.OperationValid(_model.IsProjectLoaded, "No project is loaded");
            if (sender is ToolStripMenuItem item && !item.Checked)
            {
                EnsureSingleItemChecked(item);

                // Only RequestedAgentName is read by the engine. SelectedAgentName is
                // written by the engine itself, so a stale value is simply removed.
                _model.TopLevelPackage.RemoveSetting(SettingDefinitions.SelectedAgentName, recursive: true);
                _model.TopLevelPackage.RemoveSetting(SettingDefinitions.RequestedAgentName, recursive: true);

                if (item.Tag is string itemTag)
                {
                    if (itemTag != DEFAULT)
                        _model.TopLevelPackage.AddSetting(SettingDefinitions.RequestedAgentName.WithValue(itemTag), recursive: true);

                    UpdateToolTip(itemTag);
                }
            }

            void EnsureSingleItemChecked(ToolStripMenuItem itemToCheck)
            {
                foreach (ToolStripMenuItem item in _view.RunOnButton.MenuItems)
                    item.Checked = false;
                itemToCheck.Checked = true;
            }
        }
    }
}
