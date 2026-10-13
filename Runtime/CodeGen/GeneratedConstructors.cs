using System;
using System.Reflection;

namespace EgorLin.DIsolated.CodeGen
{
	public static class GeneratedConstructors
	{
		public static void Require<T>()
		{
		}

		public static void Require(Type type)
		{
		}

		public static void Register(IGeneratedConstructorProvider provider)
		{
			GeneratedConstructorDiscovery.Register(provider);
		}

		public static int RegisterFromAssembly(Assembly assembly)
		{
			return GeneratedConstructorDiscovery.RegisterFromAssembly(assembly);
		}
	}
}
