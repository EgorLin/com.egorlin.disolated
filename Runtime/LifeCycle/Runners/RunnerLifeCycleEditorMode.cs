#if UNITY_EDITOR
using UnityEditor;

namespace EgorLin.DIsolated.LifeCycle.Runners
{
	[InitializeOnLoad]
	public static class RunnerLifeCycleEditorMode
	{
		static RunnerLifeCycleEditorMode()
		{
			RunnerLifeCycleDiContainer.RegisterPlayerLoop();
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingPlayMode)
			{
				RunnerLifeCycleDiContainer.ClearAll();
			}
		}
	}
}
#endif