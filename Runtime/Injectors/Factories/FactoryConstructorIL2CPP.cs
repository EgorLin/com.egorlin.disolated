using System;
using System.Reflection;
using EgorLin.DIsolated.Injectors.Builders;
using EgorLin.DIsolated.Injectors.Data.Info;

namespace EgorLin.DIsolated.Injectors.Factories
{
	internal class FactoryConstructorIL2CPP : IFactoryConstructor
	{
        public IBuilderConstructor CreateBuilder(Type type, ConstructorInfo constructor, InfoConstructorParameter[] parameters)
        {
	        return new BuilderConstructorIL2CPP(type, constructor);
        }

        public IBuilderConstructor CreateBuilderDefault(Type type)
        {
            return new BuilderConstructorIL2CPPDefault(type);
        }
	}
}
