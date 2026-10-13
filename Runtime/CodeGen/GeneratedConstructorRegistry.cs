using System;
using System.Collections.Generic;
using EgorLin.DIsolated.Core;

namespace EgorLin.DIsolated.CodeGen
{
	public sealed class GeneratedConstructorRegistry
	{
		private readonly Dictionary<int, Entry> _entries = new Dictionary<int, Entry>();

		public void Register(Type concrete, Func<DiContainer, object> factory, params Type[] parameters)
		{
			_entries[TypeRegistry.GetId(concrete)] = new Entry(factory, parameters ?? Type.EmptyTypes);
		}

		internal bool TryGet(Type concrete, out Entry entry)
		{
			return _entries.TryGetValue(TypeRegistry.GetId(concrete), out entry);
		}

		internal int Count => _entries.Count;

		internal sealed class Entry
		{
			internal readonly Func<DiContainer, object> Factory;
			internal readonly Type[] Parameters;

			internal Entry(Func<DiContainer, object> factory, Type[] parameters)
			{
				Factory = factory;
				Parameters = parameters;
			}
		}
	}
}
