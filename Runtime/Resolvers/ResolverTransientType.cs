using System;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Injectors.Factories;

namespace EgorLin.DIsolated.Resolvers
{
	public class ResolverTransientType : IResolver
	{
		private Type _type;
		private DiContainer _diContainer;

		public ResolverTransientType(Type type, DiContainer diContainer)
		{
			_type = type;
			_diContainer = diContainer;
		}
		
		public object Resolve()
		{
			return FactoryConstructorDependencies.Create(_diContainer, _type);
		}

		public void Dispose()
		{
			_type = null;
			_diContainer = null;
		}
	}
}
