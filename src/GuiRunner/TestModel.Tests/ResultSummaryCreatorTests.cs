// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using NUnit.Framework;
using System;

namespace TestCentric.Gui.Model
{
    [TestFixture]
    public class ResultSummaryCreatorTests
    {
        [Test]
        public void WhenResultNodeIsNotForTestRunExceptionIsThrown()
        {
            Assert.That(() => CreateResultSummary("<anything-other-than-test-run/>"),
                Throws.InstanceOf<InvalidOperationException>());

            Assert.That(() => CreateResultSummary("<test-run id='10'/>"),
                Throws.Nothing);
        }

        [Test]
        public void ResultOfTestRunIsValueOfOverallResult()
        {
            var innerXml = "<test-case result='Passed'/>";
            var summary = CreateResultSummary($"<test-run id='10' result='Failed'>{innerXml}</test-run>");

            Assert.That(summary.OverallResult, Is.EqualTo("Failed"));
        }

        [Test]
        public void WhenResultIsNotSpecified_PassedIsDefault()
        {
            var innerXml = "<test-case result='Failed'/>";
            var summary = CreateResultSummary($"<test-run id='10'>{innerXml}</test-run>");

            Assert.That(summary.OverallResult, Is.EqualTo("Passed"));
        }

        [Test]
        public void DurationOfTestRunIsValueOfDuration()
        {
            var innerXml = "<test-case duration='999' id='100' name='TestA' />";
            var summary = CreateResultSummary($"<test-run id='0' duration='1.9'>{innerXml}</test-run>");

            Assert.That(summary.Duration, Is.EqualTo(1.9));
        }

        [Test]
        public void WhenDurationIsNotSpecified_ZeroIsDefault()
        {
            var innerXml = "<test-case duration='999' id='100' name='TestA' />";
            var summary = CreateResultSummary($"<test-run id='0' > {innerXml}</test-run>");

            Assert.That(summary.Duration, Is.EqualTo(0.0));
        }

        [Test]
        public void StartTimeOfTestRunIsValueOfStartTime()
        {
            var expectedDate = new DateTime(2017, 7, 8, 6, 19, 23);
            var innerXml = $"<test-case start-time='{DateTime.MinValue:u}' id='100' name='TestA' fullname='TestA'/>";
            var summary = CreateResultSummary($"<test-run id='0' start-time='{expectedDate:u}'>{innerXml}</test-run>");

            Assert.That(summary.StartTime, Is.EqualTo(expectedDate));
        }

        [Test]
        public void WhenStartTimeIsNotSpecified_DateTimeMinValueIsDefault()
        {
            var innerXml = $"<test-case start-time='{DateTime.MaxValue:u}' id='100' name='TestA' />";
            var summary = CreateResultSummary($"<test-run id='0' >{innerXml}</test-run>");

            Assert.That(summary.StartTime, Is.EqualTo(DateTime.MinValue));
        }

        [Test]
        public void EndTimeOfTestRunIsValueOfEndTime()
        {
            var expectedDate = new DateTime(2017, 7, 8, 6, 19, 23);
            var innerXml = $"<test-case end-time='{DateTime.MaxValue:u}' id='100' name='TestA' />";
            var summary = CreateResultSummary($"<test-run id='0' end-time='{expectedDate:u}'>{innerXml}</test-run>");

            Assert.That(summary.EndTime, Is.EqualTo(expectedDate));
        }

