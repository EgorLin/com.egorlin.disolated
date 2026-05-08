using UnityEngine;

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
		}
	}
}
