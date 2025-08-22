using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public record CSharpMethodSyntaxWrapper : ISyntaxWrapper
{
    public string Name { get; private set; }

    public string FullName { get; private set; }

    public string Namespace { get; private set; }

    public int SpanEnd { get; private set; }

    public int SpanStart { get; private set; }

    public CSharpMethodSyntaxWrapper(object symbol, MethodDeclarationSyntax syntax)
    {
        if (symbol is ISymbol isymbol)
        {
            Name = syntax.Identifier.Text;
            FullName = isymbol.GetFullMemberName();
            SpanStart = syntax.Span.Start;
            SpanEnd = syntax.Span.End;
            Namespace = syntax.GetNamespaceName();
        }
        else
        {
            throw new Exception("symbol is not isymbol");
        }
    }

    public CSharpMethodSyntaxWrapper(IInvocationSyntaxWrapper wrapper)
    {
        Name = wrapper.Name;
        FullName = wrapper.FullName;
        SpanStart = wrapper.SpanStart;
        SpanEnd = wrapper.SpanEnd;
        Namespace = wrapper.Namespace;
    }

    public IContextInfo GetContextInfoDto()
    {
        return new ContextInfoDto(ContextInfoElementType.method, FullName, Name, Namespace, $"{FullName}", SpanStart, SpanEnd);
    }
}
