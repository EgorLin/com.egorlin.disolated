using System;

namespace Editor.Serialization.TypeSearch
{
    public interface IIntrinsicTypePolicy
    {
        bool IsAllowed (Type candiateType);
    }
}