using System;
using System.Reflection;
using EgorLin.DIsolated.Injectors.Data.Info;

namespace EgorLin.DIsolated.Injectors.Builders
{
	internal class BuilderConstructorIL2CPP : IBuilderConstructor
	{
		private readonly ConstructorInfo _constructor;

		public BuilderConstructorIL2CPP(Type type, ConstructorInfo constructor, InfoConstructorParameter[] parameters)
		{
			_constructor = constructor;
		}

		public object Build(params object[] args) => _constructor.Invoke(args);
	}
}
