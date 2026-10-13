using System;
using EgorLin.DIsolated.CodeGen;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Injectors.Data.Info;
using EgorLin.DIsolated.Injectors.Exceptions;
using EgorLin.DIsolated.Injectors.Providers;
using EgorLin.DIsolated.Pools;

namespace EgorLin.DIsolated.Injectors.Factories
{
	public class FactoryConstructorDependencies
	{
		[ThreadStatic]
		private static SizeSpecificArrayPool<object> _arrayPool;
		internal static SizeSpecificArrayPool<object> ArrayPool => _arrayPool ??= new SizeSpecificArrayPool<object>(maxLength: 16);
		
		public static object Create(DiContainer container, Type concrete)
		{
            if (GeneratedConstructorDiscovery.TryGet(concrete, out var generated))
            {
	            try
	            {
		            return generated.Factory(container);
	            }
	            catch (Exception exception)
	            {
		            throw new ExceptionConstructorInjector(concrete, exception, GetGeneratedParameters(generated.Parameters));
	            }
            }
            
            var info = ProviderInfoConstructorInjection.Get(concrete);
            
            var parametersConstructor = info.ListInfoConstructorParameter;
            var lengthParametersConstructor = info.ListInfoConstructorParameter.Length;
            
            var arguments = ArrayPool.Rent(lengthParametersConstructor);
            
            try
            {
	            for (var i = 0; i < lengthParametersConstructor; i++)
	            {
		            var parameter = parametersConstructor[i];
		            
					if (!container.TryResolve(parameter.ParameterType, out var argument))
					{
						if (parameter.HasDefaultValue)
						{
							argument = parameter.DefaultValue;
						}
						else
						{
							throw new ExceptionUnknownContract(parameter.ParameterType);
						}
					}
					
					arguments[i] = argument;
	            }

	            return info.BuilderConstructor.Build(arguments);
            }
            catch (Exception exception)
            {
	            throw new ExceptionConstructorInjector(concrete, exception, parametersConstructor);
            }
            finally
            {
	            ArrayPool.Return(arguments);
            }
        }

        private static InfoConstructorParameter[] GetGeneratedParameters(Type[] parameters)
        {
	        if (parameters == null || parameters.Length == 0)
	        {
		        return Array.Empty<InfoConstructorParameter>();
	        }

	        var result = new InfoConstructorParameter[parameters.Length];

	        for (var i = 0; i < parameters.Length; i++)
	        {
		        result[i] = new InfoConstructorParameter(parameters[i], false, null);
	        }

	        return result;
        }
	}
}