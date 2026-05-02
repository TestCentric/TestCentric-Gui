// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
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
                TreeNode topNode = null;
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
    }
}
