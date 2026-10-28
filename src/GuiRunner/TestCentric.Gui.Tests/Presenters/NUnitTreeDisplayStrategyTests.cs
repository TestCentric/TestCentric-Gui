// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Windows.Forms;
using NSubstitute;
using NUnit.Framework;

namespace TestCentric.Gui.Presenters.TestTree
{
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using NUnit;
    using NUnit.Framework.Internal;
    using TestCentric.Gui.Controls;
    using Views;

    public abstract class DisplayStrategyTests
    {
        protected ITestTreeView _view = null!;
        protected ITestModel _model = null!;
        protected DisplayStrategy _strategy = null!;

        [SetUp]
        public void CreateDisplayStrategy()
        {
            _view = Substitute.For<ITestTreeView>();
            _model = Substitute.For<ITestModel>();
            _model.TreeConfiguration.NUnitTreeShowNamespaces = true;
            _model.TreeConfiguration.DisplayFormat = "NUNIT_TREE";
            _model.TreeConfiguration.NUnitTreeShowFixtures = true;
            _model.TreeConfiguration.NUnitTreeShowNamespaces = true;
            _model.TreeConfiguration.NUnitTreeShowAssemblies = true;

            // We can't construct a TreeNodeCollection, so we fake it
            var nodes = new TreeNode().Nodes;
            nodes.Add(new TreeNode("test.dll"));
            _view.Nodes.Returns(nodes);

            var treeView = new TestCentricTreeView();
            _view.TreeView.Returns(treeView);

            var project = new TestCentricProject("MyProject", "dummy.dll");
            _model.TestCentricProject.Returns(project);

            _strategy = GetDisplayStrategy();
        }

        protected abstract DisplayStrategy GetDisplayStrategy();

        // TODO: FIX
        //[Test]
        public void WhenTestsAreLoaded_TreeViewIsLoaded()
        {
            _strategy.OnTestLoaded(
                new TestNode("<test-run id='1'><test-suite id='42'/><test-suite id='99'/></test-run>"),
                null);

            _view.Received().Clear();
            _view.Received().Add(Arg.Is<TreeNode>((tn) => (tn.Tag as TestNode)!.Id == "42"));
            _view.Received().Add(Arg.Is<TreeNode>((tn) => (tn.Tag as TestNode)!.Id == "99"));
        }

        [Test]
        public void WhenTestsAreUnloaded_TreeViewIsCleared()
        {
            _strategy.OnTestUnloaded();

            _view.Received().Clear();
        }
    }

