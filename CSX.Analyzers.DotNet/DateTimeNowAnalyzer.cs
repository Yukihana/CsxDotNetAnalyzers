using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace CSX.Analyzers.DotNet;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DateTimeNowAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CsxDateTimeNowUsage";
    private const string Title = "Avoid using DateTime.Now";
    private const string MessageFormat = "Avoid 'DateTime.Now'. Use 'DateTime.UtcNow' instead.";
    private const string Description = "Using 'DateTime.Now' without timezone will cause sync issues and using with will add additional overheads. Use 'DateTime.UtcNow' instead.";
    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.SimpleMemberAccessExpression);
    }

    private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        var memberAccessExpression = (MemberAccessExpressionSyntax)context.Node;
        if (memberAccessExpression.Name.Identifier.Text == "Now" &&
            memberAccessExpression.Expression.ToString() == "DateTime")
        {
            var diagnostic = Diagnostic.Create(Rule, memberAccessExpression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}