        [Test]
        public void WhenEndTimeIsNotSpecified_DateTimeMaxValueIsDefault()
        {
            var innerXml = $"<test-case end-time='{DateTime.MinValue:u}' id='100' name='TestA'/>";
            var summary = CreateResultSummary($"<test-run id='0' >{innerXml}</test-run>");

            Assert.That(summary.EndTime, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void TestCountIsCountOfEachNestedTestCase()
        {
            var innerXml =
                "<test-case id='0' name='TestA'/>" +
                "<test-case id='1' name='TestB'/>" +
                "<test-suite id='20' name='Suite1' type='Assembly'>" +
                    "<test-case id='2' name='TestC'/>" +
                    "<test-case id='3' name='TestD'/>" +
                "</test-suite>";
            var summary = CreateResultSummary($"<test-run id='10'>{innerXml}</test-run>");

            Assert.That(summary.TestCount, Is.EqualTo(4));
        }

        [Test]
        public void WhenNoResultIsSpecifiedInTestCase_SkipCountIsIncremented()
        {
            var innerXml =
                "<test-case result='Passed' id='0' name='TestA'/>" +
                "<test-case/>";
            var summary = CreateResultSummary($"<test-run id ='10'>{innerXml}</test-run>");

            Assert.That(summary.TestCount, Is.EqualTo(2));
            Assert.That(summary.PassCount, Is.EqualTo(1));
            Assert.That(summary.SkipCount, Is.EqualTo(1));
        }

        [Test]
        public void ExtendedFailureInformationAreBasedOnLabel()
        {
            var innerXml =
                "<test-case id='0' name='TestA' result='Failed'/>" +
                "<test-case id='1' name='TestB' result='Failed' label='Invalid'/>" +
                "<test-case id='2' name='TestC' result='Failed' label='Anything else increases ErrorCount'/>" +
                "<test-case id='3' name='TestD' result='Failed' label='I am not null'/>";
            var summary = CreateResultSummary($"<test-run id='10'>{innerXml}</test-run>");

            Assert.That(summary.TestCount, Is.EqualTo(4));
            Assert.That(summary.FailedCount, Is.EqualTo(4));
            Assert.That(summary.FailureCount, Is.EqualTo(1));
            Assert.That(summary.InvalidCount, Is.EqualTo(1));
            Assert.That(summary.ErrorCount, Is.EqualTo(2));
        }

        [Test]
        public void ExtendedSkipInformationAreBasedOnLabel()
        {
            var innerXml =
                "<test-case id='0' name='TestA' result='Skipped'/>" +
                "<test-case id='1' name='TestB' result='Skipped' label='Ignored'/>" +
                "<test-case id='2' name='TestC' result='Skipped' label='Explicit'/>" +
                "<test-case id='3' name='TestD' result='Skipped' label='Anything else increases SkippedCount'/>" +
                "<test-case id='4' name='TestE' result='Skipped' label='I am not null'/>";
            var summary = CreateResultSummary($"<test-run id='10'>{innerXml}</test-run>");

            Assert.That(summary.TestCount, Is.EqualTo(5));
            Assert.That(summary.TotalSkipCount, Is.EqualTo(5));
            Assert.That(summary.SkipCount, Is.EqualTo(3));
            Assert.That(summary.IgnoreCount, Is.EqualTo(1));
            Assert.That(summary.ExplicitCount, Is.EqualTo(1));
        }

        [Test]
        public void InvalidTestSuitesAreTracked()
        {
            var innerXml =
                "<test-suite id='0' name='TestA' result='Failed' label='Invalid' type='TestFixture'/>" +
                "<test-suite id='1' name='TestB' result='Failed' label='Invalid' type='Assembly'/>";
            var summary = CreateResultSummary($"<test-run id='10'>{innerXml}</test-run>");

            Assert.That(summary.InvalidTestFixtures, Is.EqualTo(1));
            Assert.That(summary.InvalidAssemblies, Is.EqualTo(1));
            Assert.That(summary.UnexpectedError, Is.False);
        }

        [Test]
        public void ErrorAssembliesMarkSummaryAsUnexpectedError()
        {
            var innerXml =
                "<test-suite id='0' name='TestA' result='Failed' label='Invalid' type='Assembly'/>" +
                "<test-suite id='1' name='TestB' result='Failed' label='Error' type='Assembly'/>";
            var summary = CreateResultSummary($"<test-run id='10'>{innerXml}</test-run>");

            Assert.That(summary.InvalidAssemblies, Is.EqualTo(2));
            Assert.That(summary.UnexpectedError, Is.True);
        }

        private ResultSummary CreateResultSummary(string xml)
        {
            var resultNode = new ResultNode(xml);

            return ResultSummaryCreator.FromResultNode(resultNode);
        }
    }
}
