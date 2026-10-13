using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EgorLin.DIsolated.CodeGen
{
	[Generator(LanguageNames.CSharp)]
	public sealed class ConstructorGenerator : IIncrementalGenerator
	{
		private const string ContainerType = "EgorLin.DIsolated.Core.DiContainer";
		private const string ExtensionsType = "EgorLin.DIsolated.Core.Extensions.ExtensionsDiContainer";
		private const string HintsType = "EgorLin.DIsolated.CodeGen.GeneratedConstructors";
		private const string AttributeType = "EgorLin.DIsolated.CodeGen.GenerateConstructorAttribute";
		private const string ProviderType = "EgorLin.DIsolated.CodeGen.IGeneratedConstructorProvider";
		private const string RuntimeInitializeAttribute = "UnityEngine.RuntimeInitializeOnLoadMethodAttribute";
		private const string EditorInitializeAttribute = "UnityEditor.InitializeOnLoadMethodAttribute";

		private static readonly DiagnosticDescriptor DynamicBindingDescriptor = new DiagnosticDescriptor(
			"DISOLATED001",
			"Dynamically bound type cannot be code-generated",
			"The concrete type passed to a Bind method is not a typeof(...) expression, so no constructor can be generated for it. Annotate the type with [GenerateConstructor] or add GeneratedConstructors.Require<T>() / Require(typeof(T)).",
			"EgorLin.DIsolated.CodeGen",
			DiagnosticSeverity.Info,
			isEnabledByDefault: true);

		private static readonly HashSet<string> BindingMethods = new HashSet<string>(StringComparer.Ordinal)
		{
			"Bind", "BindSingleton", "BindTransient", "Resolve", "TryResolve"
		};

		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			var usage = context.SyntaxProvider
				.CreateSyntaxProvider(
					predicate: static (node, _) => IsCandidateInvocation(node),
					transform: static (syntaxContext, cancellationToken) => ExtractFromInvocation(syntaxContext, cancellationToken))
				.Where(static type => type != null);

			var attributed = context.SyntaxProvider
				.ForAttributeWithMetadataName(
					AttributeType,
					predicate: static (node, _) => node is TypeDeclarationSyntax,
					transform: static (attributeContext, _) => attributeContext.TargetSymbol as INamedTypeSymbol)
				.Where(static type => type != null);

			var dynamicBindings = context.SyntaxProvider
				.CreateSyntaxProvider(
					predicate: static (node, _) => IsCandidateInvocation(node),
					transform: static (syntaxContext, cancellationToken) => FindDynamicBinding(syntaxContext, cancellationToken))
				.Where(static location => location != null);

			var combined = usage.Collect()
				.Combine(attributed.Collect())
				.Combine(context.CompilationProvider);

			context.RegisterSourceOutput(combined, static (productionContext, source) => Emit(productionContext, source));
			context.RegisterSourceOutput(dynamicBindings, static (productionContext, location) =>
				productionContext.ReportDiagnostic(Diagnostic.Create(DynamicBindingDescriptor, location)));
		}

		private static Location FindDynamicBinding(GeneratorSyntaxContext context, CancellationToken cancellationToken)
		{
			var invocation = (InvocationExpressionSyntax)context.Node;
			var symbol = context.SemanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;

			if (symbol == null || !IsKnownMethod(symbol))
			{
				return null;
			}

			if (symbol.Name != "Bind" && symbol.Name != "BindSingleton" && symbol.Name != "BindTransient")
			{
				return null;
			}

			var parameters = symbol.Parameters;
			var arguments = invocation.ArgumentList.Arguments;

			for (var i = 0; i < parameters.Length && i < arguments.Count; i++)
			{
				if (parameters[i].Type.ToDisplayString() != "System.Type")
				{
					continue;
				}

				return arguments[i].Expression is TypeOfExpressionSyntax ? null : arguments[i].GetLocation();
			}

			return null;
		}

		private static bool IsCandidateInvocation(SyntaxNode node)
		{
			if (node is not InvocationExpressionSyntax invocation)
			{
				return false;
			}

			var name = GetInvokedName(invocation);
			return name != null && (BindingMethods.Contains(name) || name == "Require");
		}

		private static string GetInvokedName(InvocationExpressionSyntax invocation)
		{
			switch (invocation.Expression)
			{
				case MemberAccessExpressionSyntax member:
					return member.Name.Identifier.ValueText;
				case GenericNameSyntax generic:
					return generic.Identifier.ValueText;
				case IdentifierNameSyntax identifier:
					return identifier.Identifier.ValueText;
				default:
					return null;
			}
		}

		private static INamedTypeSymbol ExtractFromInvocation(GeneratorSyntaxContext context, CancellationToken cancellationToken)
		{
			var invocation = (InvocationExpressionSyntax)context.Node;
			var symbol = context.SemanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;

			if (symbol == null || !IsKnownMethod(symbol))
			{
				return null;
			}

			return ExtractConcreteType(symbol, invocation, context.SemanticModel, cancellationToken);
		}

		private static bool IsKnownMethod(IMethodSymbol symbol)
		{
			var containing = symbol.ContainingType?.ToDisplayString();

			if (containing == HintsType && symbol.Name == "Require")
			{
				return true;
			}

			return (containing == ContainerType || containing == ExtensionsType) && BindingMethods.Contains(symbol.Name);
		}

		private static INamedTypeSymbol ExtractConcreteType(IMethodSymbol symbol, InvocationExpressionSyntax invocation, SemanticModel model, CancellationToken cancellationToken)
		{
			switch (symbol.Name)
			{
				case "Bind":
					return symbol.IsGenericMethod
						? AsNamed(symbol.TypeArguments)
						: TypeOfArgument(invocation, 1, model, cancellationToken);
				case "BindSingleton":
				case "BindTransient":
				case "Resolve":
				case "TryResolve":
				case "Require":
					return symbol.IsGenericMethod
						? AsNamed(symbol.TypeArguments)
						: TypeOfArgument(invocation, 0, model, cancellationToken);
				default:
					return null;
			}
		}

		private static INamedTypeSymbol AsNamed(ImmutableArray<ITypeSymbol> typeArguments)
		{
			return typeArguments.Length == 0 ? null : typeArguments[0] as INamedTypeSymbol;
		}

		private static INamedTypeSymbol TypeOfArgument(InvocationExpressionSyntax invocation, int index, SemanticModel model, CancellationToken cancellationToken)
		{
			var arguments = invocation.ArgumentList.Arguments;

			if (arguments.Count <= index || arguments[index].Expression is not TypeOfExpressionSyntax typeOf)
			{
				return null;
			}

			return model.GetTypeInfo(typeOf.Type, cancellationToken).Type as INamedTypeSymbol;
		}

		private static void Emit(SourceProductionContext context, ((ImmutableArray<INamedTypeSymbol> Usage, ImmutableArray<INamedTypeSymbol> Attributed) Candidates, Compilation Compilation) source)
		{
			var compilation = source.Compilation;

			if (compilation.GetTypeByMetadataName(ContainerType) == null)
			{
				return;
			}

			if (compilation.GetTypeByMetadataName(ProviderType) == null)
			{
				return;
			}

			var seen = new HashSet<string>(StringComparer.Ordinal);
			var candidates = new List<INamedTypeSymbol>();

			foreach (var type in source.Candidates.Usage)
			{
				Consider(type, seen, candidates);
			}

			foreach (var type in source.Candidates.Attributed)
			{
				Consider(type, seen, candidates);
			}

			if (candidates.Count == 0)
			{
				return;
			}

			candidates.Sort(static (left, right) => string.CompareOrdinal(
				left.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
				right.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

			var builder = new FactoryBuilder(compilation);
			var registrations = new StringBuilder();
			var methods = new StringBuilder();
			var index = 0;

			foreach (var type in candidates)
			{
				if (!builder.TryCreate(type, index, out var method, out var parameterTypes))
				{
					continue;
				}

				var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
				registrations.Append("\t\t\tregistry.Register(typeof(").Append(typeName).Append("), Factory_").Append(index);

				if (parameterTypes.Length > 0)
				{
					registrations.Append(", new[] { ");

					for (var i = 0; i < parameterTypes.Length; i++)
					{
						if (i > 0)
						{
							registrations.Append(", ");
						}

						registrations.Append("typeof(").Append(parameterTypes[i]).Append(")");
					}

					registrations.Append(" }");
				}

				registrations.Append(");\n");
				methods.Append(method);
				index++;
			}

			if (index == 0)
			{
				return;
			}

			var className = "GeneratedConstructors_" + Sanitize(compilation.AssemblyName);
			var hasUnityEngine = compilation.GetTypeByMetadataName(RuntimeInitializeAttribute) != null;
			var hasUnityEditor = compilation.GetTypeByMetadataName(EditorInitializeAttribute) != null;
			var text = new StringBuilder();
			text.Append("// <auto-generated/>\n");
			text.Append("[assembly: global::EgorLin.DIsolated.CodeGen.GeneratedConstructorProviderAttribute(typeof(global::EgorLin.DIsolated.CodeGen.Generated.")
				.Append(className).Append("))]\n");
			text.Append("namespace EgorLin.DIsolated.CodeGen.Generated\n{\n");

			if (hasUnityEngine)
			{
				text.Append("\t[global::UnityEngine.Scripting.Preserve]\n");
			}

			text.Append("\tinternal sealed class ").Append(className).Append(" : global::EgorLin.DIsolated.CodeGen.IGeneratedConstructorProvider\n\t{\n");

			if (hasUnityEngine)
			{
				text.Append("\t\t[global::UnityEngine.RuntimeInitializeOnLoadMethod(global::UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]\n");

				if (hasUnityEditor)
				{
					text.Append("#if UNITY_EDITOR\n");
					text.Append("\t\t[global::UnityEditor.InitializeOnLoadMethod]\n");
					text.Append("#endif\n");
				}

				text.Append("\t\tprivate static void __DisolatedRegisterGeneratedConstructors()\n\t\t{\n");
				text.Append("\t\t\tglobal::EgorLin.DIsolated.CodeGen.GeneratedConstructors.Register(new ").Append(className).Append("());\n");
				text.Append("\t\t}\n\n");
			}

			text.Append("\t\tpublic void RegisterConstructors(global::EgorLin.DIsolated.CodeGen.GeneratedConstructorRegistry registry)\n\t\t{\n");
			text.Append(registrations);
			text.Append("\t\t}\n\n");
			text.Append(methods);
			text.Append("\t}\n}\n");

			context.AddSource(className + ".g.cs", text.ToString());
		}

		private static void Consider(INamedTypeSymbol type, HashSet<string> seen, List<INamedTypeSymbol> candidates)
		{
			if (type == null)
			{
				return;
			}

			var key = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

			if (seen.Add(key))
			{
				candidates.Add(type);
			}
		}

		private static string Sanitize(string name)
		{
			var builder = new StringBuilder(name.Length);

			foreach (var character in name)
			{
				builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
			}

			return builder.ToString();
		}
	}
}
