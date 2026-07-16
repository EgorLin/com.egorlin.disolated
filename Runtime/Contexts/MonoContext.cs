using System.Collections.Generic;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Installers;
using EgorLin.DIsolated.Installers.Extensions;
using EgorLin.DIsolated.Serialization;
using UnityEngine;

namespace EgorLin.DIsolated.Contexts
{
    public abstract class MonoContext : MonoBehaviour
    {
        [SerializeReference] [SubclassSelector] private List<IDependency> dependencies = new();
        
		private void Awake()
		{
			var diContainer = new DiContainer();
			
			diContainer.BindInstallerDependencies(dependencies);
			
			Install(diContainer);
			
			diContainer.Build();
		}

		protected abstract void Install(DiContainer diContainer);
    }
}