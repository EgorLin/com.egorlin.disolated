using System;
using System.Collections.Generic;
using System.Reflection;

namespace EgorLin.DIsolated.CodeGen
{
	internal static class GeneratedConstructorDiscovery
	{
		private static readonly GeneratedConstructorRegistry Registry = new GeneratedConstructorRegistry();
		private static readonly HashSet<Type> RegisteredProviders = new HashSet<Type>();
		private static bool _initialized;

		internal static bool Enabled = true;

		internal static void Register(IGeneratedConstructorProvider provider)
		{
			if (provider == null || !RegisteredProviders.Add(provider.GetType()))
			{
				return;
			}

			provider.RegisterConstructors(Registry);
		}

		internal static bool TryGet(Type concrete, out GeneratedConstructorRegistry.Entry entry)
		{
			if (!Enabled)
			{
				entry = null;
				return false;
			}

			EnsureInitialized();

			return Registry.TryGet(concrete, out entry);
		}

		private static void EnsureInitialized()
		{
			if (_initialized)
			{
				return;
			}

			_initialized = true;

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			for (var i = 0; i < assemblies.Length; i++)
			{
				RegisterAssembly(assemblies[i]);
			}
		}

		private static void RegisterAssembly(Assembly assembly)
		{
			Type[] types;

			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException exception)
			{
				types = exception.Types;
			}
			catch (Exception)
			{
				return;
			}

			for (var i = 0; i < types.Length; i++)
			{
				var type = types[i];

				if (type == null || type.IsAbstract || type.IsInterface)
				{
					continue;
				}

				if (!typeof(IGeneratedConstructorProvider).IsAssignableFrom(type))
				{
					continue;
				}

				if (RegisteredProviders.Contains(type))
				{
					continue;
				}

				try
				{
					var provider = (IGeneratedConstructorProvider)Activator.CreateInstance(type, true);

					Register(provider);
				}
				catch (Exception)
				{
					// Ignored: a provider that cannot be created must not break container startup.
				}
			}
		}
	}
}
