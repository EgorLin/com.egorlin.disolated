using System;
using System.Reflection;
using EgorLin.DIsolated.Injectors.Builders;
using EgorLin.DIsolated.Injectors.Data.Info;

namespace EgorLin.DIsolated.Injectors.Factories
{
	internal interface IFactoryConstructor
	{
		IBuilderConstructor CreateBuilder(Type type, ConstructorInfo constructor, InfoConstructorParameter[] parameters);
		IBuilderConstructor CreateBuilderDefault(Type type);
	}
}
