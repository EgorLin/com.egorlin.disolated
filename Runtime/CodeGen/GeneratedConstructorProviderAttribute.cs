using System;

namespace EgorLin.DIsolated.CodeGen
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class GeneratedConstructorProviderAttribute : Attribute
	{
		public Type ProviderType { get; }

		public GeneratedConstructorProviderAttribute(Type providerType)
		{
			ProviderType = providerType;
		}
	}
}