    public class NUnitTreeDisplayStrategyTests : DisplayStrategyTests
    {
        protected override DisplayStrategy GetDisplayStrategy()
        {
            return new NUnitTreeDisplayStrategy(_view, _model);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnStrategyCreated_OutcomeFilter_IsVisible(bool isVisible)
        {
            // Arrange
            var view = Substitute.For<ITestTreeView>();
            var model = Substitute.For<ITestModel>();
            model.Settings.Gui.TestTree.ShowFilter.Returns(isVisible);

            // Act
            var strategy = new NUnitTreeDisplayStrategy(view, model);

            // Assert
            view.Received().SetTestFilterVisibility(isVisible);
        }

        [Test]
        public void OnTestLoaded_Namespaces_AreShown_NamespaceNodes_AreCreated()
        {
            // Arrange
            _model.TreeConfiguration.NUnitTreeShowNamespaces = true;
            string xml =
                "<test-run id='0'> <test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='1-1031' name='Library'>" +
                    "</test-suite>" +
                "</test-suite>  </test-run>";

            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1030"), Is.True);
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1031"), Is.True);
        }

        [Test]
        public void OnTestLoaded_Namespaces_AreNotShown_NamespaceNodes_AreNotCreated()
        {
            // Arrange
            _model.TreeConfiguration.NUnitTreeShowNamespaces = false;
            string xml =
                "<test-run id='0'> <test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='1-1031' name='Library'>" +
                    "</test-suite>" +
                "</test-suite> </test-run>";

            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1030"), Is.True);
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1031"), Is.False);
        }

        [Test]
        public void OnTestLoaded_Assemblies_AreNotShown_AssemblyNode_IsNotCreated()
        {
            // Arrange
            _model.TreeConfiguration.NUnitTreeShowAssemblies = false;
            string xml =
                "<test-run id='0'> <test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='1-1031' name='Library'>" +
                    "</test-suite>" +
                "</test-suite> </test-run>";

            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1031"), Is.True);
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1030"), Is.False);
        }

        [Test]
        public void OnTestLoaded_Fixtures_AreNotShown_FixtureNode_IsNotCreated()
        {
            // Arrange
            _model.TreeConfiguration.NUnitTreeShowFixtures = false;

            string xml =
                "<test-run id='0'> <test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='1-1031' name='Library'>" +
                        "<test-suite type='TestFixture' id='1-1032' name='FixtureA'/>" +
                        "<test-suite type='TestFixture' id='1-1033' name='FixtureB'/>" +
                    "</test-suite>" +
                "</test-suite> </test-run>";

            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1032"), Is.False);
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1033"), Is.False);
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1031"), Is.True);
            Assert.That(NodeCollectionContainsTestNode(_view.Nodes, "1-1030"), Is.True);
        }

        [TestCase("Passed", "Passed", true, true, TestTreeView.SuccessIndex)]
        [TestCase("Passed", "Failed", true, true, TestTreeView.FailureIndex)]
        [TestCase("Passed", "", true, true, TestTreeView.SuccessIndex)]
        [TestCase("Warning", "Passed", true, true, TestTreeView.WarningIndex)]
        [TestCase("", "", true, true, TestTreeView.SkippedIndex)]
        [TestCase("Passed", "Passed", false, false, TestTreeView.SuccessIndex_NotLatestRun)]
        [TestCase("Passed", "Failed", true, false, TestTreeView.FailureIndex)]
        [TestCase("Passed", "Warning", false, true, TestTreeView.WarningIndex)]
        [TestCase("Warning", "Passed", false, false, TestTreeView.WarningIndex_NotLatestRun)]
        public void OnTestLoaded_TestResult_AreAppliedToTreeNodes(string child1Result, string child2Result, bool child1ResultFromLatestRun, bool child2ResultFromLatestRun, int expectedImageIndex)
        {
            // Arrange
            _model.HasResults.Returns(true);
            string xml =
                "<test-run id='0'> <test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='1-1031' name='Library'>" +
                        "<test-suite type='TestFixture' id='1-1032' name='FixtureA'>" +
                            "<test-case id='1-1040' name='TestA' /> " +
                            "<test-case id='1-1041' name='TestB' /> " +
                        "</test-suite>" +
                    "</test-suite>" +
                "</test-suite> </test-run>";

            var result1 = new ResultNode("<test-case id='1-1040' name='TestA' result='" + child1Result + "' />") { IsLatestRun = child1ResultFromLatestRun };
            var result2 = new ResultNode("<test-case id='1-1041' name='TestB' result='" + child2Result + "' />") { IsLatestRun = child2ResultFromLatestRun };
            if (!string.IsNullOrEmpty(child1Result))
                _model.TestResultManager.GetResultForTest("1-1040").Returns(result1);

            if (!string.IsNullOrEmpty(child2Result))
                _model.TestResultManager.GetResultForTest("1-1041").Returns(result2);

            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            var treeNode = GetTreeNode(_view.Nodes, new[] { "1-1030", "1-1031", "1-1032" });
            _view.Received().SetImageIndex(treeNode, expectedImageIndex, false);

        }

        [Test]
        public void OnTestLoaded_NamespacesNodes_ContainingOneNamespace_AreFolded()
        {
            // Arrange
            string xml =
                "<test-run id='0'> <test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='1-1031' name='Library'>" +
                        "<test-suite type='TestSuite' id='1-1032' name='Test'>" +
                            "<test-suite type='TestSuite' id='1-1033' name='Folder'>" +
                            "</test-suite>" +
                        "</test-suite>" +
                    "</test-suite>" +
                "</test-suite> </test-run>";

            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            TreeNode treeNode = GetTreeNode(_view.Nodes, "1-1030");
            Assert.That(treeNode.Text, Is.EqualTo("Library.Test.dll (0 tests)"));
            Assert.That(treeNode.Nodes.Count, Is.EqualTo(1));

            treeNode = GetTreeNode(treeNode.Nodes, "1-1031");
            Assert.That(treeNode.Text, Is.EqualTo("Library.Test.Folder (0 tests)"));
            Assert.That(treeNode.Nodes.Count, Is.EqualTo(0));
        }

        [Test]
        public void OnTestLoaded_NamespacesNode_ContainingTwoNamespaces_AreNotFolded()
        {
            // Arrange
            string xml =
                "<test-run id='0'> <test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                    "<test-suite type='TestSuite' id='1-1031' name='Library'>" +
                        "<test-suite type='TestSuite' id='1-1032' name='Test' >" +
                            "<test-case id='1-1040' name='Test1'/>" +
                        "</test-suite>" +
                        "<test-suite type='TestSuite' id='1-1033' name='Folder' >" +
                            "<test-case id='1-1050' name='Test1'/>" +
                        "</test-suite>" +
                    "</test-suite>" +
                "</test-suite> </test-run>";


            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            TreeNode treeNode = GetTreeNode(_view.Nodes, "1-1030");
            Assert.That(treeNode.Text, Is.EqualTo("Library.Test.dll (2 tests)"));
            Assert.That(treeNode.Nodes.Count, Is.EqualTo(1));

            treeNode = GetTreeNode(treeNode.Nodes, "1-1031");
            Assert.That(treeNode.Text, Is.EqualTo("Library (2 tests)"));
            Assert.That(treeNode.Nodes.Count, Is.EqualTo(2));

            var treeNode1 = GetTreeNode(treeNode.Nodes, "1-1032");
            Assert.That(treeNode1.Text, Is.EqualTo("Test (1 test)"));
            Assert.That(treeNode1.Nodes.Count, Is.EqualTo(1));

            var treeNode2 = GetTreeNode(treeNode.Nodes, "1-1033");
            Assert.That(treeNode2.Text, Is.EqualTo("Folder (1 test)"));
            Assert.That(treeNode2.Nodes.Count, Is.EqualTo(1));
        }

        [Test]
        public void OnTestLoaded_SetupFixtureNode_ContainingOneNamespace_AreFolded()
        {
            // Arrange
            string xml =
                "<test-run id='0'> <test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                    "<test-suite type='SetUpFixture' id='1-1031' name='Library'>" +
                        "<test-suite type='TestSuite' id='1-1032' name='Test'>" +
                            "<test-suite type='SetUpFixture' id='1-1033' name='Folder'>" +
                            "</test-suite>" +
                        "</test-suite>" +
                    "</test-suite>" +
                "</test-suite> </test-run>";

            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            TreeNode treeNode = GetTreeNode(_view.Nodes, "1-1030");
            Assert.That(treeNode.Text, Is.EqualTo("Library.Test.dll (0 tests)"));
            Assert.That(treeNode.Nodes.Count, Is.EqualTo(1));

            treeNode = GetTreeNode(treeNode.Nodes, "1-1031");
            Assert.That(treeNode.Text, Is.EqualTo("Library.Test.Folder (0 tests)"));
            Assert.That(treeNode.Nodes.Count, Is.EqualTo(0));
        }

        [Test]
        public void OnTestLoaded_SetupFixtureNode_ContainingTwoNamespaces_AreNotFolded()
        {
            // Arrange
            string xml =
                "<test-run id='0'> <test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                    "<test-suite type='SetUpFixture' id='1-1031' name='Library'>" +
                        "<test-suite type='TestSuite' id='1-1032' name='Test'>" +
                            "<test-case id='1-1040' name='Test1'/>" +
                        "</test-suite>" +
                        "<test-suite type='TestSuite' id='1-1033' name='Folder'>" +
                            "<test-case id='1-1050' name='Test1'/>" +
                        "</test-suite>" +
                    "</test-suite>" +
                "</test-suite> </test-run>";

            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            TreeNode treeNode = GetTreeNode(_view.Nodes, "1-1030");
            Assert.That(treeNode.Text, Is.EqualTo("Library.Test.dll (2 tests)"));
            Assert.That(treeNode.Nodes.Count, Is.EqualTo(1));

            treeNode = GetTreeNode(treeNode.Nodes, "1-1031");
            Assert.That(treeNode.Text, Is.EqualTo("Library (2 tests)"));
            Assert.That(treeNode.Nodes.Count, Is.EqualTo(2));

            var treeNode1 = GetTreeNode(treeNode.Nodes, "1-1032");
            Assert.That(treeNode1.Text, Is.EqualTo("Test (1 test)"));
            Assert.That(treeNode1.Nodes.Count, Is.EqualTo(1));

            var treeNode2 = GetTreeNode(treeNode.Nodes, "1-1033");
            Assert.That(treeNode2.Text, Is.EqualTo("Folder (1 test)"));
            Assert.That(treeNode2.Nodes.Count, Is.EqualTo(1));
        }

        [Test]
        public void OnTestLoaded_TestFilters_AreEnabled()
        {
            // Arrange
            string xml =
                "<test-suite type='Assembly' id='1-1030' name='Library.Test.dll'>" +
                "</test-suite>";

            // Act
            _strategy.OnTestLoaded(new TestNode(xml), null);

            // Assert
            _view.Received().EnableTestFilter(true);
        }

        [Test]
        public void OnTestUnloaded_TestFilters_AreDisabled()
        {
            // Arrange + Act
            _strategy.OnTestUnloaded();

            // Assert
            _view.Received().EnableTestFilter(false);
        }

        [Test]
        public void CollapseToFixtures_AllFixtureNodes_AreShown()
        {
            // Arrange
            var treeView = new TestCentricTreeView();
            _view.TreeView.Returns(treeView);

            var rootNode = _view.Nodes[0];
            var fixtureNode1 = CreateTreeNode("<test-suite type='TestFixture' id='2' name='FixtureA'/>");
            var fixtureNode2 = CreateTreeNode("<test-suite type='TestFixture' id='3' name='FixtureB'/>");
            rootNode.Nodes.AddRange(new[] { fixtureNode1, fixtureNode2 });

            var testcase1 = CreateTreeNode("<test-case id='10' name='Test1'/>");
            var testcase2 = CreateTreeNode("<test-case id='20' name='Test2'/>");
            fixtureNode1.Nodes.AddRange(new[] { testcase1, testcase2 });

            var testcase3 = CreateTreeNode("<test-case id='30' name='Test3'/>");
            var testcase4 = CreateTreeNode("<test-case id='40' name='Test4'/>");
            fixtureNode2.Nodes.AddRange(new[] { testcase3, testcase4 });

            rootNode.ExpandAll();

            // Act
            _strategy.CollapseToFixtures();

            // Assert
            Assert.That(rootNode.IsExpanded, Is.EqualTo(true));
            Assert.That(fixtureNode1.IsExpanded, Is.EqualTo(false));
            Assert.That(fixtureNode2.IsExpanded, Is.EqualTo(false));
        }

        TreeNode CreateTreeNode(string testNodeXml)
        {
            var testNode = new TestNode(testNodeXml);
            return new TreeNode() { Tag = testNode };
        }

        private bool NodeCollectionContainsTestNode(TreeNodeCollection nodes, string testNodeId)
        {
            foreach (TreeNode treeNode in nodes)
            {
                TestNode? testNode = treeNode.Tag as TestNode;
                if (testNode != null && testNode.Id == testNodeId)
                    return true;

                if (NodeCollectionContainsTestNode(treeNode.Nodes, testNodeId))
                    return true;
            }

            return false;
        }

        private TreeNode GetTreeNode(TreeNodeCollection nodes, string testNodeId)
        {
            return GetTreeNode(nodes, new[] { testNodeId });
        }

        private TreeNode GetTreeNode(TreeNodeCollection nodes, IEnumerable<string> testIds)
        {
            TreeNode? treeNode = null;
            foreach (string testId in testIds)
            {
                treeNode = nodes.Cast<TreeNode>().FirstOrDefault(node => HasTestId(node, testId));
                Assert.That(treeNode, Is.Not.Null, $"TreeNode with name '{testId}' not found.");
                nodes = treeNode!.Nodes;
            }

            return treeNode.ShouldNotBeNull();
        }

        private static bool HasTestId(TreeNode node, string testId) =>
            node.Tag switch
            {
                TestNode testNode => testNode.Id == testId,
                TestGroup testGroup => testGroup.RootTestNode?.Id == testId,
                _ => false
            };
    }
}
