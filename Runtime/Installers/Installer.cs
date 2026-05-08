using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Injectors.Factories;

namespace EgorLin.DIsolated.Installers
{
	public abstract class Installer<TDerived> : InstallerBase where TDerived : Installer<TDerived>
	{
		public static void Install(DiContainer diContainer)
		{
			var installer = FactoryConstructorDependencies.Create(diContainer, typeof(TDerived)) as TDerived;
			
			installer!.InstallBindings(diContainer);
		}
	}
	
	public abstract class InstallerDep<TDerived, TDependency> : InstallerDep<TDependency> where TDerived : InstallerDep<TDerived, TDependency>
	{
		public static void Install(DiContainer diContainer, TDependency dependencies = default)
		{
			var installer = FactoryConstructorDependencies.Create(diContainer, typeof(TDerived)) as TDerived;
			
			installer!.SetRefDependency(dependencies);
			installer!.InstallBindings(diContainer);
		}
	}
	
	public abstract class InstallerData<TDerived> : InstallerBase, IInstallerData where TDerived : InstallerData<TDerived>, IInstallerData, new()
	{
		public static void Install(DiContainer diContainer)
		{
			var installerData = new TDerived();
			
			installerData.InstallBindings(diContainer);
		}
	}
}
