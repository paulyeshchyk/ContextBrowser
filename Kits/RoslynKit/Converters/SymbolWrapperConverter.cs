using System;
using System.Threading;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Signature.SignatureBuilder;
using RoslynKit.Wrappers;
using SemanticKit.Model;

namespace RoslynKit.Converters;

public class SymbolWrapperConverter : ISymbolWrapperConverter
{
    private readonly IAppLogger<AppLevel> _logger;

    public SymbolWrapperConverter(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }


    public CSharpISymbolWrapper Convert(ISemanticModelWrapper semanticModel, ISyntaxNodeWrapper syntaxWrapper, CancellationToken cancellationToken)
    {
        if (semanticModel is null)
        {
            throw new ArgumentNullException(nameof(semanticModel), "Semantic model was not provided.");
        }

        if (syntaxWrapper.GetSyntax() is not MemberDeclarationSyntax syntax)
        {
            throw new InvalidOperationException("Syntax is not a MemberDeclarationSyntax.");
        }

        var wrapper = new CSharpISymbolWrapper();
        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, semanticModel, _logger, cancellationToken);

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
