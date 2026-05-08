using System;
using System.Reflection;

namespace EgorLin.DIsolated.Injectors.Builders
{
	internal class BuilderConstructorIL2CPP : IBuilderConstructor
	{
		private readonly Type _type;
		private readonly ConstructorInfo _constructorInfo;

		public BuilderConstructorIL2CPP(Type type, ConstructorInfo constructorInfo)
		{
			_type = type;
			_constructorInfo = constructorInfo;
		}
		
		public object Build(params object[] args)
		{
			// TODO: this is a really slow operation. Replace on code generation
			var instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(_type);
			
			_constructorInfo.Invoke(instance, args);
			
			return instance;
		}
	}
}
