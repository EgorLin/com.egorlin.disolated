using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Installers;
using EgorLin.DIsolated.LifeCycle;

namespace EgorLin.DIsolated.Tests.Fixtures
{
    public sealed class TestInstaller : Installer<TestInstaller>
    {
        public override void InstallBindings(DiContainer diContainer)
        {
            diContainer.BindSingleton<A, IA>();
        }
    }

    public sealed class TestInstallerWithDep : Installer<TestInstallerWithDep, TestDependency>
    {
        public TestDependency Dependency { get; private set; }

        public override void Pass(TestDependency dependency) => Dependency = dependency;

        public override void InstallBindings(DiContainer diContainer)
        {
            diContainer.BindInstance(Dependency, typeof(TestDependency));
        }
    }

    public sealed class TestIsolatedInstaller : InstallerIsolated<TestIsolatedInstaller>
    {
        public override void InstallBindings(DiContainer diContainer)
        {
            diContainer.BindSingleton<Bar, IBar>();
            diContainer.BindSingleton<TestBootstrap, IBootstrap>();
        }

        public override void InstallIsolated(DiContainer diContainer)
        {
            diContainer.BindSingleton<Foo, IFoo>();
        }
    }
}
