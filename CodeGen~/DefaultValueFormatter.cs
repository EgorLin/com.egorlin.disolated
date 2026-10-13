using System;
using System.Globalization;
using Microsoft.CodeAnalysis;

namespace EgorLin.DIsolated.CodeGen
{
	internal static class DefaultValueFormatter
	{
		public static bool TryFormat(object value, ITypeSymbol type, out string literal)
		{
			literal = null;

			if (value == null)
			{
				literal = type.IsValueType && type.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T
					? "default(" + Name(type) + ")"
					: "null";
				return true;
			}

			if (type.TypeKind == TypeKind.Enum && type is INamedTypeSymbol enumType && enumType.EnumUnderlyingType != null)
			{
				if (!TryFormat(value, enumType.EnumUnderlyingType, out var underlyingLiteral))
				{
					return false;
				}

				literal = "(" + Name(type) + ")(" + underlyingLiteral + ")";
				return true;
			}

			switch (value)
			{
				case bool boolean:
					literal = boolean ? "true" : "false";
					return true;
				case string text:
					literal = "\"" + Escape(text) + "\"";
					return true;
				case char character:
					literal = "'" + EscapeChar(character) + "'";
					return true;
				case float single:
					literal = FormatSingle(single);
					return true;
				case double number:
					literal = FormatDouble(number);
					return true;
				case decimal decimalValue:
					literal = decimalValue.ToString(CultureInfo.InvariantCulture) + "m";
					return true;
				case long longValue:
					literal = longValue.ToString(CultureInfo.InvariantCulture) + "L";
					return true;
				case ulong ulongValue:
					literal = ulongValue.ToString(CultureInfo.InvariantCulture) + "UL";
					return true;
				case uint uintValue:
					literal = uintValue.ToString(CultureInfo.InvariantCulture) + "U";
					return true;
				case int intValue:
					literal = intValue.ToString(CultureInfo.InvariantCulture);
					return true;
				case short shortValue:
					literal = shortValue.ToString(CultureInfo.InvariantCulture);
					return true;
				case ushort ushortValue:
					literal = ushortValue.ToString(CultureInfo.InvariantCulture);
					return true;
				case byte byteValue:
					literal = byteValue.ToString(CultureInfo.InvariantCulture);
					return true;
				case sbyte sbyteValue:
					literal = sbyteValue.ToString(CultureInfo.InvariantCulture);
					return true;
			}

			if (type.IsValueType)
			{
				literal = "default(" + Name(type) + ")";
				return true;
			}

			return false;
		}

		private static string Name(ITypeSymbol type)
		{
			return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		}

		private static string FormatSingle(float value)
		{
			if (float.IsNaN(value))
			{
				return "global::System.Single.NaN";
			}

			if (float.IsPositiveInfinity(value))
			{
				return "global::System.Single.PositiveInfinity";
			}

			if (float.IsNegativeInfinity(value))
			{
				return "global::System.Single.NegativeInfinity";
			}

			return value.ToString("R", CultureInfo.InvariantCulture) + "f";
		}

		private static string FormatDouble(double value)
		{
			if (double.IsNaN(value))
			{
				return "global::System.Double.NaN";
			}

			if (double.IsPositiveInfinity(value))
			{
				return "global::System.Double.PositiveInfinity";
			}

			if (double.IsNegativeInfinity(value))
			{
				return "global::System.Double.NegativeInfinity";
			}

			return value.ToString("R", CultureInfo.InvariantCulture) + "d";
		}

		private static string Escape(string value)
		{
			return value
				.Replace("\\", "\\\\")
				.Replace("\"", "\\\"")
				.Replace("\n", "\\n")
				.Replace("\r", "\\r")
				.Replace("\t", "\\t");
		}

		private static string EscapeChar(char value)
		{
			switch (value)
			{
				case '\\': return "\\\\";
				case '\'': return "\\'";
				case '\n': return "\\n";
				case '\r': return "\\r";
				case '\t': return "\\t";
				default: return value.ToString();
			}
		}
	}
}
