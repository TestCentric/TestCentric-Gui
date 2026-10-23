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

            UpdateButtonText(DEFAULT);
        }

        public void UpdateMenuItems()
        {
            IPopup agentMenu = _view.RunOnButton;
            agentMenu.Enabled = false;
            if (!_model.IsProjectLoaded)
            {
                UpdateButtonText(DEFAULT);
                return;
            }

            IList<string> agentsToEnable = _model.GetAgentsForPackage(_model.TopLevelPackage);
            string selectedAgent = _model.TopLevelPackage.Settings.GetValueOrDefault(SettingDefinitions.SelectedAgentName);
            if (string.IsNullOrEmpty(selectedAgent))
                selectedAgent = DEFAULT;

            UpdateButtonText(selectedAgent);

            agentMenu.Enabled = agentsToEnable.Count > 1;
            if (agentMenu.Enabled)
            {
                foreach (ToolStripMenuItem item in agentMenu.MenuItems)
                {
                    if (item.Tag is string itemTag)
                    {
                        item.Enabled = itemTag == DEFAULT || agentsToEnable.Contains(itemTag);
                        item.Checked = itemTag == selectedAgent;
                    }
                }
            }
        }

        private void UpdateButtonText(string selectedAgent)
        {
            string displayName = selectedAgent == DEFAULT ? "Default" : GetDisplayName(selectedAgent);
            _view.RunOnButton.Text = $"Run on: {displayName}";
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

                // TODO: We currently need to use both settings. Investigate.
                _model.TopLevelPackage.RemoveSetting(SettingDefinitions.SelectedAgentName, recursive: true);
                _model.TopLevelPackage.RemoveSetting(SettingDefinitions.RequestedAgentName, recursive: true);

                if (item.Tag is string itemTag)
                {
                    if (itemTag != DEFAULT)
                    {
                        _model.TopLevelPackage.AddSetting(SettingDefinitions.SelectedAgentName.WithValue(itemTag), recursive: true);
                        _model.TopLevelPackage.AddSetting(SettingDefinitions.RequestedAgentName.WithValue(itemTag), recursive: true);
                    }

                    UpdateButtonText(itemTag);
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
