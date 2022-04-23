using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Chatter.Rest.Hal.CodeGenerators;

internal class Parser
{
	private const string HalResponse = "HalResponse";
	private const string HalResponseAttribute = "HalResponseAttribute";
	private const string HalResponseAttributeQualifiedName = "Chatter.Rest.Hal.HalResponseAttribute";
	private readonly CancellationToken _cancellationToken;
	private readonly Compilation _compilation;
	private readonly Action<Diagnostic> _reportDiagnostic;

	public Parser(Compilation compilation, Action<Diagnostic> reportDiagnostic, CancellationToken cancellationToken)
	{
		_compilation = compilation;
		_cancellationToken = cancellationToken;
		_reportDiagnostic = reportDiagnostic;
	}

	public IReadOnlyList<HalResponseClass> GetHalResponseClasses(IEnumerable<ClassDeclarationSyntax> classes)
	{
		var results = new List<HalResponseClass>();

		foreach (IGrouping<SyntaxTree, ClassDeclarationSyntax>? group in classes.GroupBy(x => x.SyntaxTree))
		{
			foreach (ClassDeclarationSyntax classDec in group)
			{
				_cancellationToken.ThrowIfCancellationRequested();
				var halResponseClass = new HalResponseClass
				{
					Namespace = GetNamespaceFrom(classDec),
					Name = classDec.Identifier.Text
				};

				foreach (MemberDeclarationSyntax member in classDec.Members)
				{
					if (member is not PropertyDeclarationSyntax property)
					{
						// we only care about properties
						continue;
					}

					switch (property.Identifier.Text)
					{
						case "Links":
							halResponseClass.HasLinks = true;
							break;
						case "Embedded":
							halResponseClass.HasEmbed = true;
							break;
					}
				}

				if (!halResponseClass.HasLinks || !halResponseClass.HasEmbed)
				{
					results.Add(halResponseClass);
				}
			}
		}

		return results;
	}

	internal static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
	{
		var attributeSyntax = (AttributeSyntax)context.Node;

		if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
		{
			return null;
		}

		string fullName = attributeSymbol.ContainingType.ToDisplayString();

		if (fullName != HalResponseAttributeQualifiedName)
		{
			return null;
		}

		return attributeSyntax.Parent?.Parent as ClassDeclarationSyntax;
	}

	internal static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
		node is AttributeSyntax attribute &&
		ExtractName(attribute.Name) is HalResponse or HalResponseAttribute;

	private static string? ExtractName(NameSyntax? name) =>
		name switch
		{
			IdentifierNameSyntax ins => ins.Identifier.Text,
			QualifiedNameSyntax qns => ExtractName(qns.Right),
			_ => null
		};

	private static string? GetNamespaceFrom(SyntaxNode s) =>
		s.Parent switch
		{
			FileScopedNamespaceDeclarationSyntax fileScopedNamespace => fileScopedNamespace.Name.ToString(),
			NamespaceDeclarationSyntax namespaceDeclarationSyntax => namespaceDeclarationSyntax.Name.ToString(),
			null => null,
			_ => GetNamespaceFrom(s.Parent)
		};

	internal class HalResponseClass
	{
		public bool HasEmbed;
		public bool HasLinks;
		public string Name = string.Empty;
		public string? Namespace = string.Empty;
	}
}