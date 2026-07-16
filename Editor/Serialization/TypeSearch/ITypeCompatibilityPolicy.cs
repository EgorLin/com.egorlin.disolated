using System;

namespace Editor.Serialization.TypeSearch
{
    public interface ITypeCompatibilityPolicy
    {
        bool IsCompatible (Type baseType, Type candiateType);
    }
}