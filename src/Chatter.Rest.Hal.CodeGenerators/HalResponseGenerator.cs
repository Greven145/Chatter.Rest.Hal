using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Chatter.Rest.Hal.CodeGenerators;

[Generator]
public class HalResponseGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
//#if DEBUG
//		if (!System.Diagnostics.Debugger.IsAttached)
//		{
//			System.Diagnostics.Debugger.Launch();
//		}
//#endif
		IncrementalValuesProvider<ClassDeclarationSyntax?> halResponseTypes = context.SyntaxProvider
			.CreateSyntaxProvider(
				static (node, _) => Parser.IsSyntaxTargetForGeneration(node),
				static (ctx, _) => Parser.GetSemanticTargetForGeneration(ctx)
			)
			.Where(static type => type is not null);

		IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax?>)> compilationAndClasses =
			context.CompilationProvider.Combine(halResponseTypes.Collect());

		context.RegisterSourceOutput(compilationAndClasses,
			static (spc, source) => Execute(source.Item1, source.Item2, spc));
	}

	private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax?> classes,
		SourceProductionContext context)
	{
		if (classes.IsDefaultOrEmpty)
		{
			// nothing to do yet
			return;
		}

		IEnumerable<ClassDeclarationSyntax> distinctClasses = classes.NotNull().Distinct();

		var p = new Parser(compilation, context.ReportDiagnostic, context.CancellationToken);
		IReadOnlyList<Parser.HalResponseClass> logClasses = p.GetHalResponseClasses(distinctClasses);
		if (!logClasses.Any())
		{
			return;
		}

		var e = new Emitter();
		IReadOnlyList<(string, string)> results = e.Emit(logClasses, context.CancellationToken);

		foreach ((string fileName, string fileContents) in results)
		{
			context.AddSource(fileName, SourceText.From(fileContents, Encoding.UTF8));
		}
	}
}