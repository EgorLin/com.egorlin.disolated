using System;

namespace EgorLin.DIsolated.CodeGen
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public sealed class GenerateConstructorAttribute : Attribute
	{
	}
}
