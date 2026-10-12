using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Installers;
using EgorLin.DIsolated.LifeCycle;
using EgorLin.DIsolated.LifeCycle.Runners;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;

namespace EgorLin.DIsolated.Tests
{
    public class InstallerTests
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
        public void Installer_InstallsBindings()
        {
            var container = new DiContainer();

            Installer<TestInstaller>.Install(container);

            Assert.IsInstanceOf<A>(container.Resolve<IA>());
        }

        [Test]
        public void Installer_WithDependency_PassesAndBindsIt()
        {
            var container = new DiContainer();
            var dependency = new TestDependency();

            Installer<TestInstallerWithDep, TestDependency>.Install(container, dependency);

            Assert.AreSame(dependency, container.Resolve<TestDependency>());
        }

        [Test]
        public void InstallerIsolated_ReExportsNonLifecycleBindings()
        {
            var container = new DiContainer();

            InstallerIsolated<TestIsolatedInstaller>.Install(container);

            Assert.IsInstanceOf<Bar>(container.Resolve<IBar>());
        }

        [Test]
        public void InstallerIsolated_SkipsLifecycleTypes()
        {
            var container = new DiContainer();

            InstallerIsolated<TestIsolatedInstaller>.Install(container);

            Assert.IsFalse(container.TryResolve<IBootstrap>(out _));
        }

        [Test]
        public void InstallerIsolated_DoesNotReExportIsolatedOnlyBindings()
        {
            var container = new DiContainer();

            InstallerIsolated<TestIsolatedInstaller>.Install(container);

            Assert.IsFalse(container.TryResolve<IFoo>(out _));
        }
    }
}
