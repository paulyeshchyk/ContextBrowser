using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.Wrappers;

namespace RoslynKit.Parser.Phases;

// context: csharp, build, contextInfo
public class MethodContextInfoBuilder<TContext>
        where TContext : IContextWithReferences<TContext>
{
    private IContextCollector<TContext> _collector;
    private IContextFactory<TContext> _factory;
    private OnWriteLog? _onWriteLog = null;

    public MethodContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
    {
        _collector = collector;
        _factory = factory;
        _onWriteLog = onWriteLog;
    }

    // context: csharp, build, contextInfo
    public List<(TContext context, MethodDeclarationSyntax syntax)> BuildContextInfoForMethods(SemanticModel semanticModel, string ns, TContext parent, IEnumerable<MethodDeclarationSyntax> methods)
    {
        var result = new List<(TContext, MethodDeclarationSyntax)>();
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Iterating methods [{parent.Name}]", LogLevelNode.Start);

        foreach(var method in methods)
        {
            var methodModel = MethodSyntaxExtractor.Extract(method, semanticModel);
            if(methodModel == null)
            {
                _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{parent.Name}]: Модель для метода не найдена [{method}]", LogLevelNode.Start);
                continue;
            }

            var context = BuildContextInfoForMethod(parent, methodModel, ns, method);
            if(context != null)
            {
                result.Add((context, method));
            }
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return result;
    }

    // context: csharp, build, contextInfo
    public TContext? BuildContextInfoForMethod(TContext? typeContext, MethodSyntaxWrapper methodmodel, string nsName, MethodDeclarationSyntax? resultSyntax = null)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Creating method ContextInfo: {methodmodel.MethodName}");

        var result = _factory.Create(
            typeContext,
            ContextInfoElementType.method,
            nsName,
            methodmodel.MethodName,
            methodmodel.MethodFullName,
            resultSyntax,
            methodmodel.SpanStart,
            methodmodel.SpanEnd,
            methodmodel.Symbol);

        if(result != null)
        {
            _collector.Add(result);
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Creating method ContextInfo: {result.Name}");
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Creating method ContextInfo failed {methodmodel.MethodName}");
        }


        return result;
    }
}

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