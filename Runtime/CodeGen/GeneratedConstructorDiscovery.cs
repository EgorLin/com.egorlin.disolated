using System;
using System.Collections.Generic;
using System.Reflection;

namespace EgorLin.DIsolated.CodeGen
{
	internal static class GeneratedConstructorDiscovery
	{
		private static GeneratedConstructorRegistry _registry = new GeneratedConstructorRegistry();
		private static readonly HashSet<Type> RegisteredProviders = new HashSet<Type>();

		internal static bool Enabled = true;

		internal static void Register(IGeneratedConstructorProvider provider)
		{
			if (provider == null || !RegisteredProviders.Add(provider.GetType()))
			{
				return;
			}

			provider.RegisterConstructors(_registry);
		}

		internal static int RegisterFromAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				return 0;
			}

			object[] attributes;

			try
			{
				attributes = assembly.GetCustomAttributes(typeof(GeneratedConstructorProviderAttribute), false);
			}
			catch (Exception)
			{
				return 0;
			}

			var registered = 0;

			for (var i = 0; i < attributes.Length; i++)
			{
				if (attributes[i] is not GeneratedConstructorProviderAttribute attribute || attribute.ProviderType == null)
				{
					continue;
				}

				if (RegisteredProviders.Contains(attribute.ProviderType))
				{
					continue;
				}

				try
				{
					var provider = (IGeneratedConstructorProvider)Activator.CreateInstance(attribute.ProviderType, true);

					Register(provider);
					registered++;
				}
				catch (Exception)
				{
					// Ignored: a provider that cannot be created must not break container startup.
				}
			}

			return registered;
		}

		internal static bool TryGet(Type concrete, out GeneratedConstructorRegistry.Entry entry)
		{
			if (!Enabled)
			{
				entry = null;
				return false;
			}

			return _registry.TryGet(concrete, out entry);
		}

		internal static int RegisteredCount => _registry.Count;

		internal static void Reset()
		{
			RegisteredProviders.Clear();
			_registry = new GeneratedConstructorRegistry();
		}
	}
}
