using System;
using System.Collections.Generic;
using EgorLin.DIsolated.Core.Data.Types;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Injectors.Factories;
using EgorLin.DIsolated.LifeCycle.Runners;

namespace EgorLin.DIsolated.Core
{
	public class DiContainer : IObjectResolver, IDisposable
	{
		private struct Binding
		{
			public Type Contract;
			public List<Resolver> Resolvers;
		}

		private Dictionary<int, Binding> _bindings = new Dictionary<int, Binding>();

		private readonly DiContainer _parent;

		public DiContainer() { }

		public DiContainer(DiContainer parent)
		{
			_parent = parent;
		}

		public void Bind(TypeBinding typeBinding, Type typeConcrete, params Type[] typeContracts)
		{
			var resolver = GetResolver(typeBinding, typeConcrete);

			if (typeContracts.Length == 0)
			{
				AddResolver(typeConcrete, resolver);

				return;
			}

			foreach (var typeContract in typeContracts)
			{
				AddResolver(typeContract, resolver);
			}
		}

		public void BindInstance(object instance, Type instanceType)
		{
			AddResolver(instanceType, new Resolver(instance));
		}

		public void BindInstance<TContract>(TContract instance) where TContract : class
		{
			BindInstance(instance, typeof(TContract));
		}

		public T Resolve<T>()
		{
			var type = typeof(T);

			if (type.IsEnumerable(out _))
			{
				return (T)Resolve(type);
			}

			if (TryResolveId(TypeId<T>.Value, out var instance))
			{
				return (T)instance;
			}

			throw new InvalidOperationException($"Type not registered: {type}");
		}

		public object Resolve(Type type)
		{
			if (TryResolve(type, out var instance))
			{
				return instance;
			}

			throw new InvalidOperationException($"Type not registered: {type}");
		}

		public bool TryResolve<T>(out T instance) where T : class
		{
			var hasResolve = TryResolve(typeof(T), out var newInstance);

			if (hasResolve)
			{
				instance = (T)newInstance;
				return true;
			}

			instance = null;
			return false;
		}

		public bool TryResolve(Type type, out object instance)
		{
			if (type.IsEnumerable(out var typeEnumerable))
			{
				var isResolved = TryResolveList(typeEnumerable, out var enumerable);

				instance = null;
				if (isResolved)
				{
					instance = enumerable.CastDynamic(typeEnumerable);
				}

				return isResolved;
			}

			return TryResolveId(TypeRegistry.GetId(type), out instance);
		}

		internal void AddResolver(Type typeConcrete, Resolver resolver)
		{
			var id = TypeRegistry.GetId(typeConcrete);

			if (!_bindings.TryGetValue(id, out var binding))
			{
				binding = new Binding
				{
					Contract = typeConcrete,
					Resolvers = new List<Resolver>(1)
				};

				_bindings.Add(id, binding);
			}

			binding.Resolvers.Add(resolver);
		}

		public void Build()
		{
			BindInstance(this, typeof(IObjectResolver));
			BindInstance(this, typeof(DiContainer));

			RunnerLifeCycleDiContainer.AddContainer(this);
		}

		public void Clear()
		{
			_bindings = new Dictionary<int, Binding>();
		}

		public void Dispose()
		{
			RunnerLifeCycleDiContainer.RemoveContainer(this);
		}

		internal IEnumerable<(Type Type, List<Resolver> Resolvers)> GetBindings()
		{
			foreach (var pair in _bindings)
			{
				yield return (pair.Value.Contract, pair.Value.Resolvers);
			}
		}

		private bool TryResolveId(int id, out object instance)
		{
			if (_bindings.TryGetValue(id, out var binding))
			{
				instance = binding.Resolvers[^1].Resolve();
				return true;
			}

			if (_parent != null)
			{
				return _parent.TryResolveId(id, out instance);
			}

			instance = null;
			return false;
		}

		private bool TryResolveList(Type type, out IReadOnlyList<object> instance)
		{
			var id = TypeRegistry.GetId(type);

			if (_bindings.TryGetValue(id, out var binding))
			{
				var resolvers = binding.Resolvers;
				var objects = new List<object>(resolvers.Count);

				for (var i = 0; i < resolvers.Count; i++)
				{
					objects.Add(resolvers[i].Resolve());
				}

				instance = objects;
				return true;
			}

			instance = null;
			return false;
		}

		private Resolver GetResolver(TypeBinding typeBinding, Type type)
		{
			switch (typeBinding)
			{
				case TypeBinding.Singleton:
					return new Resolver(this, type, true);

				case TypeBinding.Transient:
					return new Resolver(this, type, false);

				default:
					throw new ArgumentOutOfRangeException(nameof(typeBinding), typeBinding, null);
			}
		}
	}
}
