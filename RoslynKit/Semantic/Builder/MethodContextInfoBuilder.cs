using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Wrappers.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.Wrappers;

namespace RoslynKit.Semantic.Builder;

// context: csharp, build, contextInfo
public class MethodContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public MethodContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog) : base(collector, factory, onWriteLog)
    {
    }

    public List<(TContext context, MethodDeclarationSyntax syntax)> ParseMethodSyntax(IEnumerable<MethodDeclarationSyntax> methods, SemanticModel semanticModel, string ns, TContext parent)
    {
        var result = new List<(TContext, MethodDeclarationSyntax)>();
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Iterating methods [{parent.Name}]", LogLevelNode.Start);

        foreach(var method in methods)
        {
            var methodModel = MethodSyntaxExtractor.Extract(method, semanticModel);
            if(methodModel == null)
            {
                _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Warn, $"[{parent.Name}]: Модель для метода не найдена [{method}]", LogLevelNode.Start);
                continue;
            }

            var context = BuildContextInfoForMethod(parent, methodModel, ns, method);
            if(context != null)
            {
                result.Add((context, method));
            }
        }

        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return result;
    }

    // context: csharp, build, contextInfo
    public List<(TContext context, MethodDeclarationSyntax syntax)> BuildContextInfoForMethods(SemanticModel semanticModel, string ns, TContext parent, IEnumerable<MethodDeclarationSyntax> methods)
    {
        return ParseMethodSyntax(methods, semanticModel, ns, parent);
    }

    // context: csharp, build, contextInfo
    public TContext? BuildContextInfoForMethod(TContext? typeContext, MethodSyntaxWrapper methodmodel, string nsName, MethodDeclarationSyntax? resultSyntax = null)
    {
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Creating method ContextInfo: {methodmodel.MethodName}");

        var syntaxWrapper = new SyntaxNodeWrapper(resultSyntax);
        var symbolWrapper = new SymbolWrapper(methodmodel.Symbol);
        var result = _factory.Create(
            owner: typeContext,
            elementType: ContextInfoElementType.method,
            nsName: nsName,
            name: methodmodel.MethodFullName,
            fullName: methodmodel.MethodFullName,
            syntaxNode: syntaxWrapper,
            spanStart: methodmodel.SpanStart,
            spanEnd: methodmodel.SpanEnd,
            symbol: symbolWrapper);

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Creating method ContextInfo failed {methodmodel.MethodName}");
            return default;
        }

        _collector.Add(result);

        return result;
    }
}