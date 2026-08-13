// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Windows.Forms;

namespace TestCentric.Gui.SettingsPages
{
    public partial class AssemblyReloadSettingsPage : SettingsPage
    {

        public AssemblyReloadSettingsPage(string key) : base(key)
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            rerunOnChangeCheckBox.Checked = Settings.Gui.RerunOnChange;
        }

        public override void ApplySettings()
        {
            Settings.Gui.RerunOnChange = rerunOnChangeCheckBox.Checked;
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            System.Diagnostics.Process.Start("http://nunit.com/?p=optionsDialog&r=2.4.5");
        }
    }
}

