using System;
using System.Diagnostics;
using EgorLin.DIsolated.CodeGen;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;

namespace EgorLin.DIsolated.Tests
{
    public class GeneratedConstructorRegistrationTests
    {
        private sealed class FakeGeneratedService
        {
        }

        private sealed class FakeProvider : IGeneratedConstructorProvider
        {
            public static int Registrations;

            public void RegisterConstructors(GeneratedConstructorRegistry registry)
            {
                Registrations++;
                registry.Register(typeof(FakeGeneratedService), _ => new FakeGeneratedService(), Type.EmptyTypes);
            }
        }

        [TearDown]
        public void TearDown()
        {
            RestoreProviders();
        }

        [Test]
        public void Register_AddsProvider()
        {
            FakeProvider.Registrations = 0;

            GeneratedConstructors.Register(new FakeProvider());

            Assert.AreEqual(1, FakeProvider.Registrations);
            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(FakeGeneratedService), out _));
        }

        [Test]
        public void Register_IsIdempotent_ForSameProviderType()
        {
            GeneratedConstructorDiscovery.Reset();
            FakeProvider.Registrations = 0;

            GeneratedConstructors.Register(new FakeProvider());
            GeneratedConstructors.Register(new FakeProvider());

            Assert.AreEqual(1, FakeProvider.Registrations);
            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(FakeGeneratedService), out _));
        }

        [Test]
        public void TryGet_DoesNotScanAssemblies()
        {
            GeneratedConstructorDiscovery.Reset();

            Assert.IsFalse(GeneratedConstructorDiscovery.TryGet(typeof(NeedsNotRegistered), out _));
            Assert.IsFalse(GeneratedConstructorDiscovery.TryGet(typeof(A), out _));
        }

        [Test]
        public void RegisterFromAssembly_RegistersProviderAfterReset()
        {
            GeneratedConstructorDiscovery.Reset();

            Assert.IsFalse(GeneratedConstructorDiscovery.TryGet(typeof(NeedsNotRegistered), out _));

            var registered = GeneratedConstructors.RegisterFromAssembly(typeof(GeneratedConstructorRegistrationTests).Assembly);

            Assert.GreaterOrEqual(registered, 0);
            Assert.IsTrue(GeneratedConstructorDiscovery.TryGet(typeof(NeedsNotRegistered), out _));
        }

        [Test]
        public void UngeneratedType_FallsBackToReflection()
        {
            var type = ReflectionOnlyTypes.Service;

            Assert.IsFalse(GeneratedConstructorDiscovery.TryGet(type, out _));

            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            container.BindTransient(ReflectionOnlyTypes.Service);

            var resolved = container.Resolve(type);

            Assert.IsInstanceOf<ReflectionOnlyService>(resolved);
            Assert.IsNotNull(((ReflectionOnlyService)resolved).A);
        }

        [Test]
        public void ColdRegistration_RegistersOnlyGeneratedProviders()
        {
            GeneratedConstructorDiscovery.Reset();

            var stopwatch = Stopwatch.StartNew();
            GeneratedConstructors.RegisterFromAssembly(typeof(GeneratedConstructorRegistrationTests).Assembly);
            GeneratedConstructors.RegisterFromAssembly(typeof(IA).Assembly);
            stopwatch.Stop();

            TestContext.WriteLine(
                $"[Disolated] explicit cold registration: {stopwatch.Elapsed.TotalMilliseconds:F3} ms, entries={GeneratedConstructorDiscovery.RegisteredCount}");

            Assert.Greater(GeneratedConstructorDiscovery.RegisteredCount, 0);
        }

        private static void RestoreProviders()
        {
            GeneratedConstructors.RegisterFromAssembly(typeof(DiContainer).Assembly);
            GeneratedConstructors.RegisterFromAssembly(typeof(IA).Assembly);
            GeneratedConstructors.RegisterFromAssembly(typeof(GeneratedConstructorRegistrationTests).Assembly);
        }
    }
}
