using System.Collections.Generic;
using EgorLin.DIsolated.Core;

namespace EgorLin.DIsolated.Installers.Extensions
{
	public static class ExtensionsDiContainer
	{
		public static void BindInstallerDependencies(this DiContainer diContainer,
			IEnumerable<IInstallerDependency> dependencies)
		{
			foreach (var dependency in dependencies)
			{
				var type = dependency.GetType();
				
				diContainer.BindInstance(dependency, type);
			}
		}
		
		public static void BindInstallerData(this DiContainer diContainer,
			IEnumerable<IInstallerData> installers)
		{
			foreach (var installer in installers)
			{
				installer.InstallBindings(diContainer);
			}
		}
	}
}
