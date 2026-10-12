using System;
using System.Collections;
using System.Collections.Generic;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.LifeCycle;
using EgorLin.DIsolated.LifeCycle.Runners;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace EgorLin.DIsolated.Tests.PlayMode
{
    public class BootstrapLifecycleTests
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

        [UnityTest]
        public IEnumerator Bootstrap_And_Tickable_RunThroughPlayerLoop()
        {
            var bootstrap = new TestBootstrap();
            var tickable = new TestTickable();

            var container = new DiContainer();
            container.BindInstance<IBootstrap>(bootstrap);
            container.BindInstance<ITickable>(tickable);
            container.Build();

            yield return null;
            yield return null;

            Assert.GreaterOrEqual(bootstrap.BootCount, 1);
            Assert.GreaterOrEqual(tickable.TickCount, 1);

            container.Dispose();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Disposable_RunsThroughPlayerLoop()
        {
            var log = new List<int>();
            var container = new DiContainer();
            container.BindInstance<IDisposable>(new RecordingDisposable(log, 0));
            container.BindInstance<IDisposable>(new RecordingDisposable(log, 1));
            container.Build();

            yield return null;

            container.Dispose();
            yield return null;

            CollectionAssert.AreEqual(new[] { 1, 0 }, log);
        }
    }
}
