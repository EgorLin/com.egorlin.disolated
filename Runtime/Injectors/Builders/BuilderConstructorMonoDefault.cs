using System;
using System.Linq.Expressions;
using EgorLin.DIsolated.Injectors.Data.Info;

namespace EgorLin.DIsolated.Injectors.Builders
{
	public class BuilderConstructorMonoDefault : IBuilderConstructor
	{
		private readonly Type _type;

		public BuilderConstructorMonoDefault(Type type)
		{
			_type = type;
		}
		
		public object Build(params object[] args)
		{
            var body = Expression.Convert(Expression.Default(_type), typeof(object));
            var lambda = Expression.Lambda<InfoFactoryConstructor>(body, Expression.Parameter(typeof(object[])));
            
            return lambda.Compile();
		}
	}
}
