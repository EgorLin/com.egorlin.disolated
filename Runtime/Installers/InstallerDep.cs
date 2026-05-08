namespace EgorLin.DIsolated.Installers
{
	public abstract class InstallerDep<TDependency> : InstallerBase
	{
		public abstract void SetRefDependency(TDependency dependency);
	}
}
