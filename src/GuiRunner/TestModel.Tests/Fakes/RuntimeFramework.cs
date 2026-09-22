// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Runtime.Versioning;
using TestCentric.Engine;

namespace TestCentric.Gui.Model.Fakes
{
    public class RuntimeFramework : NUnit.Engine.IRuntimeFramework
    {
        public RuntimeFramework(string frameworkName)
        {
            FrameworkName = new FrameworkName(frameworkName);
            DisplayName = frameworkName;
        }

        public FrameworkName FrameworkName { get; }

        public string TFM { get; }

        public string DisplayName { get; set; }
    }
}
