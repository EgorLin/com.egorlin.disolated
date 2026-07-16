using System;
using System.Collections.Generic;

namespace Editor.Serialization.TypeSearch
{
    public interface ITypeCandiateProvider
    {
        IEnumerable<Type> GetTypeCandidates (Type baseType);
    }
}