using EgorLin.DIsolated.Core;

namespace EgorLin.DIsolated.Installers
{
	public abstract class InstallerData<TDerived> : InstallerBase, IInstallerData where TDerived : InstallerData<TDerived>, IInstallerData, new()
	{
		public static void Install(DiContainer diContainer)
		{
			var installerData = new TDerived();
			
			installerData.InstallBindings(diContainer);
		}
	}
}
