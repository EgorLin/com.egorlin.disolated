namespace EgorLin.DIsolated.Injectors.Builders
{
	internal interface IBuilderConstructor
	{
		object Build(params object[] args);
	}
}
