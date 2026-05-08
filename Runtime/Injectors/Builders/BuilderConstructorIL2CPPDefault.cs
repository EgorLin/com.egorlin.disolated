using System;

namespace EgorLin.DIsolated.Injectors.Builders
{
	internal class BuilderConstructorIL2CPPDefault : IBuilderConstructor
	{
		private readonly Type _type;

		public BuilderConstructorIL2CPPDefault(Type type)
		{
			_type = type;
		}
		
		public object Build(params object[] args)
		{
			return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(_type);
		}
	}
}
