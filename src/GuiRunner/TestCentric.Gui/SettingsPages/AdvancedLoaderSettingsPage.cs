// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Security.Principal;
using System.Windows.Forms;
using NUnit.Common;

namespace TestCentric.Gui.SettingsPages
{
    using System.Diagnostics.CodeAnalysis;
    using NUnit;
    using NUnit.Engine;

    public partial class AdvancedLoaderSettingsPage : SettingsPage
    {


        public AdvancedLoaderSettingsPage(string key) : base(key)
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            PackageSettings = null!;
        }

        private PackageSettings PackageSettings { get; set; }

        public override void LoadSettings()
        {
            var package = Model.TopLevelPackage.ShouldNotBeNull();
            PackageSettings = package.Settings;

            // Update UI elements based on current settings in TestCentricProject
            int agents = PackageSettings.GetValueOrDefault(SettingDefinitions.MaxAgents);
            numberOfAgentsCheckBox.Checked = agents > 0;
            numberOfAgentsUpDown.Value = agents;

            string principalPolicy = PackageSettings.GetValueOrDefault(SettingDefinitions.PrincipalPolicy);
            if (string.IsNullOrEmpty(principalPolicy))
                principalPolicy = nameof(PrincipalPolicy.UnauthenticatedPrincipal);

            principalPolicyCheckBox.Checked = principalPolicyListBox.Enabled =
                principalPolicy != nameof(PrincipalPolicy.UnauthenticatedPrincipal);
            principalPolicyListBox.SelectedItem = principalPolicy;

            int currentSeed = PackageSettings.GetValueOrDefault(SettingDefinitions.RandomSeed);
            if (currentSeed > 0)
                currentRandomSeedTextBox.Text = currentSeed.ToString();
        }

        public override void ApplySettings()
        {
            // Check if current values in UI elements differ from those in TestCentricProject
            // If values differ, add them to SettingsChanges list, so they can be applied later
            int numAgents = numberOfAgentsCheckBox.Checked
                ? (int)numberOfAgentsUpDown.Value : 0;
            if (numAgents != PackageSettings.GetValueOrDefault(SettingDefinitions.MaxAgents))
                TopLevelPackageSettingChanges.Add(SettingDefinitions.MaxAgents.WithValue(numAgents));

            string principalPolicy = principalPolicyCheckBox.Checked
                ? (string)principalPolicyListBox.SelectedItem
                : nameof(PrincipalPolicy.UnauthenticatedPrincipal);
            if (principalPolicy != PackageSettings.GetValueOrDefault(SettingDefinitions.PrincipalPolicy))
                SubPackageSettingChanges.Add(SettingDefinitions.PrincipalPolicy.WithValue(principalPolicy));

            if (randomSeedCheckBox.Checked)
            {
                if (int.TryParse(randomSeedTextBox.Text, out int seed))
                    SubPackageSettingChanges.Add(SettingDefinitions.RandomSeed.WithValue(seed));
            }
        }

        private void numberOfAgentsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            numberOfAgentsUpDown.Enabled = numberOfAgentsCheckBox.Checked;
        }

        private void principalPolicyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            principalPolicyListBox.Enabled = principalPolicyCheckBox.Checked;
            if (!principalPolicyCheckBox.Checked)
                principalPolicyListBox.SelectedItem = nameof(PrincipalPolicy.UnauthenticatedPrincipal);
        }

        private void randomSeedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            randomSeedTextBox.Enabled = randomSeedCheckBox.Checked;
        }

        private void randomSeedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void randomSeedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (randomSeedCheckBox.Checked && !int.TryParse(randomSeedTextBox.Text, out _))
            {
                MessageDisplay.Error("Random Seed must be a valid integer.");
                e.Cancel = true;
            }
        }
    }
}

