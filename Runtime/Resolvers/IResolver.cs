using System;

namespace EgorLin.DIsolated.Resolvers
{
	public interface IResolver : IDisposable
	{
		object Resolve();
	}
}
