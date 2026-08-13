// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace TestCentric.Gui.Dialogs
{
    public partial class AboutBox : Form
    {


        public AboutBox()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string versionText = $"Version {executingAssembly.GetName().Version}";

            object[] objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            if (objectAttrs.Length > 0)
                versionText = $"Version {((AssemblyInformationalVersionAttribute)objectAttrs[0]).InformationalVersion}";
            // Truncate the informational version to get the version with optional pre-release label.
            int plus = versionText.IndexOf('+');
            if (plus > 0)
                versionText = versionText.Substring(0, plus);

            objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (objectAttrs.Length > 0)
            {
                string configText = ((AssemblyConfigurationAttribute)objectAttrs[0]).Configuration;
                if (configText != "")
                    versionText += string.Format(" ({0})", configText);
            }

            string copyrightText = "Copyright (C) 2018-2021 Charlie Poole and TestCentric contributors";
            objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (objectAttrs.Length > 0)
                copyrightText = ((AssemblyCopyrightAttribute)objectAttrs[0]).Copyright;

            versionLabel.Text = versionText;
            copyright.Text = copyrightText;
        }

        private void OkButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://test-centric.org");
            linkLabel1.LinkVisited = true;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://test-centric.org/testcentric-gui/license");
            linkLabel2.LinkVisited = true;
        }
    }
}
