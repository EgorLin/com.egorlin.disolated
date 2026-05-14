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
	
	public abstract class Installer<TDerived, TDependency> : InstallerBase where TDerived : Installer<TDerived, TDependency>
	{
		public static void Install(DiContainer diContainer, TDependency dependencies = default)
		{
			var installer = FactoryConstructorDependencies.Create(diContainer, typeof(TDerived)) as TDerived;
			
			installer!.Pass(dependencies);
			installer!.InstallBindings(diContainer);
		}
		
		public abstract void Pass(TDependency dependency);
	}
}
