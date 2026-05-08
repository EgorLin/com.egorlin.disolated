using System;
using EgorLin.DIsolated.Core.Data.Types;

namespace EgorLin.DIsolated.Core.Extensions
{
	public static class ExtensionsDiContainer
	{
		public static void Bind<TConcrete, TContract>(this DiContainer container, TypeBinding typeBinding) where TConcrete : TContract
		{
			container.Bind(typeBinding, typeof(TConcrete), typeof(TContract));
		}
		
		public static void Bind<TConcrete, TContract1, TContract2>(this DiContainer container, TypeBinding typeBinding) where TConcrete : TContract1, TContract2
		{
			container.Bind(typeBinding, typeof(TConcrete), typeof(TContract1), typeof(TContract2));
		}
		
		public static void Bind<TConcrete, TContract1, TContract2, TContract3>(this DiContainer container, TypeBinding typeBinding) where TConcrete : TContract1, TContract2, TContract3
		{
			container.Bind(typeBinding, typeof(TConcrete), typeof(TContract1), typeof(TContract2), typeof(TContract3));
		}
		
		public static void Bind<TConcrete, TContract1, TContract2, TContract3, TContract4>(this DiContainer container, TypeBinding typeBinding) where TConcrete : TContract1, TContract2, TContract3, TContract4
		{
			container.Bind(typeBinding, typeof(TConcrete), typeof(TContract1), typeof(TContract2), typeof(TContract3), typeof(TContract4));
		}
		
		public static void BindSingleton(this DiContainer container, Type typeConcrete)
		{
			container.Bind(TypeBinding.Singleton, typeConcrete);
		}
		
		public static void BindSingleton(this DiContainer container, Type typeConcrete, params Type[] typeContracts)
		{
			container.Bind(TypeBinding.Singleton, typeConcrete, typeContracts);
		}

		public static void BindTransient(this DiContainer container, Type typeConcrete, Type typeContract)
		{
			container.Bind(TypeBinding.Transient, typeConcrete, typeContract);
		}

		public static void BindTransient(this DiContainer container, Type typeConcrete)
		{
			container.Bind(TypeBinding.Transient, typeConcrete);
		}
	}
}