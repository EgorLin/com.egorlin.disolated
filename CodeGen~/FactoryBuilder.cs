using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace EgorLin.DIsolated.CodeGen
{
	internal sealed class FactoryBuilder
	{
		private readonly Compilation _compilation;

		public FactoryBuilder(Compilation compilation)
		{
			_compilation = compilation;
		}

		public bool TryCreate(INamedTypeSymbol type, int index, out string method, out string[] parameterTypeNames)
		{
			method = null;
			parameterTypeNames = null;

			if (!IsCandidate(type))
			{
				return false;
			}

			if (!_compilation.IsSymbolAccessibleWithin(type, _compilation.Assembly))
			{
				return false;
			}

			var constructor = SelectConstructor(type);
			var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			var builder = new StringBuilder();
			var arguments = new List<string>();
			var parameters = new List<string>();

			builder.Append("\t\tprivate static object Factory_").Append(index)
				.Append("(global::EgorLin.DIsolated.Core.DiContainer __container)\n\t\t{\n");

			if (constructor == null)
			{
				builder.Append(type.IsValueType
					? "\t\t\treturn default(" + typeName + ");\n"
					: "\t\t\treturn new " + typeName + "();\n");
			}
			else
			{
				if (!_compilation.IsSymbolAccessibleWithin(constructor, _compilation.Assembly))
				{
					return false;
				}

				var constructorParameters = constructor.Parameters;

				for (var i = 0; i < constructorParameters.Length; i++)
				{
					var parameter = constructorParameters[i];

					if (parameter.RefKind != RefKind.None)
					{
						return false;
					}

					if (parameter.Type.TypeKind == TypeKind.Pointer || parameter.Type.IsRefLikeType)
					{
						return false;
					}

					var parameterTypeName = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
					var argumentName = "__arg" + i;
					parameters.Add(parameterTypeName);
					arguments.Add("(" + parameterTypeName + ")" + argumentName);

					builder.Append("\t\t\tif (!__container.TryResolve(typeof(").Append(parameterTypeName)
						.Append("), out var ").Append(argumentName).Append("))\n\t\t\t{\n");

					if (parameter.HasExplicitDefaultValue && DefaultValueFormatter.TryFormat(parameter.ExplicitDefaultValue, parameter.Type, out var literal))
					{
						builder.Append("\t\t\t\t").Append(argumentName).Append(" = ").Append(literal).Append(";\n");
					}
					else
					{
						builder.Append("\t\t\t\tthrow new global::EgorLin.DIsolated.Injectors.Exceptions.ExceptionUnknownContract(typeof(")
							.Append(parameterTypeName).Append("));\n");
					}

					builder.Append("\t\t\t}\n");
				}

				builder.Append("\t\t\treturn new ").Append(typeName).Append("(").Append(string.Join(", ", arguments)).Append(");\n");
			}

			builder.Append("\t\t}\n");
			method = builder.ToString();
			parameterTypeNames = parameters.ToArray();
			return true;
		}

		private static bool IsCandidate(INamedTypeSymbol type)
		{
			if (type == null)
			{
				return false;
			}

			if (type.TypeKind != TypeKind.Class && type.TypeKind != TypeKind.Struct)
			{
				return false;
			}

			if (type.IsAbstract || type.IsStatic || type.IsUnboundGenericType)
			{
				return false;
			}

			if (type.IsGenericType && type.TypeArguments.Any(argument => argument.TypeKind == TypeKind.TypeParameter))
			{
				return false;
			}

			if (type.IsRefLikeType || type.SpecialType != SpecialType.None)
			{
				return false;
			}

			return true;
		}

		private static IMethodSymbol SelectConstructor(INamedTypeSymbol type)
		{
			IMethodSymbol best = null;
			var bestCount = -1;

			foreach (var constructor in type.InstanceConstructors)
			{
				var count = constructor.Parameters.Length;

				if (count > bestCount)
				{
					bestCount = count;
					best = constructor;
				}
			}

			return best;
		}
	}
}
