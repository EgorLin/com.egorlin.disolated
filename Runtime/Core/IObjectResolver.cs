using System;

namespace EgorLin.DIsolated.Core
{
	public interface IObjectResolver
	{
		T Resolve<T>();
		object Resolve(Type type);
	}
}
