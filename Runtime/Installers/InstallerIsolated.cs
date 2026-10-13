using System;
using System.Collections.Generic;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Injectors.Factories;
using EgorLin.DIsolated.LifeCycle;

namespace EgorLin.DIsolated.Installers
{
	public abstract class InstallerIsolated<TDerived> : InstallerBase where TDerived : InstallerIsolated<TDerived>
	{
		private static readonly HashSet<Type> LifecycleTypes = new()
		{ 
			typeof(IBootstrap), typeof(ITickable), typeof(IDisposable)
		};
		
		public static void Install(DiContainer outerContainer)
		{
			var isolatedContainer = new DiContainer(outerContainer);

			var installer = FactoryConstructorDependencies.Create(outerContainer, typeof(TDerived)) as TDerived;

			installer!.InstallBindings(isolatedContainer);
			GetPublicBindings(isolatedContainer, outerContainer);

			installer!.InstallIsolated(isolatedContainer);

			isolatedContainer.Build();
		}

		private static void GetPublicBindings(DiContainer isolatedContainer, DiContainer outerContainer)
		{
			foreach (var (type, resolvers) in isolatedContainer.GetBindings())
			{
				if (LifecycleTypes.Contains(type))
				{
					continue;
				}
				
				foreach (var resolver in resolvers)
				{
					outerContainer.AddResolver(type, resolver);
				}
			}
		}

		public abstract void InstallIsolated(DiContainer diContainer);
	}
}
