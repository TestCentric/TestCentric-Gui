// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace TestCentric.Gui.Presenters
{
    using System.Linq;
    using Model;
    using Views;

    /// <summary>
    /// GroupDisplayStrategy is the abstract base class for 
    /// DisplayStrategies that list tests in various groupings.
    /// </summary>
    public abstract class GroupDisplayStrategy : DisplayStrategy
    {
        protected TestGrouping _topLevelGrouping;

        #region Construction and Initialization

        public GroupDisplayStrategy(ITestTreeView view, ITestModel model)
            : base(view, model)
        {
            _topLevelGrouping = CreateTestGrouping(GroupBy);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Post a test result to the tree, changing the treeNode
        /// color to reflect success or failure. Overridden here
        /// to allow for moving nodes from one group to another
        /// based on the result of running the test.
        /// </summary>
        public override void OnTestFinished(ResultNode result)
        {
            _view.InvokeIfRequired(() =>
            {
                _topLevelGrouping?.OnTestFinished(result);
                
                base.OnTestFinished(result);
            });
        }

        /// <summary>
        /// During a run the tree is repainted at intervals rather than after every
        /// result: moving a test between groups renames the group nodes, and each
        /// repaint of the owner-drawn TreeView measures every expanded node. The
        /// interval grows with the cost of a repaint, so that large trees spend
        /// most of the run processing results rather than repainting.
        /// </summary>
        public override void OnTestRunStarting()
        {
            base.OnTestRunStarting();

            _view.InvokeIfRequired(() =>
            {
                if (_runRefreshTimer != null)
                    return;

                _treeView.BeginUpdate();
                var timer = new Timer { Interval = RunRefreshInterval };
                timer.Tick += (s, e) =>
                {
                    // Covers a run that finishes after this strategy has been replaced
                    if (!_model.IsTestRunning)
                    {
                        StopRunRefresh();
                        return;
                    }

                    var repaint = Stopwatch.StartNew();
                    ApplyPendingNodeTexts();
                    _treeView.EndUpdate();
                    // Paint now: while results keep arriving, a queued WM_PAINT is not processed
                    _treeView.Update();
                    _treeView.BeginUpdate();
                    timer.Interval = Math.Max(RunRefreshInterval, (int)repaint.ElapsedMilliseconds * RepaintCostFactor);
                };
                _runRefreshTimer = timer;
                timer.Start();
            });
        }

        public override void OnTestRunFinished()
        {
            _view.InvokeIfRequired(StopRunRefresh);

            base.OnTestRunFinished();
        }

        public void Add(TreeNode treeNode)
        {
            _view.Add(treeNode);
        }

        /// <summary>
        /// ApplyResultToGroup takes no action in the base GroupDisplayStrategyClass.
        /// It should be overridden in derived strategies with categories based
        /// on the result of the test, which may change upon execution.
        /// </summary>
        /// <param name="result">A ResultNode</param>
        public virtual void ApplyResultToGroup(ResultNode result) { }

        #endregion

        #region Protected Members

        protected abstract string GroupBy { get; }

        protected override void SetTreeNodeText(TreeNode treeNode, string text)
        {
            _view.InvokeIfRequired(() =>
            {
                if (_runRefreshTimer == null)
                    treeNode.Text = text;
                else
                    _pendingNodeTexts[treeNode] = text;
            });
        }

        protected TreeNode MakeTreeNode(TestGroup group, bool recursive)
        {
            TreeNode treeNode = new TreeNode(group.Name)
            {
                Name = group.Name,
                Tag = group,
            };

            if (recursive)
            {
                if (group.SubGroups.Count > 0)
                    foreach (var subGroup in group.SubGroups)
                        treeNode.Nodes.Add(MakeTreeNode(subGroup, recursive));
                else
                    foreach (TestNode test in group)
                        AddTreeNodeToCollection(test, treeNode.Nodes);
            }

            return group.TreeNode = treeNode;
        }

        protected TestGrouping CreateTestGrouping(string groupBy)
        {
            switch (groupBy)
            {
                case null: // Needed by tests that use NSubstitute
                case "UNGROUPED":
                    return new UngroupedGrouping(this);
                case "OUTCOME":
                    return new OutcomeGrouping(this);
                case "DURATION":
                    return new DurationGrouping(this);
                case "CATEGORY":
                    return new CategoryGrouping(this, true);
                case "ASSEMBLY":
                    return new AssemblyGrouping(this);
                case "FIXTURE":
                    return new TestFixtureGrouping(this);
                default:
                    throw new ArgumentException($"Unknown grouping ID: {groupBy}");
            }
        }

        protected void UpdateDisplay()
        {
            if (_topLevelGrouping != null)
            {
                this.ClearTree();
                TreeNode? topNode = null;
                foreach (var group in _topLevelGrouping.Groups)
                {
                    var treeNode = MakeTreeNode(group, true);
                    group.TreeNode = treeNode;
                    treeNode.Expand();
                    if (group.TestNodes.Count() > 0)
                    {
                        _view.Add(treeNode);
                        if (topNode == null)
                            topNode = treeNode;
                    }
                }
                if (topNode != null)
                    topNode.EnsureVisible();
            }
        }

        #endregion

        #region Private Members

        private const int RunRefreshInterval = 250; // milliseconds

        // Minimum interval between repaints, as a multiple of the last repaint's duration
        private const int RepaintCostFactor = 4;

        private Timer? _runRefreshTimer;

        private void StopRunRefresh()
        {
            if (_runRefreshTimer == null)
                return;

            _runRefreshTimer.Stop();
            _runRefreshTimer.Dispose();
            _runRefreshTimer = null;
            ApplyPendingNodeTexts();
            _treeView.EndUpdate();
        }

        // Renaming a node makes the TreeView measure it again, even inside an update
        // block. During a run, group names change with every result, so only the
        // latest text of each node is kept and applied when the tree is repainted.
        private readonly Dictionary<TreeNode, string> _pendingNodeTexts = new Dictionary<TreeNode, string>();

        private void ApplyPendingNodeTexts()
        {
            foreach (var entry in _pendingNodeTexts)
                entry.Key.Text = entry.Value;

            _pendingNodeTexts.Clear();
        }

        #endregion
    }
}
