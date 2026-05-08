using EgorLin.DIsolated.Core;

namespace EgorLin.DIsolated.Installers
{
	public abstract class InstallerBase : IInstaller
	{
		public abstract void InstallBindings(DiContainer diContainer);
	}
}
