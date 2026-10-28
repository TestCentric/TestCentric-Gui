// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

namespace TestCentric.Gui.Presenters
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit;
    using TestCentric.Gui.Model;

    /// <summary>
    /// Helper class to manage folding of namespace nodes
    /// </summary>
    public static class NamespaceNodeHelper
    {
        /// <summary>
        /// Determines whether the specified node is considered "folded".
        /// </summary>
        /// <c>true</c> if both the node and its parent are namespaces, and the parent 
        /// has exactly one child (this node); otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFolded(TestNode testNode)
        {
            Guard.ArgumentNotNull(testNode, nameof(testNode));

            TestNode? parent = testNode.Parent;
            return parent != null && testNode.IsNamespace && parent.IsNamespace && parent.Children.Count == 1;
        }

        /// <summary>
        /// Gets the display name for a namespace node by combining all subsequent folded namespace child nodes.
        /// </summary>
        /// <returns>A dot-separated string representing the combined path of the folded namespace node chain.</returns>
        public static string GetTreeNodeName(TestNode testNode)
        {
            Guard.ArgumentNotNull(testNode, nameof(testNode));

            List<TestNode> foldedNodes = GetFoldedChildren(testNode);
            return string.Join(".", foldedNodes.Select(n => n.Name));
        }

        private static List<TestNode> GetFoldedChildren(TestNode startNode)
        {
            List<TestNode> nodes = [startNode];
            var current = startNode;

            while (current.Children.FirstOrDefault() is TestNode next && IsFolded(next))
            {
                nodes.Add(next);
                current = next;
            }

            return nodes;
        }
    }
}
