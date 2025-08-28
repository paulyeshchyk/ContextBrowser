using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public record CSharpMethodSyntaxWrapper : BaseSyntaxWrapper
{
    public string Name { get; set; }

    public string FullName { get; set; }

    public string Namespace { get; set; }

    public int SpanEnd { get; set; }

    public int SpanStart { get; set; }

    public string Identifier { get; set; }

    public bool IsPartial { get; set; }

    public string ShortName { get; set; }

    public bool IsValid { get; set; } = true;

    public CSharpMethodSyntaxWrapper(object symbol, MethodDeclarationSyntax syntax)
    {
        if (symbol is ISymbol isymbol)
        {
            IsPartial = false;
            ShortName = isymbol.GetShortName();
            Identifier = isymbol.GetFullMemberName(includeParams: true);
            Name = isymbol.GetNameAndClassOwnerName();
            FullName = isymbol.GetFullMemberName(includeParams: true);
            SpanStart = syntax.Span.Start;
            SpanEnd = syntax.Span.End;
            Namespace = isymbol.GetNamespaceOrGlobal();
        }
        else
        {
            throw new Exception("symbol is not isymbol");
        }
    }

    public CSharpMethodSyntaxWrapper(BaseSyntaxWrapper wrapper)
    {
        Identifier = wrapper.Identifier;
        Name = wrapper.Name;
        FullName = wrapper.FullName;
        SpanStart = wrapper.SpanStart;
        SpanEnd = wrapper.SpanEnd;
        Namespace = wrapper.Namespace;
        IsPartial = false;
        ShortName = wrapper.ShortName;
    }

    public IContextInfo GetContextInfoDto()
    {
        return new ContextInfoDto(
            elementType: ContextInfoElementType.method,
               fullName: FullName,
                   name: Name,
              shortName: ShortName,
              nameSpace: Namespace,
             identifier: FullName,
              spanStart: SpanStart,
                spanEnd: SpanEnd);
    }

    public string? ToDisplayString()
    {
        throw new NotImplementedException();
    }
}
