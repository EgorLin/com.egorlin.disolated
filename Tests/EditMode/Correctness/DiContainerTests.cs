using System;
using System.Collections.Generic;
using System.Linq;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Data.Types;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Injectors.Exceptions;
using EgorLin.DIsolated.LifeCycle.Runners;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;

namespace EgorLin.DIsolated.Tests
{
    public class DiContainerTests
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
        public void Singleton_ResolvesSameInstance()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();

            var first = container.Resolve<IA>();
            var second = container.Resolve<IA>();

            Assert.AreSame(first, second);
        }

        [Test]
        public void Transient_ResolvesNewInstance()
        {
            var container = new DiContainer();
            container.BindTransient<A, IA>();

            var first = container.Resolve<IA>();
            var second = container.Resolve<IA>();

            Assert.AreNotSame(first, second);
        }

        [Test]
        public void LastBindingWins()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            container.BindSingleton<AAlt, IA>();

            Assert.IsInstanceOf<AAlt>(container.Resolve<IA>());
        }

        [Test]
        public void Enumerable_ResolvesAllBindings_InOrder()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            container.BindSingleton<AAlt, IA>();

            var all = container.Resolve<IEnumerable<IA>>().ToArray();

            Assert.AreEqual(2, all.Length);
            Assert.IsInstanceOf<A>(all[0]);
            Assert.IsInstanceOf<AAlt>(all[1]);
        }

        [Test]
        public void ContractBinding_DoesNotRegisterConcrete()
        {
            var container = new DiContainer();
            container.Bind(TypeBinding.Singleton, typeof(A), typeof(IA));

            Assert.IsInstanceOf<A>(container.Resolve<IA>());
            Assert.IsFalse(container.TryResolve<A>(out _));
        }

        [Test]
        public void ConcreteBinding_WhenNoContractsGiven()
        {
            var container = new DiContainer();
            container.BindSingleton<A>();

            Assert.IsInstanceOf<A>(container.Resolve<A>());
        }

        [Test]
        public void MultipleContracts_ShareSameSingletonInstance()
        {
            var container = new DiContainer();
            container.Bind(TypeBinding.Singleton, typeof(MultiContract), typeof(IA), typeof(IB));

            var asA = container.Resolve<IA>();
            var asB = container.Resolve<IB>();

            Assert.AreSame(asA, asB);
        }

        [Test]
        public void BindInstance_ResolvesExactInstance()
        {
            var container = new DiContainer();
            var instance = new A();
            container.BindInstance<IA>(instance);

            Assert.AreSame(instance, container.Resolve<IA>());
        }

        [Test]
        public void Child_ResolvesParentBinding()
        {
            var parent = new DiContainer();
            parent.BindSingleton<A, IA>();

            var child = new DiContainer(parent);

            Assert.AreSame(parent.Resolve<IA>(), child.Resolve<IA>());
        }

        [Test]
        public void Child_Override_Wins_And_ParentUnaffected()
        {
            var parent = new DiContainer();
            parent.BindSingleton<A, IA>();

            var child = new DiContainer(parent);
            child.BindSingleton<AAlt, IA>();

            Assert.IsInstanceOf<AAlt>(child.Resolve<IA>());
            Assert.IsInstanceOf<A>(parent.Resolve<IA>());
        }

        [Test]
        public void Build_BindsSelf()
        {
            var container = new DiContainer();
            container.Build();

            Assert.AreSame(container, container.Resolve<IObjectResolver>());
            Assert.AreSame(container, container.Resolve<DiContainer>());
        }

        [Test]
        public void Resolve_Unknown_Throws_And_TryResolveReturnsFalse()
        {
            var container = new DiContainer();

            Assert.Throws<InvalidOperationException>(() => container.Resolve<IA>());
            Assert.IsFalse(container.TryResolve<IA>(out IA _));
        }

        [Test]
        public void Constructor_WithMostParameters_IsSelected()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            container.BindTransient<TwoCtor>();

            var resolved = container.Resolve<TwoCtor>();

            Assert.IsTrue(resolved.UsedDependency);
            Assert.IsInstanceOf<A>(resolved.A);
        }

        [Test]
        public void DefaultParameterValue_IsUsed_WhenDependencyMissing()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            container.BindTransient<OptionalDependency>();

            var resolved = container.Resolve<OptionalDependency>();

            Assert.AreEqual(5, resolved.Count);
        }

        [Test]
        public void MissingContract_Throws()
        {
            var container = new DiContainer();
            container.BindTransient<NeedsNotRegistered>();

            var exception = Assert.Throws<ExceptionConstructorInjector>(() => container.Resolve<NeedsNotRegistered>());

            StringAssert.Contains("Cannot resolve contract", exception.Message);
        }

        [Test]
        public void ValueType_WithoutConstructor_ResolvesDefault()
        {
            var container = new DiContainer();
            container.BindTransient<PlainStruct>();

            var resolved = container.Resolve<PlainStruct>();

            Assert.AreEqual(0, resolved.Value);
        }

        [Test]
        public void Graph_Chain_Resolves()
        {
            var container = BuildChain();

            var resolved = container.Resolve<Chain10>();

            Assert.IsNotNull(resolved.Value);
            Assert.IsNotNull(resolved.Value.Value);
        }

        [Test]
        public void Graph_Diamond_Resolves()
        {
            var container = new DiContainer();
            container.BindTransient<A, IA>();
            container.BindTransient<B, IB>();
            container.BindTransient<C, IC>();
            container.BindTransient<D, ID>();

            var resolved = container.Resolve<ID>() as D;

            Assert.IsNotNull(resolved);
            Assert.IsInstanceOf<IB>(resolved.B);
            Assert.IsInstanceOf<IC>(resolved.C);
        }

        [Test]
        public void GenericItems_ResolveThroughEnumerable()
        {
            var container = new DiContainer();
            for (var i = 0; i < 4; i++)
            {
                container.Bind(TypeBinding.Transient, typeof(GenericItem<>).MakeGenericType(Markers.All[i]), typeof(IItem));
            }

            var all = container.Resolve<IEnumerable<IItem>>().ToArray();

            Assert.AreEqual(4, all.Length);
            Assert.IsTrue(all.All(item => item is IItem));
        }

        private static DiContainer BuildChain()
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
            return container;
        }
    }
}
