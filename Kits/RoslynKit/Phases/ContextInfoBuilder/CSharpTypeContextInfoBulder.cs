using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Wrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpTypeContextInfoBulder<TContext> : BaseContextInfoBuilder<TContext, MemberDeclarationSyntax, ISemanticModelWrapper, CSharpTypeSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpTypeContextInfoBulder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, OnWriteLog? onWriteLog)
        : base(collector, factory, onWriteLog)
    {
    }

    public TContext? BuildContextInfo(TContext? ownerContext, CSharpTypeSyntaxWrapper wrapper)
    {
        var symInfo = wrapper.GetContextInfoDto();

        return BuildContextInfo(ownerContext, symInfo);
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@class;
}


public class CSharpTypeSyntaxNodeWrapper : CSharpSyntaxNodeWrapper<MemberDeclarationSyntax>, ISymbolInfo
{
    private MemberDeclarationSyntax _syntaxNode => GetCoSyntax<MemberDeclarationSyntax>();

    public override string Identifier => _syntaxNode.GetIdentifier();

    public override string Namespace => _syntaxNode.GetNamespaceOrGlobal();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetName() => _syntaxNode.GetIdentifier();

    public override string GetShortName() => GetName();
}