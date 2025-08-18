using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.ContextKit;
using RoslynKit.Extensions;
using RoslynKit.Syntax.AssemblyLoader;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Semantic.Builder;

// context: csharp, build, contextInfo
public class CSharpMethodContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, MethodDeclarationSyntax, SemanticModel>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpMethodContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    protected override IContextInfo BuildContextInfoDto(TContext? ownerContext, MethodDeclarationSyntax syntax, SemanticModel model)
    {
        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, model, _onWriteLog, CancellationToken.None);

        var syntaxWrap = new CSharpSyntaxNodeWrapper(syntax);
        var symbolWrap = new CSharpISymbolWrapper(symbol);

        var nameSpace = syntax.GetNamespaceName();
        var spanStart = syntax.Span.Start;
        var spanEnd = syntax.Span.End;
        var identifier = syntax.Identifier.Text;

        string fullName;
        string name;
        if(symbol != null)
        {
            fullName = symbol.ToDisplayString();
            name = symbolWrap.GetName();
        }
        else
        {
            fullName = $"{nameSpace}.{identifier}";
            name = identifier;
        }
        var result = new ContextInfoDto(ContextInfoElementType.method, fullName, name, nameSpace, identifier, spanStart, spanEnd, classOwner: ownerContext, symbol: symbolWrap, syntaxNode: syntaxWrap);

        result.MethodOwner = result; // делаем себя же владельцем метода

        return result;
    }

    public TContext? BuildInvocationContextInfo(TContext? ownerContext, CSharpMethodSyntaxWrapper wrapper)
    {
        var contextInfoDto = wrapper.GetContextInfoDto();
        if(contextInfoDto == null)
        {
            return default;
        }

        contextInfoDto.ClassOwner = ownerContext;
        contextInfoDto.MethodOwner = contextInfoDto;// делаем себя же владельцем метода

        return BuildContextInfo(ownerContext, contextInfoDto);
    }
}