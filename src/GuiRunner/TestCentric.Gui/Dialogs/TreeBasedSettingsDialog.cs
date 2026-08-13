// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

namespace TestCentric.Gui.Dialogs
{
    using System.Windows.Forms;
    using Model;
    using Presenters;

    public partial class TreeBasedSettingsDialog : SettingsDialogBase
    {
        private SettingsPage? current;

        public static void Display(TestCentricPresenter presenter, ITestModel model, params SettingsPage[] pages)
        {
            using (TreeBasedSettingsDialog dialog = new TreeBasedSettingsDialog(presenter, model))
            {
                dialog.Font = model.Settings.Gui.Font;
                dialog.SettingsPages.AddRange(pages);
                dialog.ShowDialog();
            }
        }

        public TreeBasedSettingsDialog(TestCentricPresenter presenter, ITestModel model) : base(presenter, model)
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }

       private void TreeBasedSettingsDialog_Load(object sender, System.EventArgs e)
        {
            foreach (SettingsPage page in SettingsPages)
                AddBranchToTree(treeView1.Nodes, page.Key);

            if (treeView1.VisibleCount >= treeView1.GetNodeCount(true))
                treeView1.ExpandAll();

            SelectInitialPage();

            treeView1.Select();

            // Hide tree view if there is only one single page displayed
            if (treeView1.GetNodeCount(true) == 1)
            {
                HideTreeView();
            }
        }

        private void HideTreeView()
        {
            treeView1.Visible = false;

            // Move remaining controls to left and resize dialog
            panel1.Left = treeView1.Left;
            groupBox1.Left = treeView1.Left;
            Size = new System.Drawing.Size(Size.Width - treeView1.Width, Size.Height);
        }

        private void SelectInitialPage()
        {
            string initialPage = Settings.Gui.InitialSettingsPage;

            if (initialPage != null)
                SelectPage(initialPage);
            else
            if (treeView1.Nodes.Count > 0)
                SelectFirstPage(treeView1.Nodes);
        }

        private void SelectPage(string initialPage)
        {
            TreeNode? node = FindNode(treeView1.Nodes, initialPage);
            if (node != null)
                treeView1.SelectedNode = node;
            else
                SelectFirstPage(treeView1.Nodes);
        }

        private TreeNode? FindNode(TreeNodeCollection nodes, string key)
        {
            int dot = key.IndexOf('.');
            string? tail = null;

            if (dot >= 0)
            {
                tail = key.Substring(dot + 1);
                key = key.Substring(0, dot);
            }

            foreach (TreeNode node in nodes)
                if (node.Text == key)
                    return tail == null
                        ? node
                        : FindNode(node.Nodes, tail);

            return null;
        }

        private void SelectFirstPage(TreeNodeCollection nodes)
        {
            if (nodes[0].Nodes.Count == 0)
                treeView1.SelectedNode = nodes[0];
            else
            {
                nodes[0].Expand();
                SelectFirstPage(nodes[0].Nodes);
            }
        }

        private void AddBranchToTree(TreeNodeCollection nodes, string key)
        {
            int dot = key.IndexOf('.');
            if (dot < 0)
            {
                nodes.Add(new TreeNode(key, 2, 2));
                return;
            }

            string name = key.Substring(0, dot);
            key = key.Substring(dot + 1);

            TreeNode node = FindOrAddNode(nodes, name);

            if (key != null)
                AddBranchToTree(node.Nodes, key);
        }

        private TreeNode FindOrAddNode(TreeNodeCollection nodes, string name)
        {
            foreach (TreeNode node in nodes)
                if (node.Text == name)
                    return node;

            TreeNode newNode = new TreeNode(name, 0, 0);
            nodes.Add(newNode);
            return newNode;
        }

        private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            string key = e.Node.FullPath;
            SettingsPage page = SettingsPages[key];
            Settings.Gui.InitialSettingsPage = key;

            if (page != null && page != current)
            {
                panel1.Controls.Clear();
                panel1.Controls.Add(page);
                page.Dock = DockStyle.Fill;
                current = page;
                return;
            }
        }

        private void treeView1_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
        }

        private void treeView1_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
        }
    }
}

