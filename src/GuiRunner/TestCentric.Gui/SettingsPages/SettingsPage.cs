// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TestCentric.Gui
{
    using Dialogs;
    using Model;
    using Model.Settings;
    using NUnit.Engine;

    /// <summary>
    /// SettingsPage is the base class for all pages used
    /// in a tabbed or tree-structured SettingsDialog.
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private string _key;
        private string _title;

        private MessageDisplay _messageDisplay;

#nullable disable
        // Constructor used by the Windows.Forms Designer
        public SettingsPage()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }
#nullable restore

        // Constructor we use in creating page for a Tabbed
        // or TreeBased dialog.
        public SettingsPage(string key) : this()
        {
            _key = key;
            _title = key;
            int dot = key.LastIndexOf('.');
            if (dot >= 0) _title = key.Substring(dot + 1);
            _messageDisplay = new MessageDisplay("TestCentric Settings", Font);
        }

        #region Properties

        public string Key
        {
            get { return _key; }
        }

        public string Title
        {
            get { return _title; }
        }

        public bool SettingsLoaded
        {
            get { return Settings != null; }
        }

        public IMessageDisplay MessageDisplay
        {
            get { return _messageDisplay; }
        }

        protected ITestModel Model { get; private set; }
        protected IUserSettings Settings { get; private set; }

        protected IList<PackageSetting> SubPackageSettingChanges { get; private set; }
        protected IList<PackageSetting> TopLevelPackageSettingChanges { get; private set; }

        #endregion

        #region Public Methods
        public virtual void LoadSettings()
        {
        }

        public virtual void ApplySettings()
        {
        }
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                var dlg = ParentForm as SettingsDialogBase;

                if (dlg == null)
                    throw new InvalidOperationException("SettingsPage is only designed to be used in a Settings Dialog");
                if (dlg.Settings == null)
                    throw new InvalidOperationException("The Settings Dialog was not properly initialized");

                Model = dlg.Model;
                Settings = dlg.Settings;
                SubPackageSettingChanges = dlg.SubPackageSettingChanges;
                TopLevelPackageSettingChanges = dlg.TopLevelPackageSettingChanges;

                LoadSettings();
            }
        }
    }
}
