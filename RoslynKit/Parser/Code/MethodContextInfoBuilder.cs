using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Parser.Extractor;

namespace RoslynKit.Parser.Phases;

// context: csharp, build, contextInfo
public class MethodContextInfoBuilder<TContext>
        where TContext : IContextWithReferences<TContext>
{
    private IContextCollector<TContext> _collector;
    private IContextFactory<TContext> _factory;
    private OnWriteLog? _onWriteLog = null;
    private Func<InvocationExpressionSyntax, SemanticModel?>? _symbolResolver;

    public MethodContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog, Func<InvocationExpressionSyntax, SemanticModel?>? symbolResolver)
    {
        _collector = collector;
        _factory = factory;
        _onWriteLog = onWriteLog;
        //TODO: tobe reused
        _symbolResolver = symbolResolver;
    }

    // context: csharp, build, contextInfo
    public IEnumerable<(TContext, MethodDeclarationSyntax)> BuildContextInfoForMethods(SemanticModel semanticModel, string nsName, TContext typeContext, IEnumerable<MethodDeclarationSyntax> methodDeclarationSyntaxies)
    {
        var result = new List<(TContext, MethodDeclarationSyntax)>();
        foreach(var methodDeclarationSyntax in methodDeclarationSyntaxies)
        {
            var methodmodel = MethodSyntaxExtractor.Extract(methodDeclarationSyntax, semanticModel);
            if(methodmodel == null)
            {
                _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{methodDeclarationSyntax}\" was not resolved in {nsName}");
                continue;
            }


            var item = BuildContextInfoForMethod(methodmodel, nsName, typeContext, methodDeclarationSyntax);
            if(item != null)
            {
                result.Add((item, methodDeclarationSyntax));
            }
        }
        return result;
    }

    // context: csharp, build, contextInfo
    public TContext? BuildContextInfoForMethod(MethodSyntaxModel methodmodel, string nsName, TContext typeContext, MethodDeclarationSyntax? resultSyntax = null)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"[{methodmodel.methodName}]:Creating method ContextInfo");

        var result = _factory.Create(typeContext, ContextInfoElementType.method, nsName, methodmodel.methodName, methodmodel.methodFullName, resultSyntax);
        _collector.Add(result);

        return result;
    }
}