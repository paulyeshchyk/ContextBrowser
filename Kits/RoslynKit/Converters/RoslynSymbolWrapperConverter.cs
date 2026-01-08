using System;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Assembly;
using RoslynKit.AWrappers;
using RoslynKit.Signature.SignatureBuilder;
using RoslynKit.Wrappers;
using SemanticKit.Model;

namespace RoslynKit.Converters;

public class RoslynSymbolWrapperConverter : ISymbolWrapperConverter
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ISymbolLoader<MemberDeclarationSyntax, ISymbol> _symbolLoader;

    public RoslynSymbolWrapperConverter(IAppLogger<AppLevel> logger, ISymbolLoader<MemberDeclarationSyntax, ISymbol> symbolLoader)
    {
        _logger = logger;
        _symbolLoader = symbolLoader;
    }


    public async Task<ISymbolInfo> ConvertAsync(ISemanticModelWrapper semanticModel, ISyntaxNodeWrapper syntaxWrapper, CancellationToken cancellationToken)
    {
        if (semanticModel is null)
        {
            throw new ArgumentNullException(nameof(semanticModel), "Semantic model was not provided.");
        }

        if (syntaxWrapper.GetSyntax() is not MemberDeclarationSyntax syntax)
        {
            throw new InvalidOperationException("Syntax is not a MemberDeclarationSyntax.");
        }

        var symbol = await _symbolLoader.LoadSymbolAsync(syntax, semanticModel, cancellationToken).ConfigureAwait(false);

        var wrapper = new CSharpSymbolInfoWrapper();

#warning refactor this
        if (symbol == null)
        {
            wrapper.SetIdentifier(syntaxWrapper.Identifier);
            wrapper.SetNamespace(syntaxWrapper.Namespace);
            wrapper.SetName(syntaxWrapper.GetName());
            wrapper.SetFullName(syntaxWrapper.GetFullName());
            wrapper.SetShortName(syntaxWrapper.GetShortName());
        }
        else
        {
            wrapper.SetIdentifier(symbol.BuildFullMemberName());
            wrapper.SetNamespace(symbol.GetNamespaceOrGlobal());
            wrapper.SetName(symbol.BuildNameAndClassOwnerName());
            wrapper.SetFullName(symbol.BuildFullMemberName());
            wrapper.SetShortName(symbol.BuildShortName());
        }
        wrapper.SetSyntax(syntaxWrapper.GetSyntax());

        return wrapper;
    }
}
