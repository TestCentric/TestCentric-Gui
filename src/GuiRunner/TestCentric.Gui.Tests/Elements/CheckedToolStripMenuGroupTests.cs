// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using NUnit.Framework;

namespace TestCentric.Gui.Elements
{
    [TestFixture]
    internal class CheckedToolStripMenuGroupTests
    {
        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenMenuItemsAreNull()
        {
            System.Windows.Forms.ToolStripMenuItem[] menuItems = null!;
            Assert.Throws<ArgumentNullException>(() =>  new CheckedToolStripMenuGroup("Menu", menuItems));
        }

        [Test]
        public void Constructor_ThrowsArgumentException_WhenMenuItemsAreEmpty()
        {
            System.Windows.Forms.ToolStripMenuItem[] menuItems = [];
            Assert.Throws<ArgumentException>(() => new CheckedToolStripMenuGroup("Menu", menuItems));
        }
    }
}
