using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Route.Wrappers;
using RoslynKit.Route.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Route.Phases.ContextInfoBuilder;

public class CSharpInterfaceContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, InterfaceDeclarationSyntax, ISemanticModelWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpInterfaceContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    protected override IContextInfo BuildContextInfoDto(TContext? ownerContext, InterfaceDeclarationSyntax syntax, ISemanticModelWrapper model)
    {
        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, model, _onWriteLog, CancellationToken.None);

        var syntaxNodeWrap = new CSharpSyntaxNodeWrapper(syntax);
        var symbolWrap = new CSharpISymbolWrapper(symbol);

        var nameSpace = syntax.GetNamespaceName();
        var spanStart = syntax.Span.Start;
        var spanEnd = syntax.Span.End;
        var identifier = syntax.GetDeclarationName();

        string fullName;
        string name;
        if(symbol != null)
        {
            fullName = symbolWrap.ToDisplayString();
            name = symbolWrap.GetName();
        }
        else
        {
            fullName = $"{nameSpace}.{identifier}";
            name = identifier;
        }
        return new ContextInfoDto(ContextInfoElementType.@interface, fullName, name, nameSpace, identifier, spanStart, spanEnd, symbol: symbolWrap, syntaxNode: syntaxNodeWrap);
    }
}