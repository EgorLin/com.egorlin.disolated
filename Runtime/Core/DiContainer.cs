using System;
using System.Collections.Generic;
using EgorLin.DIsolated.Core.Data.Types;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.LifeCycle.Runners;
using EgorLin.DIsolated.Resolvers;

namespace EgorLin.DIsolated.Core
{
	public class DiContainer : IObjectResolver, IDisposable
	{
        private readonly Dictionary<Type, List<IResolver>> _registrations = new();
        
        private readonly DiContainer _parent;
        
        public Dictionary<Type, List<IResolver>> Registrations => _registrations;

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
            if (!_registrations.ContainsKey(instanceType))
            {
                _registrations.Add(instanceType, new List<IResolver>());
            }

            var resolver = new ResolverSingletonInstance(instance);
            
            _registrations[instanceType].Add(resolver);
        }

        public void BindInstance<TContract>(TContract instance) where TContract : class
        {
            var type = typeof(TContract);
            
            BindInstance(instance, type);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
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
            
            if (_registrations.TryGetValue(type, out var resolvers))
            {
                var lastResolver = resolvers[^1];
                
                instance = lastResolver.Resolve();
                return true;
            }
            
            if (_parent != null)
            {
                return _parent.TryResolve(type, out instance);
            }

            instance = null;
            return false;
        }
        
        public void AddResolver(Type typeConcrete, IResolver resolver)
        {
            if (!_registrations.ContainsKey(typeConcrete))
            {
                _registrations.Add(typeConcrete, new List<IResolver>());
            }

            _registrations[typeConcrete].Add(resolver);
        }


        public void Build()
        {
            BindInstance(this, typeof(IObjectResolver));
            BindInstance(this, typeof(DiContainer));
            
            RunnerLifeCycleDiContainer.AddContainer(this);
        }

        public void Clear()
        {
            _registrations.Clear();
        }

        public void Dispose()
        {
            RunnerLifeCycleDiContainer.RemoveContainer(this);
        }

        private bool TryResolveList(Type type, out IReadOnlyList<object> instance)
        {
            if (_registrations.TryGetValue(type, out var resolvers))
            {
                var countResolvers = resolvers.Count;
                var objects = new List<object>(countResolvers);

                for (int indexObject = 0; indexObject < countResolvers; indexObject++)
                {
                    objects.Add(resolvers[indexObject].Resolve());
                }

                instance = objects;
                return true;
            }

            instance = null;
            return false;
        }

        private IResolver GetResolver(TypeBinding typeBinding, Type type)
        {
            IResolver resolver = typeBinding switch 
            {
                TypeBinding.Singleton => new ResolverSingletonType(type, this),
                TypeBinding.Transient => new ResolverTransientType(type, this),
                _ => throw new ArgumentOutOfRangeException(nameof(typeBinding), typeBinding, null)
            };
            return resolver;
        }
    }
}