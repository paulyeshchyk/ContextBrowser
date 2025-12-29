using System;
using System.Threading;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Model.SymbolWrapper;
using RoslynKit.Signature.SignatureBuilder;
using RoslynKit.Wrappers;
using SemanticKit.Model;

namespace RoslynKit.Converters;

public static class CSharpISymbolWrapperConverter
{
    public static CSharpISymbolWrapper FromSymbolInfo(
        ISemanticModelWrapper semanticModel,
        ISyntaxNodeWrapper syntaxWrapper,
        IAppLogger<AppLevel> logger,
        CancellationToken cancellationToken)
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
        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, semanticModel, logger, cancellationToken);

        if (symbol != null)
        {
            wrapper.SetIdentifier(symbol.BuildFullMemberName());
            wrapper.SetNamespace(symbol.GetNamespaceOrGlobal());
            wrapper.SetName(symbol.BuildNameAndClassOwnerName());
            wrapper.SetFullName(symbol.BuildFullMemberName());
            wrapper.SetShortName(symbol.BuildShortName());
        }
        else
        {
            if (symbol != null)
            {
                throw new InvalidOperationException($"Symbol is not an ISymbol ({symbol.GetType().Name}).");
            }

            wrapper.SetIdentifier(syntaxWrapper.Identifier);
            wrapper.SetNamespace(syntaxWrapper.Namespace);
            wrapper.SetName(syntaxWrapper.GetName());
            wrapper.SetFullName(syntaxWrapper.GetFullName());
            wrapper.SetShortName(syntaxWrapper.GetShortName());
        }
        wrapper.SetSyntax(syntaxWrapper.GetSyntax());
        return wrapper;
    }
}
