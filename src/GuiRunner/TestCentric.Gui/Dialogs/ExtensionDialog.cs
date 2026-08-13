// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NUnit.Engine;
using NUnit.Extensibility;

namespace TestCentric.Gui.Dialogs
{
    /// <summary>
    /// Summary description for ExtensionDialog.
    /// </summary>
    public partial class ExtensionDialog : System.Windows.Forms.Form
    {
        private IList<IExtensionPoint> _extensionPoints;
        private IList<IExtensionNode> _extensionPointExtensions;
        private IList<IExtensionNode> _allExtensions;

        private IExtensionService _extensionService;

        public ExtensionDialog(IExtensionService extensionService)
        {
            _extensionService = extensionService;

            _extensionPoints = [];
            _extensionPointExtensions = [];
            _allExtensions = [];

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        private void ExtensionDialog_Load(object sender, System.EventArgs e)
        {
            if (!DesignMode)
            {
                Tab1_InitialDisplay();
                Tab2_InitialDisplay();
            }
        }

        private void ExtensionDialog_ResizeEnd(object sender, EventArgs e) => AdjustListViewLayout();

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void Tab1_InitialDisplay()
        {
            _allExtensions = new List<IExtensionNode>(_extensionService.Extensions);
            foreach (var extension in _allExtensions)
            {
                ListViewItem item = new ListViewItem(
                    new string[] {
                            extension.TypeName,
                            extension.Status.ToString(),
                            extension.Enabled ? "Yes" : "No" });

                extensionListView1.Items.Add(item);
            }

            if (extensionListView1.Items.Count > 0)
                extensionListView1.Items[0].Selected = true;
        }

        private void Tab1_SelectedExtensionChanged(object sender, EventArgs e)
        {
            if (!DesignMode && extensionListView1.SelectedIndices.Count > 0)
            {
                int index = extensionListView1.SelectedIndices[0];
                var extension = _allExtensions[index];

                extensionDescription1.Text = extension.Description ?? "==None Provided==";

                extensionProperties1.Clear();
                foreach (string prop in extension.PropertyNames)
                {
                    var sb = new StringBuilder($"{prop} :");
                    foreach (string val in extension.GetValues(prop))
                        sb.Append(" " + val);

                    extensionProperties1.AppendText(sb.ToString() + Environment.NewLine);
                }
                extensionProperties1.Select(0, 0);
                extensionProperties1.ScrollToCaret();
                assemblyPath1.Text = extension.AssemblyPath;
                assemblyVersion1.Text = extension.AssemblyVersion.ToString();

                extensionPointPath.Text = extension.Path ?? "--No Path Found==";

                //AdjustListViewLayout();
            }
        }

        private void Tab2_InitialDisplay()
        {
            _extensionPoints = new List<IExtensionPoint>(_extensionService.ExtensionPoints);

            foreach (var ep in _extensionPoints)
                extensionPointsListBox.Items.Add(ep.Path);

            if (extensionPointsListBox.Items.Count > 0)
                extensionPointsListBox.SelectedIndex = 0;
        }

        private void Tab2_SelectedExtensionPointChanged(object sender, EventArgs e)
        {
            var index = extensionPointsListBox.SelectedIndex;
            if (index >= 0)
            {
                var ep = _extensionPoints[index];
                _extensionPointExtensions = new List<IExtensionNode>(ep.Extensions);
                extensionPointDescriptionTextBox.Text = ep.Description ?? "==None Provided==";

                extensionListView2.Items.Clear();
                foreach (var extension in ep.Extensions)
                {
                    ListViewItem item = new ListViewItem(
                        new string[] {
                            extension.TypeName,
                            extension.Status.ToString(),
                            extension.Enabled ? "Yes" : "No" });

                    //if (extension.Status == ExtensionStatus.Error)
                    //    item.ToolTipText = BuildExceptionMessage(extension.Exception);

                    extensionListView2.Items.Add(item);
                }

                if (extensionListView2.Items.Count > 0)
                {
                    extensionListView2.Items[0].Selected = true;
                }
                else
                {
                    extensionDescription2.Text = "";
                    extensionProperties2.Text = "";
                    assemblyPath2.Text = "";
                    assemblyVersion2.Text = "";
                    AdjustListViewLayout();
                }
            }
        }

        private void Tab2_SelectedExtensionChanged(object sender, System.EventArgs e)
        {
            if (!DesignMode && extensionListView2.SelectedIndices.Count > 0)
            {
                int index = extensionListView2.SelectedIndices[0];
                var extension = _extensionPointExtensions[index];

                extensionDescription2.Text = extension.Description ?? "==None Provided==";

                extensionProperties2.Clear();
                foreach (string prop in extension.PropertyNames)
                {
                    var sb = new StringBuilder($"{prop} :");
                    foreach (string val in extension.GetValues(prop))
                        sb.Append(" " + val);

                    extensionProperties2.AppendText(sb.ToString() + Environment.NewLine);
                }

                assemblyPath2.Text = extension.AssemblyPath;
                assemblyVersion2.Text = extension.AssemblyVersion.ToString();

                //AdjustListViewLayout();
            }
        }

        private static string BuildExceptionMessage(Exception exception)
        {
            var sb = new StringBuilder($"{exception.GetType().Name}: {exception.Message}");

            var inner = exception.InnerException;
            while (inner != null)
            {
                sb.AppendLine($"--> {inner.Message}");
                inner = inner.InnerException;
            }

            return sb.ToString();
        }

        private void AdjustListViewLayout()
        {
            int width = extensionListView2.ClientSize.Width;
            for (int i = 1; i < extensionListView2.Columns.Count; i++)
                width -= extensionListView2.Columns[i].Width;
            extensionListView2.Columns[0].Width = width;

            extensionListView2.Refresh();
        }
    }
}
