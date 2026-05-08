using System;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Injectors.Factories;

namespace EgorLin.DIsolated.Resolvers
{
	public class ResolverSingletonType : IResolver
	{
		private DiContainer _diContainer;
		private Type _type;
		
		private object _instance;

		public ResolverSingletonType(Type type, DiContainer diContainer)
		{
			_diContainer = diContainer;
			_type = type;
		}
		
		public object Resolve()
		{
			if (_instance == null)
			{
				_instance = FactoryConstructorDependencies.Create(_diContainer, _type);
			}
			
			return _instance;
		}

		public void Dispose()
		{
			_instance = null;
			_type = null;
			_diContainer = null;
		}
	}
}