using EgorLin.DIsolated.CodeGen;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Injectors.Exceptions;
using EgorLin.DIsolated.LifeCycle.Runners;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;

namespace EgorLin.DIsolated.Tests
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
        public void Tier1_BindingCall_RegistersFactory()
        {
            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(NeedsNotRegistered), out _));
        }

        [Test]
        public void Tier2_Attribute_RegistersFactory()
        {
            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(AttributedOnly), out _));
        }

        [Test]
        public void Tier3_Hint_RegistersFactory()
        {
            GeneratedConstructors.Require<HintOnly>();

            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(HintOnly), out _));
        }

        [Test]
        public void Tier3_ClosedGeneric_IsGeneratedAndResolves()
        {
            GeneratedConstructors.Require<GenericItem<M0>>();

            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(GenericItem<M0>), out _));

            var container = new DiContainer();
            container.BindTransient<GenericItem<M0>>();

            Assert.IsNotNull(container.Resolve<GenericItem<M0>>());
        }

        [Test]
        public void Interface_IsNotRegistered()
        {
            Assert.IsFalse(GeneratedConstructorDiscovery.TryGet(typeof(IA), out _));
        }

        [Test]
        public void GeneratedFactory_ExposesParameterTypes()
        {
            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(B), out var entry));
            CollectionAssert.AreEqual(new[] { typeof(IA) }, entry.Parameters);
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

            var first = container.Resolve<IB>();
            var second = container.Resolve<IB>();

            Assert.AreNotSame(first, second);
            Assert.IsInstanceOf<A>(((B)first).A);
        }

        [Test]
        public void Generated_Arity8_Resolves()
        {
            var container = BuildGraph();
            container.BindTransient<Service8>();

            var resolved = container.Resolve<Service8>();

            Assert.IsNotNull(resolved.A);
            Assert.IsNotNull(resolved.B);
            Assert.IsNotNull(resolved.C);
            Assert.IsNotNull(resolved.D);
            Assert.IsNotNull(resolved.E);
            Assert.IsNotNull(resolved.F);
            Assert.IsNotNull(resolved.G);
            Assert.IsNotNull(resolved.H);
        }

        [Test]
        public void Generated_MostParametersConstructor_IsSelected()
        {
            var container = BuildGraph();
            container.BindTransient<TwoCtor>();

            var resolved = container.Resolve<TwoCtor>();

            Assert.IsTrue(resolved.UsedDependency);
            Assert.IsInstanceOf<A>(resolved.A);
        }

        [Test]
        public void Generated_DefaultParameterValue_IsUsed()
        {
            var container = BuildGraph();
            container.BindTransient<OptionalDependency>();

            Assert.AreEqual(5, container.Resolve<OptionalDependency>().Count);
        }

        [Test]
        public void Generated_ValueType_ResolvesDefault()
        {
            var container = new DiContainer();
            container.BindTransient<PlainStruct>();

            Assert.AreEqual(0, container.Resolve<PlainStruct>().Value);
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

            var resolved = container.Resolve<Chain10>();

            Assert.IsNotNull(resolved.Value);
            Assert.IsNotNull(resolved.Value.Value);
        }

        [Test]
        public void Generated_MissingContract_ThrowsWrapped()
        {
            var container = new DiContainer();
            container.BindTransient<GeneratedNeedsNotRegistered>();

            var exception = Assert.Throws<ExceptionConstructorInjector>(() => container.Resolve<GeneratedNeedsNotRegistered>());

            StringAssert.Contains("Cannot resolve contract", exception.Message);
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
