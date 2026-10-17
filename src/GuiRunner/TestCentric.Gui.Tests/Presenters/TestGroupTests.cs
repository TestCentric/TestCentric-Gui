// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;

namespace TestCentric.Gui.Presenters
{
    using Model;
    using NSubstitute;
    using TestCentric.Gui.Model.Filter;
    using TestCentric.Gui.Views;

    public class TestGroupTests
    {
        private TestGroup _group = null!;

        [SetUp]
        public void CreateTestGroup()
        {
            _group = new TestGroup("NAME");
            _group.Add(MakeTestNode("1", "Tom", "Runnable"));
            _group.Add(MakeTestNode("2", "Dick", "Explicit"));
            _group.Add(MakeTestNode("3", "Harry", "Runnable"));
        }

        [Test]
        public void GroupName()
        {
            Assert.That(_group.Name, Is.EqualTo("NAME"));
        }

        [Test]
        public void TestTestFilter()
        {
            ITestCentricTestFilter guiFilter = Substitute.For<ITestCentricTestFilter>();
            var filter = XmlHelper.CreateXmlNode(_group.GetTestFilter(guiFilter).XmlText);
            Assert.That(filter.Name, Is.EqualTo("filter"));
            Assert.That(filter.ChildNodes.Count, Is.EqualTo(1));

            filter = filter.ChildNodes[0];
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter!.Name, Is.EqualTo("or"));
            Assert.That(filter.ChildNodes.Count, Is.EqualTo(2));

            var ids = new List<string>();
            foreach (XmlNode child in filter.ChildNodes)
                ids.Add(child.InnerText);
            Assert.That(ids, Is.EqualTo(new string[] { "1", "3" }));
        }

        [TestCase("Explicit", "Explicit", ExpectedResult = "Explicit")]
        [TestCase("Explicit", "Ignored", ExpectedResult = null)]
        [TestCase("Explicit", null, ExpectedResult = null)]
        public string? GetCommonSkippedLabel(string? label1, string? label2)
        {
            var group = new TestGroup("FIXTURE");
            group.Add(MakeTestNode("1", "Tom", "Explicit"));
            group.Add(MakeTestNode("2", "Dick", "Explicit"));

            var resultManager = Substitute.For<ITestResultManager>();
            resultManager.GetResultForTest("1").Returns(MakeSkippedResult("1", label1));
            resultManager.GetResultForTest("2").Returns(MakeSkippedResult("2", label2));

            return group.GetCommonSkippedLabel(resultManager);
        }

        [Test]
        public void GetCommonSkippedLabel_EmptyGroup_ReturnsNull()
        {
            var resultManager = Substitute.For<ITestResultManager>();

            Assert.That(new TestGroup("EMPTY").GetCommonSkippedLabel(resultManager), Is.Null);
        }

        private ResultNode? MakeSkippedResult(string id, string? label)
        {
            return label != null
                ? new ResultNode($"<test-case id='{id}' name='Test{id}' result='Skipped' label='{label}'/>")
                : null;
        }

        private TestNode MakeTestNode(string id, string name, string runState)
        {
            return new TestNode(string.Format("<test-case id='{0}' name='{1}' runstate='{2}'/>", id, name, runState));
        }
    }
}
