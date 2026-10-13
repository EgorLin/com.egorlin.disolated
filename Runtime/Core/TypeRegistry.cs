using System;
using System.Collections.Generic;

namespace EgorLin.DIsolated.Core
{
	internal static class TypeRegistry
	{
		private static readonly object Lock = new object();
		private static readonly Dictionary<Type, int> Ids = new Dictionary<Type, int>();
		private static int _nextId;

		internal static int GetId(Type type)
		{
			lock (Lock)
			{
				if (Ids.TryGetValue(type, out var id))
				{
					return id;
				}

				id = _nextId++;
				Ids.Add(type, id);
				return id;
			}
		}
	}

	internal static class TypeId<T>
	{
		public static readonly int Value = TypeRegistry.GetId(typeof(T));
	}
}
