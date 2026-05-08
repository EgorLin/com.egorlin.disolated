using System;
using System.Collections.Generic;
using System.Linq;

namespace EgorLin.DIsolated.Core.Extensions
{
	internal static class ExtensionsType
	{
		internal static bool IsEnumerable(this Type type, out Type elementType)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				elementType = type.GetGenericArguments()[0];
				return true;
			}

			elementType = null;
			return false;
		}
		
        internal static string GetFullName(this Type type)
        {
            if (type.IsGenericType)
            {
                var outerTypeName = type.FullName!.Split('`').First();
                var innerTypeNames = string.Join(", ", type.GenericTypeArguments.Select(GetFullName));
                return $"{outerTypeName}<{innerTypeNames}>";
            }

            return type.FullName;
        }
	}
}
