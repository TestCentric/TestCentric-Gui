// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestCentric.Gui.Elements;

namespace TestCentric.Gui.Views
{
    public partial class TestGroupPropertiesSubView : TestPropertiesView.SubView
    {
        public TestGroupPropertiesSubView()
        {
            InitializeComponent();
        }

        public override int FullHeight => testCaseCount.Top + HeightNeededForControl(testCaseCount) + 8;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FullName
        {
            get { return fullName.Text; }
            set { this.InvokeIfRequired(() => { fullName.Text = value; }); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TestCount
        {
            get { return testCaseCount.Text; }
            set { this.InvokeIfRequired(() => { testCaseCount.Text = value; }); }
        }
    }
}
