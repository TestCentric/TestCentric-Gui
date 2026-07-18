// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NUnit.Common;
using NUnit.Engine;
using TestCentric.Gui.Elements;
using TestCentric.Gui.Model;
using TestCentric.Gui.Views;

namespace TestCentric.Gui.Presenters
{
    internal class ConfigurationSelectionController
    {
        private ITestModel _model;
        private IMainView _view;

        public ConfigurationSelectionController(ITestModel model, IMainView view)
        {
            _model = model;
            _view = view;
        }

        public void PopulateMenu()
        {
            IPopup configurationMenu = _view.SelectConfigurationMenu;
            configurationMenu.MenuItems.Clear();

            string activeConfig = GetActiveConfiguration();
            foreach (string configName in GetPackageConfigurations())
            {
                var menuItem = new ToolStripMenuItem(configName);
                menuItem.Tag = configName;
                menuItem.Checked = configName == activeConfig;
                configurationMenu.MenuItems.Add(menuItem);
                menuItem.Click += OnConfigurationMenuItemClicked;
            }
        }

        private IList<string> GetPackageConfigurations()
        {
            var configurations = new List<string>();
            if (!_model.IsProjectLoaded)
                return configurations;

            foreach (TestPackage package in _model.TopLevelPackage.SubPackages)
            {
                string[] configNames = package.Settings.GetValueOrDefault(SettingDefinitions.ConfigNames).Split([';']);
                configurations.AddRange(configNames);
            }

            return configurations;
        }

        private string GetActiveConfiguration()
        {
            if (!_model.IsProjectLoaded)
                return string.Empty;
                        
            TestPackage package = _model.TopLevelPackage.SubPackages.FirstOrDefault();
            return package.Settings.GetValueOrDefault(SettingDefinitions.ActiveConfig);
        }

        private void OnConfigurationMenuItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            string config = item.Tag as string;
            if (item.Checked)
                return;

            EnsureSingleItemChecked(item);

            _model.TestCentricProject.ApplySetting(SettingDefinitions.ActiveConfig.WithValue(config));
            _model.ReloadTests();
        }

        void EnsureSingleItemChecked(ToolStripMenuItem itemToCheck)
        {
            foreach (ToolStripMenuItem item in _view.SelectAgentMenu.MenuItems)
                item.Checked = false;
            itemToCheck.Checked = true;
        }
    }
}
