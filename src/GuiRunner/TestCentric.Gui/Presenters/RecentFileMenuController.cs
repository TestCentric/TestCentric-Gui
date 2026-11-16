// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

namespace TestCentric.Gui.Presenters
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using NUnit;
    using TestCentric.Gui.Elements;
    using TestCentric.Gui.Model;

    /// <summary>
    /// This class is responsible for the recent file menu entries. It populates the menu from the recent file list settings,
    /// handles the menu item events and provide a context menu to remove entries from the recent file list.
    ///
    /// The menu can either show project files or non-project files only. 
    /// </summary>
    public class RecentFileMenuController
    {
        private bool _IsKeyEventRegistered;

        public RecentFileMenuController(ITestModel model, IPopup menuItem, bool filterProjectFiles)
        {
            Guard.ArgumentNotNull(model, nameof(model));
            Guard.ArgumentNotNull(menuItem, nameof(menuItem));

            Model = model;
            MenuItems = menuItem.MenuItems;
            FilterProjectFiles = filterProjectFiles;
        }
        
        private ITestModel Model { get; }

        private ToolStripItemCollection MenuItems { get; }

        private bool FilterProjectFiles { get; }

        public void PopulateMenu()
        {
            // Remove all non-existing files from recent file list (includes null entries)
            var entries = Model.Settings.Gui.RecentFiles.Entries;
            for (int index = entries.Count; --index >= 0;)
                if (!File.Exists(entries[index]))
                    entries.RemoveAt(index);

            // Filter the recent file list for project files or non-project files
            var recentFiles = entries.Cast<string>().Where(Filter).ToList();

            // Create menu items
            MenuItems.Clear();
            foreach (string fileName in recentFiles)
            {
                string menuItemText = $"{MenuItems.Count + 1} {fileName}";
                ToolStripMenuItem menuItem = CreateRecentFileMenuItem(menuItemText, fileName);
                MenuItems.Add(menuItem);
            }

            if (MenuItems.Count > 0)
            {
                MenuItems.Add(new ToolStripSeparator());
                var menuItem = new ToolStripMenuItem("Clear recent file list");
                menuItem.ToolTipText = "Remove all entries from the recent file list";
                menuItem.Click += OnClearRecentFileList;
                MenuItems.Add(menuItem);

                // Key events must be registered on ToolStripDropDown element
                if (!_IsKeyEventRegistered)
                {
                    menuItem.Owner.KeyDown += OnKeyDown;
                    _IsKeyEventRegistered = true;
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                // Get selected menu item
                var selectedItem = MenuItems.OfType<ToolStripMenuItem>().FirstOrDefault(item => item.Selected);
                if (selectedItem != null)
                {
                    RemoveRecentFileEntry(selectedItem);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private bool Filter(string fileName)
        {
            return FilterProjectFiles && IsProjectFile(fileName) || 
                   !FilterProjectFiles && !IsProjectFile(fileName);
        }

        private bool IsProjectFile(string fileName)
        {
            return fileName.EndsWith("tcproj", StringComparison.InvariantCultureIgnoreCase);
        }

        private ToolStripMenuItem CreateRecentFileMenuItem(string menuText, string tag)
        {
            var menuItem = new ToolStripMenuItem(menuText);
            menuItem.ToolTipText = $"Open {tag}\r\n[Press Delete to remove]";
            menuItem.Tag = tag;
            menuItem.Click += OnMenuItemClicked;
            return menuItem;
        }

        private void OnMenuItemClicked(object? sender, EventArgs e)
        {
            string? fileName = ((ToolStripMenuItem?)sender)?.Tag as string;
            if (fileName != null)
                Model.OpenExistingFile(fileName);
        }

        private void OnClearRecentFileList(object? sender, EventArgs e)
        {
            var entries = Model.Settings.Gui.RecentFiles.Entries;
            var fileNames = entries.Cast<string>().Where(Filter).ToList();
            foreach (string fileName in fileNames)
                entries.Remove(fileName);
        }

        private void RemoveRecentFileEntry(ToolStripMenuItem toolStripMenuItem)
        {
            // Remove entry from settings
            if (toolStripMenuItem?.Tag is not string fileName)
                return;
            var entries = Model.Settings.Gui.RecentFiles.Entries;
            entries.Remove(fileName);

            // Remove entry from menu
            ToolStripDropDown? dropDown = toolStripMenuItem.Owner as ToolStripDropDown;
            int currentIndex = MenuItems.IndexOf(toolStripMenuItem);
            if (currentIndex != -1)
                MenuItems.Remove(toolStripMenuItem);

            // Close recent file menu if last item was removed
            var recentFileItems = MenuItems.OfType<ToolStripMenuItem>().Where(t => t.Tag != null);
            if (!recentFileItems.Any())
            {
                MenuItems.Clear();
                dropDown?.Close();
                return;
            }

            // Update text of all remaining menu items
            int index = 1;
            foreach (ToolStripMenuItem menuItem in recentFileItems)
                menuItem.Text = $"{index++} {menuItem.Tag}";

            // Select the next item in the menu if possible
            if (currentIndex >= 0 && currentIndex < MenuItems.Count)
                (MenuItems[currentIndex] as ToolStripMenuItem)?.Select();
        }
    }
}
