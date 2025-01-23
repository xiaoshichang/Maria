using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Maria.Shared.Analyzer;

/// <summary>
/// A sample analyzer that reports the company name being used in class declarations.
/// Traverses through the Syntax Tree and checks the name (identifier) of each class node.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MAR00001Analyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "MAR00001";

	// The category of the diagnostic (Design, Naming etc.).
	private const string Category = "Naming";
	
	private const string Title = "Private member function naming";

	private const string MessageFormat = "Private member function should starts with underscore followed by Upper case alphabet. {0}.";

	private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, 
		Title, 
		MessageFormat, 
		Category,
		DiagnosticSeverity.Warning, 
		isEnabledByDefault: true);

	// Keep in mind: you have to list your rules here.
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

	public override void Initialize(AnalysisContext context)
	{
		// You must call this method to avoid analyzing generated code.
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

		// You must call this method to enable the Concurrent Execution.
		context.EnableConcurrentExecution();

		// Subscribe to the Syntax Node with the appropriate 'SyntaxKind' (ClassDeclaration) action.
		// To figure out which Syntax Nodes you should choose, consider installing the Roslyn syntax tree viewer plugin Rossynt: https://plugins.jetbrains.com/plugin/16902-rossynt/
		context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.MethodDeclaration);
	}


	private bool IsPrivateMethod( MethodDeclarationSyntax methodDeclarationNode)
	{
		foreach (var modifier in methodDeclarationNode.Modifiers)
		{
			if (modifier.IsKind(SyntaxKind.PrivateKeyword))
			{
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Executed for each Syntax Node with 'SyntaxKind' is 'ClassDeclaration'.
	/// </summary>
	/// <param name="context">Operation context.</param>
	private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
	{
		// The Roslyn architecture is based on inheritance.
		// To get the required metadata, we should match the 'Node' object to the particular type: 'ClassDeclarationSyntax'.
		if (context.Node is not MethodDeclarationSyntax methodDeclarationNode)
			return;

		if (!IsPrivateMethod(methodDeclarationNode))
		{
			return;
		}

		var methodName = methodDeclarationNode.Identifier.Text;
		if (methodName.StartsWith("_"))
		{
			return;
		}

		if (methodName.Length >= 2)
		{
			if (Char.IsUpper(methodName[1]))
			{
				return;
			}
		}
		
		var diagnostic = Diagnostic.Create(Rule,
			// The highlighted area in the analyzed source code. Keep it as specific as possible.
			methodDeclarationNode.Identifier.GetLocation(),
			// The value is passed to 'MessageFormat' argument of your 'Rule'.
			methodDeclarationNode.Identifier.Text
		);

		// Reporting a diagnostic is the primary outcome of the analyzer.
		context.ReportDiagnostic(diagnostic);

	}
}