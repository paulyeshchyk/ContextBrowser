using System;
using System.Threading;
using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public static class CSharpISymbolWrapperConverter
{
    public static CSharpISymbolWrapper FromSymbolInfo(
        ISemanticModelWrapper semanticModel,
        ISyntaxNodeWrapper syntaxWrapper,
        OnWriteLog? onWriteLog,
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
        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, semanticModel, onWriteLog, cancellationToken);

        if (symbol is ISymbol isymbol)
        {
            wrapper.SetIdentifier(isymbol.GetFullMemberName(includeParams: true));
            wrapper.SetNamespace(isymbol.GetNamespaceOrGlobal());
            wrapper.SetName(isymbol.GetNameAndClassOwnerName());
            wrapper.SetFullName(isymbol.GetFullMemberName(includeParams: true));
            wrapper.SetShortName(isymbol.GetShortName());
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
