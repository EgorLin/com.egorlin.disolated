using System;
using System.Collections.Generic;
using System.Reflection;
using EgorLin.DIsolated.Injectors.Data.Info;
using EgorLin.DIsolated.Injectors.Data.Types;
using EgorLin.DIsolated.Injectors.Factories;

namespace EgorLin.DIsolated.Injectors.Providers
{
	internal static class ProviderInfoConstructorInjection
	{
        private static readonly Dictionary<IntPtr, InfoConstructorInjection> Dictionary = new();
        internal static readonly IFactoryConstructor Factory;

        static ProviderInfoConstructorInjection()
        {
            Factory = GetFactory();
        }

        internal static InfoConstructorInjection Get(Type type)
        {
	        var key = type.TypeHandle.Value;
	        
	        if (!Dictionary.TryGetValue(key, out var info))
	        {
		        info = Generate(type);
		        
		        Dictionary.Add(key, info);
	        }
        
	        return info;
        }

        private static IFactoryConstructor GetFactory()
        {
	        return TypeBackendScripting.Current switch
	        {
		        TypeBackendScripting.Backend.Mono => new FactoryConstructorMono(),
		        TypeBackendScripting.Backend.IL2CPP => new FactoryConstructorIL2CPP(),
		        _ => throw new Exception($"UnhandledRuntimeScriptingBackend {TypeBackendScripting.Current}"),
	        };
        }

        private static InfoConstructorInjection Generate(Type type)
        {
	        var constructors = type.GetConstructors();

	        if (constructors.Length == 0)
	        {
		        var infoDefaultConstructor = Factory.CreateBuilderDefault(type);
		        
		        return new InfoConstructorInjection(infoDefaultConstructor, Array.Empty<InfoConstructorParameter>());
	        }
	        
	        var constructor = GetConstructor(constructors);

			var parameters = constructor.GetParameters();
			int length = parameters.Length;

			var paramInfo = length == 0
				? Array.Empty<InfoConstructorParameter>()
				: new InfoConstructorParameter[length];

			for (int indexParameter = 0; indexParameter < length; indexParameter++)
			{
				var infoParameter = parameters[indexParameter];
				
				paramInfo[indexParameter] = new InfoConstructorParameter(
					infoParameter.ParameterType,
					infoParameter.HasDefaultValue,
					infoParameter.DefaultValue
				);
			}

			var infoConstructor = Factory.CreateBuilder(type, constructor, paramInfo);
				
			return new InfoConstructorInjection(infoConstructor, paramInfo);
        }

        private static ConstructorInfo GetConstructor(ConstructorInfo[] constructors)
        {
	        var bestIndex = 0;
	        int bestCount = constructors[0].GetParameters().Length;

	        for (int i = 1; i < constructors.Length; i++)
	        {
		        int count = constructors[i].GetParameters().Length;
		        
		        if (count > bestCount)
		        {
			        bestCount = count;
			        bestIndex = i;
		        }
	        }

	        return constructors[bestIndex];
        }
	}
}