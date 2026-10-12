using System.Linq.Expressions;
using System.Reflection;
using EgorLin.DIsolated.Injectors.Data.Info;

namespace EgorLin.DIsolated.Injectors.Builders
{
	internal class BuilderConstructorMono : IBuilderConstructor
	{
		private readonly InfoFactoryConstructor _factory;

		public BuilderConstructorMono(ConstructorInfo constructor, InfoConstructorParameter[] parameters)
		{
			var param = Expression.Parameter(typeof(object[]));
			var argumentsExpressions = new Expression[parameters.Length];

			for (var i = 0; i < parameters.Length; i++)
			{
				var index = Expression.Constant(i);
				var parameterType = parameters[i].ParameterType;
				var parameterAccessor = Expression.ArrayIndex(param, index);
				var parameterCast = Expression.Convert(parameterAccessor, parameterType);
				argumentsExpressions[i] = parameterCast;
			}

			var newExpression = Expression.New(constructor, argumentsExpressions);
			_factory = Expression.Lambda<InfoFactoryConstructor>(
				Expression.Convert(newExpression, typeof(object)), param).Compile();
		}

		public object Build(object[] args) => _factory(args);
	}
}
