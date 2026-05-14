namespace EgorLin.DIsolated.Resolvers
{
	public class ResolverIsolated : IResolver
	{
		private readonly IResolver _resolver;

		public ResolverIsolated(IResolver resolver)
		{
			_resolver = resolver;
		}
		
		public object Resolve()
		{
			return _resolver.Resolve();
		}

		public void Dispose() { }
	}
}
