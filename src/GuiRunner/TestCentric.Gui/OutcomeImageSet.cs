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

        private const string NOT_LATEST_SUFFIX = "_NotLatestRun";

        private string _imageSetDir;
        private string _commonImageDir;

        public OutcomeImageSet(string imageSetDir)
        {
            Guard.ArgumentValid(IsValidImageSetDirectory(imageSetDir), $"Directory {imageSetDir} does not contain an image set.", nameof(imageSetDir));

            _imageSetDir = imageSetDir;
            _commonImageDir = Path.GetDirectoryName(imageSetDir);
            
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

            string fileName = imgName + ".png";

            // If an image is included in the image set itself, use it.
            if (TryToLoadImage(_imageSetDir, imgName))
                return _images[imgName];

            // Some images may be common to all image sets and found in the common directory
            if (TryToLoadImage(_commonImageDir, imgName))
                return _images[imgName];

            // Images for prior runs are a special case if not found
            if (imgName.EndsWith(NOT_LATEST_SUFFIX))
            {
                string imgBaseName = imgName.Substring(0, imgName.Length - NOT_LATEST_SUFFIX.Length);

                // TODO: Try generating from the base image here.

                // Use the base image itself as a last resort. Base images are
                // always found in the image set directory.
                if (TryToLoadImage(_imageSetDir, imgName, imgBaseName + ".png"))
                    return _images[imgName];
            }

            throw new System.Exception($"Could not locate image '{imgName}'");

            bool TryToLoadImage(string directory, string imgName, string fileName = null)
            {
                if (fileName == null)
                    fileName = imgName + ".png";

                string filePath = Path.Combine(directory, fileName);

                if (File.Exists(filePath))
                {
                    _images[imgName] = Image.FromFile(filePath);
                    return true;
                }

                return false;
            }
        }
    }
}
