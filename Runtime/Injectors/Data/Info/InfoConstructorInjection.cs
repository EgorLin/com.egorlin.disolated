using EgorLin.DIsolated.Injectors.Builders;

namespace EgorLin.DIsolated.Injectors.Data.Info
{
	internal class InfoConstructorInjection
	{
        public readonly IBuilderConstructor BuilderConstructor;
        public readonly InfoConstructorParameter[] ListInfoConstructorParameter;

        public InfoConstructorInjection(IBuilderConstructor builderConstructor, InfoConstructorParameter[] listInfoConstructorParameter)
        {
            BuilderConstructor = builderConstructor;
            ListInfoConstructorParameter = listInfoConstructorParameter;
        }
	}
}
