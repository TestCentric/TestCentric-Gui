// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using NUnit.Framework;
using System.Windows.Forms;

namespace TestCentric.Gui.Elements
{
    [TestFixture]
    internal class KeyCommandTests
    {
        private TestControl _control = null!;
        private bool _keyUpRaised;
        private bool _keyDownRaised;

        [SetUp]
        public void SetUp()
        {
            _control = new TestControl();
            KeyCommand keyCommand = new KeyCommand(_control, new[] { Keys.Enter }, new[] { Keys.Space });
            keyCommand.KeyUp += () => _keyUpRaised = true;
            keyCommand.KeyDown += () => _keyDownRaised = true;

            _keyUpRaised = false;
            _keyDownRaised = false;
        }

        [Test]
        public void KeyDown_ConfiguredKey_RaisesKeyDownEvent()
        {
            // Act
            _control.RaiseKeyDown(Keys.Space);

            // Assert
            Assert.That(_keyDownRaised, Is.True);
        }

        [TestCase(Keys.Enter)]
        [TestCase(Keys.Escape)]
        public void KeyDown_UnconfiguredKey_DoesNotRaiseKeyDownEvent(Keys key)
        {
            // Act
            _control.RaiseKeyDown(key);

            // Assert
            Assert.That(_keyDownRaised, Is.False);
        }

        [Test]
        public void KeyUp_ConfiguredKey_RaisesKeyUpEvent()
        {
            // Act
            _control.RaiseKeyUp(Keys.Enter);

            // Assert
            Assert.That(_keyUpRaised, Is.True);
        }

        [TestCase(Keys.Escape)]
        [TestCase(Keys.Space)]
        public void KeyUp_UnconfiguredKey_DoesNotRaiseKeyUpEvent(Keys key)
        {
            // Act
            _control.RaiseKeyUp(key);

            // Assert
            Assert.That(_keyUpRaised, Is.False);
        }

        [Test]
        public void NullKeyConfigurations_DoNotRaiseEvents()
        {
            // Arrange
            _control = new TestControl();
            KeyCommand keyCommand = new KeyCommand(_control, null, null);
            keyCommand.KeyUp += () => _keyUpRaised = true;
            keyCommand.KeyDown += () => _keyDownRaised = true;

            // Act
            _control.RaiseKeyUp(Keys.Enter);
            _control.RaiseKeyDown(Keys.Enter);

            // Assert
            Assert.That(_keyUpRaised, Is.False);
            Assert.That(_keyDownRaised, Is.False);
        }

        private sealed class TestControl : Control
        {
            public void RaiseKeyDown(Keys key)
            {
                OnKeyDown(new KeyEventArgs(key));
            }

            public void RaiseKeyUp(Keys key)
            {
                OnKeyUp(new KeyEventArgs(key));
            }
        }
    }
}
