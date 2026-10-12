using System;
using System.Linq.Expressions;
using EgorLin.DIsolated.Injectors.Data.Info;

namespace EgorLin.DIsolated.Injectors.Builders
{
	public class BuilderConstructorMonoDefault : IBuilderConstructor
	{
		private readonly InfoFactoryConstructor _factory;

		public BuilderConstructorMonoDefault(Type type)
		{
			_factory = Expression.Lambda<InfoFactoryConstructor>(
				Expression.Convert(Expression.Default(type), typeof(object)),
				Expression.Parameter(typeof(object[]))).Compile();
		}

		public object Build(params object[] args) => _factory(args);
	}
}
