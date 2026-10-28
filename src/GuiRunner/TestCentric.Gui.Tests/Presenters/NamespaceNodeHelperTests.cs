// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Linq;
using NUnit.Framework;
using TestCentric.Gui.Model;

namespace TestCentric.Gui.Presenters
{
    [TestFixture]
    internal class NamespaceNodeHelperTests
    {
        [Test]
        public void IsFolded_ShouldReturnFalse_WhenNodeHasNoParent()
        {
            var testNode = new TestNode("<test-run id='0' />");

            bool result = NamespaceNodeHelper.IsFolded(testNode);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsFolded_ShouldReturnFalse_WhenNodeIsNotNamespace()
        {
            string xml =
                "<test-run id='0'><test-suite type='Assembly' id='1' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='2' name='Library'>" +
                        "<test-case id='3' name='Test' />" +
                    "</test-suite>" +
                "</test-suite></test-run>";

            var testNode = new TestNode(xml);

            bool result = NamespaceNodeHelper.IsFolded(GetTestNode(testNode, "3")!);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsFolded_ShouldReturnFalse_WhenParentHasMoreThanOneChild()
        {
            string xml =
                "<test-run id='0'><test-suite type='Assembly' id='1' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='2' name='Library'>" +
                        "<test-suite type='TestSuite' id='3' name='One' />" +
                        "<test-suite type='TestSuite' id='4' name='Two' />" +
                    "</test-suite>" +
                "</test-suite></test-run>";

            var testNode = new TestNode(xml);

            bool result = NamespaceNodeHelper.IsFolded(GetTestNode(testNode, "3")!);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsFolded_ShouldReturnFalse_WhenParentIsNotNamespace()
        {
            string xml =
                "<test-run id='0'><test-suite type='Assembly' id='1' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='2' name='Library'>" +
                        "<test-suite type='TestSuite' id='3' name='One'>" +
                            "<test-suite type='TestSuite' id='4' name='Two' />" +
                        "</test-suite>" +
                    "</test-suite>" +
                "</test-suite></test-run>";

            var testNode = new TestNode(xml);

            bool result = NamespaceNodeHelper.IsFolded(GetTestNode(testNode, "2")!);

            Assert.That(result, Is.False);
        }

        [TestCase("3")]
        [TestCase("4")]
        public void IsFolded_ShouldReturnTrue_WhenParentHasSingleChild(string testId)
        {
            // Arrange
            string xml =
                "<test-run id='0'><test-suite type='Assembly' id='1' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='2' name='Library'>" +
                        "<test-suite type='TestSuite' id='3' name='One'>" +
                            "<test-suite type='TestSuite' id='4' name='Two' />" +
                        "</test-suite>" +
                    "</test-suite>" +
                "</test-suite></test-run>";

            var testNode = new TestNode(xml);

            TestNode? t = GetTestNode(testNode, testId);
            // Act
            bool result = NamespaceNodeHelper.IsFolded(t!);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetTreeNodeName_ShouldCombineFoldedNamespaceNames()
        {
            string xml =
                "<test-run id='0'><test-suite type='Assembly' id='1' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='2' name='Library'>" +
                        "<test-suite type='TestSuite' id='3' name='Tests'>" +
                            "<test-suite type='TestSuite' id='4' name='Folder' />" +
                        "</test-suite>" +
                    "</test-suite>" +
                "</test-suite></test-run>";

            var testNode = new TestNode(xml);

            string result = NamespaceNodeHelper.GetTreeNodeName(GetTestNode(testNode, "2")!);

            Assert.That(result, Is.EqualTo("Library.Tests.Folder"));
        }

        [Test]
        public void GetTreeNodeName_ShouldReturnOnlyStartNodeName_WhenChildIsNotFolded()
        {
            string xml =
                "<test-run id='0'><test-suite type='Assembly' id='1' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='2' name='Library'>" +
                        "<test-suite type='TestSuite' id='3' name='One' />" +
                        "<test-suite type='TestSuite' id='4' name='Two' />" +
                    "</test-suite>" +
                "</test-suite></test-run>";

            var testNode = new TestNode(xml);

            string result = NamespaceNodeHelper.GetTreeNodeName(GetTestNode(testNode, "2")!);

            Assert.That(result, Is.EqualTo("Library"));
        }

        private TestNode? GetTestNode(TestNode testNode, string testId)
        {
            if (testNode.Id == testId)
                return testNode;

            foreach (TestNode node in testNode.Children)
            {
                if (GetTestNode(node, testId) is { } foundNode)
                    return foundNode;
            }

            return null;
        }
    }
}
