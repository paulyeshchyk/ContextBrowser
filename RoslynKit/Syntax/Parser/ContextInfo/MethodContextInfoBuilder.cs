using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.Wrappers;

namespace RoslynKit.Syntax.Parser.ContextInfo;

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

        if(result == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Creating method ContextInfo failed {methodmodel.MethodName}");
            return default;
        }

        _collector.Add(result);

        return result;
    }
}