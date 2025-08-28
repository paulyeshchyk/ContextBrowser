using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public class CSharpISymbolWrapper : ISymbolInfo
{
    private readonly string _identifier;
    private readonly string _namespace;
    private readonly string _name;
    private readonly string _fullname;
    private readonly string _shortName;
    private object? _syntax;

    public CSharpISymbolWrapper(object? model, ISyntaxNodeWrapper syntaxWrap, OnWriteLog? onWriteLog, CancellationToken cancellationToken)
    {
        if (model is not ISemanticModelWrapper imodel)
        {
            throw new Exception($"model was not provided");
        }
        if (syntaxWrap.GetSyntax() is not MemberDeclarationSyntax syntax)
        {
            throw new Exception($"syntax not loaded");
        }
        var symbol = CSharpSymbolLoader.LoadSymbol(syntax, imodel, onWriteLog, cancellationToken);

        if (symbol is ISymbol isymbol)
        {
            _identifier = isymbol.GetFullMemberName(includeParams: true);
            _namespace = isymbol.GetNamespaceOrGlobal();
            _name = isymbol.GetNameAndClassOwnerName();
            _fullname = isymbol.GetFullMemberName(includeParams: true);
            _shortName = isymbol.GetShortName();
        }
        else
        {
            if (symbol is not null)
            {
                throw new Exception($"symbol is not isymbol ({symbol})");
            }

            _identifier = syntaxWrap.Identifier;
            _namespace = syntaxWrap.Namespace;
            _name = syntaxWrap.GetName();
            _fullname = syntaxWrap.GetFullName();
            _shortName = syntaxWrap.GetShortName();
        }
    }

    public string Identifier => _identifier;

    public string Namespace => _namespace;

    public string GetName() => _name;

    public string GetFullName() => _fullname;

    public string GetShortName() => _shortName;

    public object? GetSyntax() => _syntax;

    public void SetSyntax(object syntax)
    {
        _syntax = syntax;
    }
}
