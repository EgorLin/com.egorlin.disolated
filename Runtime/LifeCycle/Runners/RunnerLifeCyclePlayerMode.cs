using UnityEngine;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Injectors.Providers;

namespace EgorLin.DIsolated.LifeCycle.Runners
{
	public static class RunnerLifeCyclePlayerMode
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		public static void Run()
		{
			RunnerLifeCycleDiContainer.RegisterPlayerLoop();
		}
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			RunnerLifeCycleDiContainer.ClearAll();
			ProviderInfoConstructorInjection.Clear();
			ExtensionsEnumerable.Clear();
		}
	}
}
