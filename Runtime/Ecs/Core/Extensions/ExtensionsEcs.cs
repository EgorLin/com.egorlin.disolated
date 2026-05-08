using System;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Data.Types;

namespace EgorLin.DIsolated.Ecs.Core.Extensions
{
	public static class ExtensionsEcs
	{
		public static void BindSystem<T>(this DiContainer container) where T : ISystem
		{
			container.Bind(TypeBinding.Singleton, typeof(T), typeof(ISystem));
		}
		
		public static void BindSystem(this DiContainer container, Type type)
		{
			container.Bind(TypeBinding.Singleton, type, typeof(ISystem));
		}

		public static void BindSystemDestroy<T>(this DiContainer container) where T : struct, IComponent
		{
			container.Bind(TypeBinding.Singleton, typeof(SystemDestroyComponent<T>), typeof(ISystem));
		}
		
		public static void BindFeature<T>(this DiContainer container) where T : IEcsFeature
		{
			container.Bind(TypeBinding.Singleton, typeof(T), typeof(IEcsFeature));
		}
	}
}
