using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Parser.Phases;

public static class ClassLevelForeignInstanceScanner
{
    public static void MarkForeignInstanceCalls<T>(SemanticModel semanticModel, MemberDeclarationSyntax classNode, IContextCollector<T> collector)
        where T : IContextWithReferences<T>
    {
        return;
        /*

        var fieldInitializers = classNode.DescendantNodes().OfType<FieldDeclarationSyntax>();

        var createdInstances = new Dictionary<string, string>();

        foreach(var field in fieldInitializers)
        {
            foreach(var variable in field.Declaration.Variables)
            {
                if(variable.Initializer?.Value is ObjectCreationExpressionSyntax creation)
                {
                    var variableName = variable.Identifier.Text;
                    var typeSymbol = semanticModel.GetSymbolInfo(creation.Type).Symbol;

                    if(!string.IsNullOrEmpty(variableName) && typeSymbol != null)
                    {
                        createdInstances[variableName] = typeSymbol.ToDisplayString();
                    }
                }
            }
        }

        var invocations = classNode.DescendantNodes().OfType<InvocationExpressionSyntax>();

        foreach(var invocation in invocations)
        {
            if(invocation.Expression is MemberAccessExpressionSyntax access &&
                access.Expression is IdentifierNameSyntax idName)
            {
                var varName = idName.Identifier.Text;
                if(!createdInstances.TryGetValue(varName, out var targetClass))
                    continue;

                var symbol = semanticModel.GetSymbolInfo(access).Symbol;
                if(symbol == null)
                    continue;

                var context = collector.BySymbolDisplayName.GetValueOrDefault(symbol.ToDisplayString());
                if(context != null)
                {
                    context.IsForeignInstance = true;
                }
            }
        }
        */
    }
}