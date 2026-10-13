using EgorLin.DIsolated.CodeGen;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.LifeCycle.Runners;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;

namespace EgorLin.DIsolated.Tests.PlayMode
{
    public class GeneratedConstructorTests
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
        public void GeneratedFactories_AreDiscovered()
        {
            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(A), out _));
            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(Service8), out _));
            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(OptionalDependency), out _));
        }

        [Test]
        public void Generated_Singleton_ResolvesSameInstance()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();

            Assert.AreSame(container.Resolve<IA>(), container.Resolve<IA>());
        }

        [Test]
        public void Generated_Transient_InjectsDependencies()
        {
            var container = BuildGraph();
            container.BindTransient<B, IB>();

            var resolved = container.Resolve<IB>();

            Assert.IsInstanceOf<A>(((B)resolved).A);
        }

        [Test]
        public void Generated_DefaultParameterValue_IsUsed()
        {
            var container = BuildGraph();
            container.BindTransient<OptionalDependency>();

            Assert.AreEqual(5, container.Resolve<OptionalDependency>().Count);
        }

        [Test]
        public void Generated_GraphChain_Resolves()
        {
            var container = new DiContainer();
            container.BindTransient<Chain1>();
            container.BindTransient<Chain2>();
            container.BindTransient<Chain3>();
            container.BindTransient<Chain4>();
            container.BindTransient<Chain5>();
            container.BindTransient<Chain6>();
            container.BindTransient<Chain7>();
            container.BindTransient<Chain8>();
            container.BindTransient<Chain9>();
            container.BindTransient<Chain10>();

            Assert.IsNotNull(container.Resolve<Chain10>().Value);
        }

        private static DiContainer BuildGraph()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            container.BindSingleton<B, IB>();
            container.BindSingleton<C, IC>();
            container.BindSingleton<D, ID>();
            return container;
        }
    }
}
