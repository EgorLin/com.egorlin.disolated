using System.Collections.Generic;
using EgorLin.DIsolated.Core;

namespace EgorLin.DIsolated.Installers.Extensions
{
	public static class ExtensionsDiContainer
	{
		public static void BindInstallerDependencies(this DiContainer diContainer, IEnumerable<IDependency> dependencies)
		{
			foreach (var dependency in dependencies)
			{
				var type = dependency.GetType();
				
				diContainer.BindInstance(dependency, type);
			}
		}
	}
}
