using EgorLin.DIsolated.Core;

namespace EgorLin.DIsolated.Installers
{
	public interface IInstaller
	{
		public void InstallBindings(DiContainer diContainer);
	}
}