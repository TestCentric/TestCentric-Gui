// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using NSubstitute;
using NUnit.Engine;
using NUnit.Framework;
using TestCentric.Gui.Model.Fakes;

namespace TestCentric.Gui.Model
{
    /// <summary>
    /// Checks ownership of an asynchronous runner between a stop request and run completion.
    /// </summary>
    [TestFixture]
    public class TestCentricRunnerTests
    {
        private TestModel _model = null!;
        private TestEventDispatcher _events = null!;
        private ITestEngine _engine = null!;
        private TestCentricRunner _runner = null!;
        private List<ControlledRun> _runs = null!;

        [SetUp]
        public void SetUp()
        {
            _model = new TestModel(new MockTestEngine());
            _events = (TestEventDispatcher)_model.Events;
            _engine = Substitute.For<ITestEngine>();
            _runner = new TestCentricRunner(_model, _engine, _events);
            _runs = new List<ControlledRun>();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _runner.Dispose();
                _model.Dispose();
            }
            finally
            {
                foreach (var run in _runs)
                    run.Dispose();
            }
        }

        [Test]
        public void RequestStopWithoutActiveRunDoesNotAcquireRunner()
        {
            _runner.RequestStop();

            _engine.DidNotReceiveWithAnyArgs().GetRunner(null!);
        }

        [Test]
        public void RequestStopRetainsRunnerUntilRunFinished()
        {
            var run = CreateRun();
            _engine.GetRunner(Arg.Any<TestPackage>()).Returns(run.Runner);
            StartRun();

            _runner.RequestStop();

            Assert.That(run.StopRequested.Wait(TimeSpan.FromSeconds(5)), Is.True,
                "The stop command was not delivered to the runner.");
            Assert.That(run.Disposed.Wait(TimeSpan.FromMilliseconds(250)), Is.False,
                "Returning from RequestStop does not mean that execution has finished.");
            Assert.That(_runner.IsTestRunning, Is.True);

            FinishRun();

            run.Runner.Received(1).Dispose();
            Assert.That(_runner.IsTestRunning, Is.False);
        }

        [Test]
        public void AnotherRunCanCompleteAfterCooperativeStop()
        {
            var first = CreateRun();
            var second = CreateRun();
            _engine.GetRunner(Arg.Any<TestPackage>()).Returns(first.Runner, second.Runner);
            StartRun();
            _runner.RequestStop();
            Assert.That(first.StopRequested.Wait(TimeSpan.FromSeconds(5)), Is.True);
            Assert.That(first.Disposed.Wait(TimeSpan.FromMilliseconds(250)), Is.False);
            FinishRun();

            StartRun();
            Assert.That(_runner.IsTestRunning, Is.True);
            FinishRun();

            first.Runner.Received(1).Dispose();
            second.Runner.Received(1).Dispose();
        }

        [Test]
        public void StopReturningAfterCompletionDoesNotDisposeTheNextRun()
        {
            var first = CreateRun();
            var second = CreateRun();
            using var releaseStop = new ManualResetEventSlim();
            using var stopReturned = new ManualResetEventSlim();
            first.Runner.When(runner => runner.RequestStop()).Do(_ =>
            {
                first.StopRequested.Set();
                releaseStop.Wait(TimeSpan.FromSeconds(10));
                stopReturned.Set();
            });
            _engine.GetRunner(Arg.Any<TestPackage>()).Returns(first.Runner, second.Runner);
            StartRun();

            try
            {
                _runner.RequestStop();
                Assert.That(first.StopRequested.Wait(TimeSpan.FromSeconds(5)), Is.True);
                FinishRun();
                StartRun();
            }
            finally
            {
                releaseStop.Set();
            }

            Assert.That(stopReturned.Wait(TimeSpan.FromSeconds(5)), Is.True);
            Assert.That(second.Disposed.Wait(TimeSpan.FromMilliseconds(250)), Is.False,
                "Completion of an earlier stop request must not release a newer runner.");
            Assert.That(_runner.IsTestRunning, Is.True);
            FinishRun();
            second.Runner.Received(1).Dispose();
        }

        private void StartRun()
        {
            _runner.RunAsync(new TestPackage("test.dll"), NUnit.Engine.TestFilter.Empty);
        }

        private ControlledRun CreateRun()
        {
            var run = new ControlledRun();
            _runs.Add(run);
            return run;
        }

        private void FinishRun()
        {
            _events.OnTestEvent("<test-run id='0' result='Passed' total='0' passed='0' failed='0' inconclusive='0' skipped='0' asserts='0' duration='0' />");
        }

        /// <summary>
        /// Models a runner for which acknowledgement of a stop request is separate from completion.
        /// </summary>
        private sealed class ControlledRun : IDisposable
        {
            internal ITestRunner Runner { get; } = Substitute.For<ITestRunner>();
            internal ManualResetEventSlim StopRequested { get; } = new ManualResetEventSlim();
            internal ManualResetEventSlim Disposed { get; } = new ManualResetEventSlim();

            internal ControlledRun()
            {
                Runner.IsTestRunning.Returns(_ => !Disposed.IsSet);
                Runner.When(runner => runner.RequestStop()).Do(_ => StopRequested.Set());
                Runner.When(runner => runner.Dispose()).Do(_ => Disposed.Set());
            }

            public void Dispose()
            {
                StopRequested.Dispose();
                Disposed.Dispose();
            }
        }
    }
}
