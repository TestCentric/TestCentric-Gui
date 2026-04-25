// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Windows.Forms;
using NSubstitute;
using NUnit.Framework;
using TestCentric.Gui.Model;
using TestCentric.Gui.Views;

namespace TestCentric.Gui.Presenters.TestTree
{
    public class WhenTestRunCompletes : TreeViewPresenterTestBase
    {
        [SetUp]
        public void SimulateTestRunCompletion()
        {
            _presenter = new TreeViewPresenter(_view, _model, new TreeDisplayStrategyFactory());
            _view.InvokeIfRequired(Arg.Do<MethodInvoker>(x => x.Invoke()));

            ClearAllReceivedCalls();

            _model.HasTests.Returns(true);
            _model.HasResults.Returns(true);
        }

        static object[] resultData = new object[] {
            new object[] { TestTreeView.RunningIndex_Success, TestTreeView.SuccessIndex },
            new object[] { TestTreeView.RunningIndex_Failure, TestTreeView.FailureIndex },
            new object[] { TestTreeView.RunningIndex_Ignored, TestTreeView.IgnoredIndex },
            new object[] { TestTreeView.RunningIndex_Warning, TestTreeView.WarningIndex },
            new object[] { TestTreeView.RunningIndex, TestTreeView.SkippedIndex },
            new object[] { TestTreeView.PendingIndex, TestTreeView.SkippedIndex },
        };

        [TestCaseSource(nameof(resultData))]
        public void RunningIcons_AreUpdated_WhenTestRunFinish(int runningIndex, int expectedIndex)
        {
            var testNode = new TestNode("<test-run id='1'><test-suite id='100'><test-case id='200'/></test-suite></test-run>");

            // Make it look like the view loaded
            _view.Load += Raise.Event<System.EventHandler>(_view, new System.EventArgs());

            TreeView treeView = new TreeView();
            _view.TreeView.Returns(treeView);

            // We can't construct a TreeNodeCollection, so we fake it
            var nodes = new TreeNode().Nodes;
            nodes.Add(new TreeNode("test.dll") { ImageIndex = runningIndex });
            _view.Nodes.Returns(nodes);

            _model.Events.TestLoaded += Raise.Event<TestNodeEventHandler>(new TestNodeEventArgs(testNode));
            FireRunFinishedEvent(new ResultNode("<test-run id='1' result='Passed' />"));

            _view.Received().SetImageIndex(Arg.Compat.Any<TreeNode>(), expectedIndex, false);
        }
    }
}
