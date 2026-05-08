using System.Linq.Expressions;
using System.Reflection;
using EgorLin.DIsolated.Injectors.Data.Info;

namespace EgorLin.DIsolated.Injectors.Builders
{
	internal class BuilderConstructorMono : IBuilderConstructor
	{
		private readonly ConstructorInfo _constructor;
		private readonly InfoConstructorParameter[] _parameters;

		public BuilderConstructorMono(ConstructorInfo constructor, InfoConstructorParameter[] parameters)
		{
			_constructor = constructor;
			_parameters = parameters;
		}
		
		public object Build(object[] args)
		{
			var param = Expression.Parameter(typeof(object[]));
			var argumentsExpressions = new Expression[_parameters.Length];

			for (var i = 0; i < _parameters.Length; i++)
			{
				var index = Expression.Constant(i);
				var parameterType = _parameters[i].ParameterType;
				var parameterAccessor = Expression.ArrayIndex(param, index);
				var parameterCast = Expression.Convert(parameterAccessor, parameterType);
				argumentsExpressions[i] = parameterCast;
			}

			var newExpression = Expression.New(_constructor, argumentsExpressions);
			var lambda = Expression.Lambda<InfoFactoryConstructor>(Expression.Convert(newExpression, typeof(object)), param);
            
			var delegateCompiled = lambda.Compile();

			var objectResult = delegateCompiled(args);
			
			return objectResult;
		}
	}
}