// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TestCentric.Gui.Dialogs
{
    using System;
    using System.Reflection;
    using Model;
    using Model.Settings;
    using NUnit.Engine;
    using Presenters;

    /// <summary>
    /// Summary description for OptionsDialogBase.
    /// </summary>
    public partial class SettingsDialogBase : TestCentricFormBase
    {
        private SettingsPageCollection pageList;

        #region Construction and Disposal

        // NOTE: We have to keep a default constructor for design mode
        public SettingsDialogBase(TestCentricPresenter presenter, ITestModel model) : this()
        {
            Presenter = presenter;
            Model = model;
            Settings = model.Settings;
            SubPackageSettingChanges = new List<PackageSetting>();
            TopLevelPackageSettingChanges = new List<PackageSetting>();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            SubPackageSettingChanges.Clear();
            TopLevelPackageSettingChanges.Clear();
        }

#nullable disable
        public SettingsDialogBase() : base("TestCentric Settings")
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            pageList = new SettingsPageCollection();
        }
#nullable restore
        #endregion

        #region Properties

        public TestCentricPresenter Presenter { get; }

        public ITestModel Model { get; }

        public IUserSettings Settings { get; }

        public IList<PackageSetting> SubPackageSettingChanges { get; }

        public IList<PackageSetting> TopLevelPackageSettingChanges { get; }

        public SettingsPageCollection SettingsPages
        {
            get { return pageList; }
        }

        #endregion

        #region Public Methods
        public void ApplySettings()
        {
            foreach (SettingsPage page in pageList)
                if (page.SettingsLoaded)
                    page.ApplySettings();

            foreach(PackageSetting setting in SubPackageSettingChanges)
                Model.TestCentricProject?.ApplySetting(setting);

            foreach (PackageSetting setting in TopLevelPackageSettingChanges)
                Model.TestCentricProject?.SetTopLevelSetting(setting);
        }
        #endregion

        #region Event Handlers

        private void okButton_Click(object sender, System.EventArgs e)
        {
            ApplySettings();
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion

        #region Nested SettingsPageCollection Class
        public class SettingsPageCollection : CollectionBase
        {
            public void Add(SettingsPage page)
            {
                this.InnerList.Add(page);
            }

            public void AddRange(params SettingsPage[] pages)
            {
                this.InnerList.AddRange(pages);
            }

            public SettingsPage this[int index]
            {
                get { return (SettingsPage)InnerList[index]; }
            }

            public SettingsPage this[string key]
            {
                get
                {
                    foreach (SettingsPage page in InnerList)
                        if (page.Key == key)
                            return page;

                    throw new InvalidOperationException($"SettingsPage with key '{key}' not found.");
                }
            }
        }
        #endregion
    }
}
