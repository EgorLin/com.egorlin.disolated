using System;
using System.Collections.Generic;
using System.Linq;
using EgorLin.Collections.Unsafe;
using EgorLin.DIsolated.Core;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace EgorLin.DIsolated.LifeCycle.Runners
{
	public static class RunnerLifeCycleDiContainer
	{
		private static readonly FastList<DiContainer> _containersNew = new();
		private static readonly FastList<DiContainer> _containersRemove = new();
		private static readonly FastList<NodeDiContainer> _infoLifeCycles = new();
			
		public static void RegisterPlayerLoop()
		{
			var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			
			UpdateLoop(ref currentPlayerLoop);
			
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
		}

		public static void AddContainer(DiContainer diContainer)
		{
			_containersNew.Add(diContainer);
		}

		public static void RemoveContainer(DiContainer diContainer)
		{
			_containersRemove.Add(diContainer);
		}

		public static void ClearAll()
		{
			_containersNew.Clear();
			_containersRemove.Clear();
			_infoLifeCycles.Clear();
		}

		private static bool ContainsSystem(PlayerLoopSystem[] systems, Type type)
		{
			if (systems == null)
			{
				return false;
			}

			for (int i = 0; i < systems.Length; i++)
			{
				if (systems[i].type == type)
				{
					return true;
				}
			}

			return false;
		}

		private static void UpdateLoop(ref PlayerLoopSystem currentPlayerLoop)
		{
			for (int i = 0; i < currentPlayerLoop.subSystemList.Length; i++)
			{
				if (currentPlayerLoop.subSystemList[i].type == typeof(Update))
				{
					var updateSystem = currentPlayerLoop.subSystemList[i];
					
					if (ContainsSystem(updateSystem.subSystemList, typeof(RunnerLifeCycleDiContainer)))
					{
						return;
					}

					var list = updateSystem.subSystemList.ToList();

					list.Add(new PlayerLoopSystem
					{
						type = typeof(RunnerLifeCycleDiContainer),
						updateDelegate = Update
					});

					updateSystem.subSystemList = list.ToArray();
					currentPlayerLoop.subSystemList[i] = updateSystem;
					
					return;
				}
			}
		}

		private static void Update()
		{
			SetupContainers();

			RunTickables();

			RemoveContainers();
		}

		private static void SetupContainers()
		{
			foreach (var newContainer in _containersNew)
			{
				RunInitializables(newContainer);
				
				RunBootstraps(newContainer);

				AddInfo(newContainer);
			}

			_containersNew.Clear();
		}

		private static void RemoveContainers()
		{
			foreach (var diContainer in _containersRemove)
			{
				NodeDiContainer itemToRemove = null;
				var isFound = false;
				
				foreach (var infoLifeCycleDi in _infoLifeCycles)
				{
					if (infoLifeCycleDi.DiContainer == diContainer)
					{
						itemToRemove = infoLifeCycleDi;
						isFound = true;
						
						break;
					}
				}

				if (!isFound)
				{
					continue;
				}

				for (int index = itemToRemove.Disposables.Length - 1; index >= 0; index--)
				{
					var disposable = itemToRemove.Disposables[index];
					
					disposable.Dispose();
				}
					
				itemToRemove.DiContainer.Clear();
					
				_infoLifeCycles.Remove(itemToRemove);
			}

			_containersRemove.Clear();
		}

		private static void AddInfo(DiContainer newContainer)
		{
			var hasTickables = newContainer.TryResolve<IEnumerable<ITickable>>(out var tickables);
			var hasDisposables = newContainer.TryResolve<IEnumerable<IDisposable>>(out var disposables);

			var infoLifeCycleDi = new NodeDiContainer
			{
				DiContainer = newContainer,
				Tickables = hasTickables ? tickables.ToArray() : Array.Empty<ITickable>(),
				Disposables = hasDisposables ? disposables.ToArray() :  Array.Empty<IDisposable>()
			};
			
			_infoLifeCycles.Add(infoLifeCycleDi);
		}

		private static void RunInitializables(DiContainer newContainer)
		{
			var hasInitializables = newContainer.TryResolve<IEnumerable<IInitializable>>(out var initializables);

			if (hasInitializables)
			{
				foreach (var initializable in initializables)
				{
					initializable.Initialize();
				}
			}
		}

		private static void RunBootstraps(DiContainer newContainer)
		{
			var hasBootstrap = newContainer.TryResolve<IEnumerable<IBootstrap>>(out var bootstraps);

			if (hasBootstrap)
			{
				foreach (var bootstrap in bootstraps)
				{
					bootstrap.Boot();
				}
			}
		}

		private static void RunTickables()
		{
			foreach (var info in _infoLifeCycles)
			{
				foreach (var tickable in info.Tickables)
				{
					tickable.Tick();
				}
			}
		}

		private class NodeDiContainer
		{
			public DiContainer DiContainer;
			public ITickable[] Tickables;
			public IDisposable[] Disposables;
		}
	}
}