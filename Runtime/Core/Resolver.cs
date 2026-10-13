using System;
using EgorLin.DIsolated.Injectors.Factories;

namespace EgorLin.DIsolated.Core
{
	internal sealed class Resolver
	{
		private readonly DiContainer _container;
		private readonly Type _type;
		private readonly bool _singleton;
		private readonly bool _isInstance;
		private object _instance;

		public Resolver(DiContainer container, Type type, bool singleton)
		{
			_container = container;
			_type = type;
			_singleton = singleton;
		}

		public Resolver(object instance)
		{
			_instance = instance;
			_isInstance = true;
		}

		public object Resolve()
		{
			if (_isInstance)
			{
				return _instance;
			}

			if (_singleton)
			{
				if (_instance == null)
				{
					_instance = FactoryConstructorDependencies.Create(_container, _type);
				}

				return _instance;
			}

			return FactoryConstructorDependencies.Create(_container, _type);
		}
	}
}
