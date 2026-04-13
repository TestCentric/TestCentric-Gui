// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NUnit;

namespace TestCentric.Gui
{
    public class OutcomeImageSet
    {
        // A valid ImageSet directory must contain at least the following files. It may
        // contain additional image .png files known to and used by the ImageSetManager.
        private static readonly string[] REQUIRED_FILES = new[] {
            "Inconclusive.png", "Success.png", "Failure.png", "Ignored.png", "Skipped.png", "Warning.png" };

        private string _imageSetDir;

        public OutcomeImageSet(string imageSetDir)
        {
            Guard.ArgumentValid(IsValidImageSetDirectory(imageSetDir), $"Directory {imageSetDir} does not contain an image set.", nameof(imageSetDir));

            _imageSetDir = imageSetDir;
            
            Name = Path.GetFileName(imageSetDir);
        }

        public string Name { get; }

        public static bool IsValidImageSetDirectory(string dir)
        {
            foreach (string file in REQUIRED_FILES)
                if (!File.Exists(Path.Combine(dir, file)))
                    return false;

            return true;
        }

        // Counter used for testing
        public int LoadCount { get; private set; } = 0;

        private Dictionary<string, Image> _images = new Dictionary<string, Image>();

        /// <summary>
        /// Load a specific icon image for use, using a cache if already loaded.
        /// </summary>
        /// <param name="imgName">The name associated with this icon image.</param>
        /// <returns>An Image for the specified icon.</returns>
        public Image LoadImage(string imgName)
        {
            if (_images.ContainsKey(imgName))
                return _images[imgName];

            LoadCount++;
            
            return _images[imgName] = Image.FromFile(Path.Combine(_imageSetDir, imgName + ".png"));
        }
    }
}
