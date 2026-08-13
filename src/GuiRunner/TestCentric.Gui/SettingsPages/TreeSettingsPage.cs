// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using TestCentric.Gui.Presenters;

namespace TestCentric.Gui.SettingsPages
{
    public partial class TreeSettingsPage : SettingsPage
    {
        private ImageSetManager _imageSetManager;

        public TreeSettingsPage(string key, ImageSetManager imageSetManager) : base(key)
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            _imageSetManager = imageSetManager;
        }

        public override void LoadSettings()
        {
            showCheckBoxesCheckBox.Checked = Settings.Gui.TestTree.ShowCheckBoxes;

            int selectedDisplayFormatIndex = Settings.Gui.TestTree.DisplayFormat == "TEST_LIST" ? 1 : 0;
            displayFormatComboBox.SelectedIndex = selectedDisplayFormatIndex;

            foreach (string imageSetName in _imageSetManager.ImageSets.Keys)
                imageSetListBox.Items.Add(imageSetName);

            imageSetListBox.SelectedItem = _imageSetManager.CurrentImageSet.Name;
        }

        public override void ApplySettings()
        {
            Settings.Gui.TestTree.ShowCheckBoxes = showCheckBoxesCheckBox.Checked;
            string displayFormat = displayFormatComboBox.SelectedIndex == 0 ? "NUNIT_TREE" : "TEST_LIST";
            Settings.Gui.TestTree.DisplayFormat = displayFormat;

            if (imageSetListBox.SelectedIndex >= 0)
                Settings.Gui.TestTree.AlternateImageSet = (string)imageSetListBox.SelectedItem;
        }

        private void imageSetListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string imageSetName = imageSetListBox.SelectedItem as string;
            OutcomeImageSet imageSet = _imageSetManager.LoadImageSet(imageSetName);

            successImage.Image = imageSet.LoadImage("Success");
            failureImage.Image = imageSet.LoadImage("Failure");
            ignoredImage.Image = imageSet.LoadImage("Ignored");
            inconclusiveImage.Image = imageSet.LoadImage("Inconclusive");
            skippedImage.Image = imageSet.LoadImage("Skipped");
            warningImage.Image = imageSet.LoadImage("Warning");
        }
    }
}

