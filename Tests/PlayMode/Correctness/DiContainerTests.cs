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

namespace EgorLin.DIsolated.Tests.PlayMode
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

            Assert.AreSame(container.Resolve<IA>(), container.Resolve<IA>());
        }

        [Test]
        public void Transient_ResolvesNewInstance()
        {
            var container = new DiContainer();
            container.BindTransient<A, IA>();

            Assert.AreNotSame(container.Resolve<IA>(), container.Resolve<IA>());
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
        public void MultipleContracts_ShareSameSingletonInstance()
        {
            var container = new DiContainer();
            container.Bind(TypeBinding.Singleton, typeof(MultiContract), typeof(IA), typeof(IB));

            Assert.AreSame(container.Resolve<IA>(), container.Resolve<IB>());
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
        public void Build_BindsSelf()
        {
            var container = new DiContainer();
            container.Build();

            Assert.AreSame(container, container.Resolve<IObjectResolver>());
            Assert.AreSame(container, container.Resolve<DiContainer>());
        }

        [Test]
        public void Resolve_Unknown_Throws()
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

            Assert.AreEqual(5, container.Resolve<OptionalDependency>().Count);
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

            Assert.AreEqual(0, container.Resolve<PlainStruct>().Value);
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

            Assert.AreEqual(4, container.Resolve<IEnumerable<IItem>>().Count());
        }
    }
}
