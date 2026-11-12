// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TestCentric.Gui.Views
{
    public partial class TestPackageSubView : TestPropertiesView.SubView
    {
        public TestPackageSubView()
        {
            InitializeComponent();
        }

        public override int FullHeight => packageSettings.Top + HeightNeededForControl(packageSettings) + 8;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PackageSettings
        {
            get { return packageSettings.Text; }
            set { InvokeIfRequired(() => { packageSettings.Text = value; }); }
        }
    }
}
