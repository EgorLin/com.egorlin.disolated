using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Injectors.Data.Types;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;
using UnityEngine;

namespace EgorLin.DIsolated.Tests.PlayMode
{
    public class BackendTests
    {
        [Test]
        public void ScriptingBackend_MatchesCompilationDefines()
        {
            var backend = TypeBackendScripting.Current;

            Debug.Log($"[DIsolated] Scripting backend under test: {backend}");

#if ENABLE_IL2CPP
            Assert.AreEqual(TypeBackendScripting.Backend.IL2CPP, backend);
#elif ENABLE_MONO
            Assert.AreEqual(TypeBackendScripting.Backend.Mono, backend);
#else
            Assert.AreEqual(TypeBackendScripting.Backend.Undefined, backend);
#endif
        }

        [Test]
        public void Construction_WorksForAllArities_ViaActiveBackend()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            container.BindSingleton<B, IB>();
            container.BindSingleton<C, IC>();
            container.BindSingleton<D, ID>();
            container.BindTransient<Service0>();
            container.BindTransient<Service4>();
            container.BindTransient<Service8>();

            Assert.IsNotNull(container.Resolve<Service0>());
            Assert.IsNotNull(container.Resolve<Service4>());
            Assert.IsNotNull(container.Resolve<Service8>());
        }

        [Test]
        public void Construction_WorksForDeepGraph_ViaActiveBackend()
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

            Assert.IsNotNull(container.Resolve<Chain9>());
        }
    }
}
