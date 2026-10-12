using System;
using System.Collections.Generic;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.LifeCycle;
using EgorLin.DIsolated.LifeCycle.Runners;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;

namespace EgorLin.DIsolated.Tests
{
    public class LifecycleTests
    {
        [SetUp]
        public void SetUp()
        {
            RunnerLifeCycleDiContainer.ClearAll();
        }

        [TearDown]
        public void TearDown()
        {
            RunnerLifeCycleDiContainer.ClearAll();
        }

        [Test]
        public void Bootstrap_RunsExactlyOnce()
        {
            var bootstrap = new TestBootstrap();
            var container = new DiContainer();
            container.BindInstance<IBootstrap>(bootstrap);
            container.Build();

            LifecyclePump.Pump();
            LifecyclePump.Pump();

            Assert.AreEqual(1, bootstrap.BootCount);
        }

        [Test]
        public void Tickable_TicksEveryPump()
        {
            var tickable = new TestTickable();
            var container = new DiContainer();
            container.BindInstance<ITickable>(tickable);
            container.Build();

            LifecyclePump.Pump();
            LifecyclePump.Pump();
            LifecyclePump.Pump();

            Assert.AreEqual(3, tickable.TickCount);
        }

        [Test]
        public void Disposables_AreDisposedInReverseOrder_OnContainerDispose()
        {
            var log = new List<int>();
            var container = new DiContainer();
            container.BindInstance<IDisposable>(new RecordingDisposable(log, 0));
            container.BindInstance<IDisposable>(new RecordingDisposable(log, 1));
            container.Build();

            LifecyclePump.Pump();

            container.Dispose();
            LifecyclePump.Pump();

            CollectionAssert.AreEqual(new[] { 1, 0 }, log);
        }

        [Test]
        public void Tickable_StopsAfterContainerDisposed()
        {
            var tickable = new TestTickable();
            var container = new DiContainer();
            container.BindInstance<ITickable>(tickable);
            container.Build();

            LifecyclePump.Pump();

            container.Dispose();
            LifecyclePump.Pump();

            var ticksAfterDisposal = tickable.TickCount;

            LifecyclePump.Pump();
            LifecyclePump.Pump();

            Assert.AreEqual(ticksAfterDisposal, tickable.TickCount);
        }

        [Test]
        public void ClearAll_RemovesTrackedContainers()
        {
            var tickable = new TestTickable();
            var container = new DiContainer();
            container.BindInstance<ITickable>(tickable);
            container.Build();
            LifecyclePump.Pump();

            RunnerLifeCycleDiContainer.ClearAll();
            LifecyclePump.Pump();

            Assert.AreEqual(1, tickable.TickCount);
        }
    }
}
