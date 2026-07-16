using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Serialization.TypeSearch.IntrinsicTypePolicy;
using UnityEditor;

namespace Editor.Serialization.TypeSearch.TypeCandiateProvider
{
    public sealed class DefaultTypeCandiateProvider : ITypeCandiateProvider
    {

        public static readonly DefaultTypeCandiateProvider Instance = new DefaultTypeCandiateProvider(DefaultIntrinsicTypePolicy.Instance);

        private readonly IIntrinsicTypePolicy intrinsicTypePolicy;

        public DefaultTypeCandiateProvider (IIntrinsicTypePolicy intrinsicTypePolicy)
        {
            this.intrinsicTypePolicy = intrinsicTypePolicy ?? throw new ArgumentNullException(nameof(intrinsicTypePolicy));
        }

        public IEnumerable<Type> GetTypeCandidates (Type baseType)
        {
            return TypeCache.GetTypesDerivedFrom(baseType)
                .Append(baseType)
                .Where(intrinsicTypePolicy.IsAllowed);
        }
    }
}