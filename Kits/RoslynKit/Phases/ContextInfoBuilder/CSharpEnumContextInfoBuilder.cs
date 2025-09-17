using System.Diagnostics.CodeAnalysis;
using ContextBrowserKit.Log;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Wrappers;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class CSharpEnumContextInfoBuilder<TContext> : BaseContextInfoBuilder<TContext, EnumDeclarationSyntax, ISemanticModelWrapper, CSharpEnumSyntaxNodeWrapper>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpEnumContextInfoBuilder(IContextCollector<TContext> collector, IContextFactory<TContext> factory, IAppLogger<AppLevel> logger)
        : base(collector, factory, logger)
    {
    }

    public override ContextInfoElementType ElementType => ContextInfoElementType.@enum;
}

public class CSharpEnumSyntaxNodeWrapper : CSharpSyntaxNodeWrapper<EnumDeclarationSyntax>
{
    private MemberDeclarationSyntax _syntaxNode => GetCoSyntax<EnumDeclarationSyntax>();

    public override string Identifier => _syntaxNode.GetIdentifier();

    public override string Namespace => _syntaxNode.GetNamespaceOrGlobal();

    public override string GetName() => _syntaxNode.GetIdentifier();

    public override string GetFullName() => $"{Namespace}.{GetName()}";

    public override string GetShortName() => _syntaxNode.GetIdentifier();
}