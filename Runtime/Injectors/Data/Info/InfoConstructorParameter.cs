using System;

namespace EgorLin.DIsolated.Injectors.Data.Info
{
	internal struct InfoConstructorParameter
	{
        public readonly Type ParameterType;
        public readonly bool HasDefaultValue;
        public readonly object DefaultValue;
        
        public InfoConstructorParameter(Type parameterType, bool hasDefaultValue, object defaultValue) : this()
        {
            ParameterType = parameterType;
            HasDefaultValue = hasDefaultValue;
            DefaultValue = defaultValue;
        }
	}
}
