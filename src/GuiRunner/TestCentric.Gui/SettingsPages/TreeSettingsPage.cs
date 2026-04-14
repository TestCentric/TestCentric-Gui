// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TestCentric.Gui.Presenters;

namespace TestCentric.Gui.SettingsPages
{
    public class TreeSettingsPage : SettingsPage
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox showCheckBoxesCheckBox;
        private System.Windows.Forms.ComboBox displayFormatComboBox;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private Label label6;
        private PictureBox successImage;
        private PictureBox failureImage;
        private PictureBox warningImage;
        private PictureBox ignoredImage;
        private PictureBox inconclusiveImage;
        private PictureBox skippedImage;
        private System.ComponentModel.IContainer components = null;
        private ListBox imageSetListBox;
        private static readonly string[] imageExtensions = { ".png", ".jpg" };
        private Label label5;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private GroupBox groupBox2;
        private ImageSetManager _imageSetManager;

        public TreeSettingsPage(string key, ImageSetManager imageSetManager) : base(key)
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            _imageSetManager = imageSetManager;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeSettingsPage));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.showCheckBoxesCheckBox = new System.Windows.Forms.CheckBox();
            this.displayFormatComboBox = new System.Windows.Forms.ComboBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.label6 = new System.Windows.Forms.Label();
            this.successImage = new System.Windows.Forms.PictureBox();
            this.failureImage = new System.Windows.Forms.PictureBox();
            this.warningImage = new System.Windows.Forms.PictureBox();
            this.ignoredImage = new System.Windows.Forms.PictureBox();
            this.inconclusiveImage = new System.Windows.Forms.PictureBox();
            this.skippedImage = new System.Windows.Forms.PictureBox();
            this.imageSetListBox = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.successImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.failureImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ignoredImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inconclusiveImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.skippedImage)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(144, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 8);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Tree Images";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Default settings for new projects:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 259);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Default tree display format:";
            // 
            // showCheckBoxesCheckBox
            // 
            this.showCheckBoxesCheckBox.AutoSize = true;
            this.helpProvider1.SetHelpString(this.showCheckBoxesCheckBox, "Selects the default display of the checkbox when a new project is created.");
            this.showCheckBoxesCheckBox.Location = new System.Drawing.Point(46, 231);
            this.showCheckBoxesCheckBox.Name = "showCheckBoxesCheckBox";
            this.helpProvider1.SetShowHelp(this.showCheckBoxesCheckBox, true);
            this.showCheckBoxesCheckBox.Size = new System.Drawing.Size(274, 17);
            this.showCheckBoxesCheckBox.TabIndex = 36;
            this.showCheckBoxesCheckBox.Text = "Default display of a checkbox next to each tree item.";
            // 
            // displayFormatComboBox
            // 
            this.displayFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.helpProvider1.SetHelpString(this.displayFormatComboBox, "Selects the default tree display format when a new project is created");
            this.displayFormatComboBox.ItemHeight = 13;
            this.displayFormatComboBox.Items.AddRange(new object[] {
            "NUnit Tree",
            "Test List"});
            this.displayFormatComboBox.Location = new System.Drawing.Point(284, 256);
            this.displayFormatComboBox.Name = "displayFormatComboBox";
            this.helpProvider1.SetShowHelp(this.displayFormatComboBox, true);
            this.displayFormatComboBox.Size = new System.Drawing.Size(168, 21);
            this.displayFormatComboBox.TabIndex = 33;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Window;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(46, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 142);
            this.label6.TabIndex = 47;
            // 
            // successImage
            // 
            this.successImage.Image = ((System.Drawing.Image)(resources.GetObject("successImage.Image")));
            this.successImage.Location = new System.Drawing.Point(59, 36);
            this.successImage.Name = "successImage";
            this.successImage.Size = new System.Drawing.Size(16, 16);
            this.successImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.successImage.TabIndex = 48;
            this.successImage.TabStop = false;
            // 
            // failureImage
            // 
            this.failureImage.Image = ((System.Drawing.Image)(resources.GetObject("failureImage.Image")));
            this.failureImage.Location = new System.Drawing.Point(59, 58);
            this.failureImage.Name = "failureImage";
            this.failureImage.Size = new System.Drawing.Size(16, 16);
            this.failureImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.failureImage.TabIndex = 49;
            this.failureImage.TabStop = false;
            // 
            // warningImage
            // 
            this.warningImage.Image = ((System.Drawing.Image)(resources.GetObject("warningImage.Image")));
            this.warningImage.Location = new System.Drawing.Point(59, 80);
            this.warningImage.Name = "warningImage";
            this.warningImage.Size = new System.Drawing.Size(16, 16);
            this.warningImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.warningImage.TabIndex = 50;
            this.warningImage.TabStop = false;
            // 
            // ignoredImage
            // 
            this.ignoredImage.Image = ((System.Drawing.Image)(resources.GetObject("ignoredImage.Image")));
            this.ignoredImage.Location = new System.Drawing.Point(59, 102);
            this.ignoredImage.Name = "ignoredImage";
            this.ignoredImage.Size = new System.Drawing.Size(16, 16);
            this.ignoredImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ignoredImage.TabIndex = 50;
            this.ignoredImage.TabStop = false;
            // 
            // inconclusiveImage
            // 
            this.inconclusiveImage.Enabled = false;
            this.inconclusiveImage.Image = ((System.Drawing.Image)(resources.GetObject("inconclusiveImage.Image")));
            this.inconclusiveImage.Location = new System.Drawing.Point(59, 124);
            this.inconclusiveImage.Name = "inconclusiveImage";
            this.inconclusiveImage.Size = new System.Drawing.Size(16, 16);
            this.inconclusiveImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.inconclusiveImage.TabIndex = 51;
            this.inconclusiveImage.TabStop = false;
            // 
            // skippedImage
            // 
            this.skippedImage.Image = ((System.Drawing.Image)(resources.GetObject("skippedImage.Image")));
            this.skippedImage.Location = new System.Drawing.Point(59, 146);
            this.skippedImage.Name = "skippedImage";
            this.skippedImage.Size = new System.Drawing.Size(16, 16);
            this.skippedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.skippedImage.TabIndex = 52;
            this.skippedImage.TabStop = false;
            // 
            // imageSetListBox
            // 
            this.imageSetListBox.FormattingEnabled = true;
            this.imageSetListBox.Location = new System.Drawing.Point(235, 44);
            this.imageSetListBox.Name = "imageSetListBox";
            this.imageSetListBox.Size = new System.Drawing.Size(168, 121);
            this.imageSetListBox.TabIndex = 54;
            this.imageSetListBox.SelectedIndexChanged += new System.EventHandler(this.imageSetListBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(81, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 55;
            this.label5.Text = "Passed";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(81, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 56;
            this.label7.Text = "Failed";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(81, 81);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 57;
            this.label8.Text = "Warning";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(81, 102);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 58;
            this.label9.Text = "Ignored";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(81, 127);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 59;
            this.label10.Text = "Inconclusive";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(81, 146);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 60;
            this.label11.Text = "Not Run";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(232, 25);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(88, 13);
            this.label12.TabIndex = 61;
            this.label12.Text = "Select Image Set";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(188, 206);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(259, 8);
            this.groupBox2.TabIndex = 62;
            this.groupBox2.TabStop = false;
            // 
            // TreeSettingsPage
            // 
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.imageSetListBox);
            this.Controls.Add(this.skippedImage);
            this.Controls.Add(this.inconclusiveImage);
            this.Controls.Add(this.ignoredImage);
            this.Controls.Add(this.warningImage);
            this.Controls.Add(this.failureImage);
            this.Controls.Add(this.successImage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.showCheckBoxesCheckBox);
            this.Controls.Add(this.displayFormatComboBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Name = "TreeSettingsPage";
            ((System.ComponentModel.ISupportInitialize)(this.successImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.failureImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ignoredImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inconclusiveImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.skippedImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

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

