using System;
using System.Reflection;
using EgorLin.DIsolated.Injectors.Builders;
using EgorLin.DIsolated.Injectors.Data.Info;

namespace EgorLin.DIsolated.Injectors.Factories
{
	internal class FactoryConstructorMono : IFactoryConstructor
	{
        public IBuilderConstructor CreateBuilder(Type type, ConstructorInfo constructor, InfoConstructorParameter[] parameters)
        {
	        return new BuilderConstructorMono(constructor, parameters);
        }

        public IBuilderConstructor CreateBuilderDefault(Type type)
        {
	        return new BuilderConstructorMonoDefault(type);
        }
	}
}