namespace EgorLin.DIsolated.Resolvers
{
	public class ResolverSingletonInstance : IResolver
	{
		private object _instance;

		public ResolverSingletonInstance(object instance)
		{
			_instance = instance;
		}
		
		public object Resolve()
		{
			return _instance;
		}

		public void Dispose()
		{
			_instance = null;
		}
	}
}
