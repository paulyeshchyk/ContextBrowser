using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model.Options;

namespace RoslynKit.Extensions;

// context: roslyn, build
internal static class RoslynCodeParserOptionsExts
{
    //context: roslyn, read
    public static IEnumerable<MemberDeclarationSyntax> GetMemberDeclarationSyntaxies(this CompilationUnitSyntax root, SemanticOptions options)
    {
        IEnumerable<MemberDeclarationSyntax> typeNodes = Enumerable.Empty<MemberDeclarationSyntax>();
        if (options.MemberTypes.Contains(SemanticMemberType.@class))
            typeNodes = typeNodes.Concat(FilterByModifier<ClassDeclarationSyntax>(root, options));
        if (options.MemberTypes.Contains(SemanticMemberType.@record))
            typeNodes = typeNodes.Concat(FilterByModifier<RecordDeclarationSyntax>(root, options));
        if (options.MemberTypes.Contains(SemanticMemberType.@struct))
            typeNodes = typeNodes.Concat(FilterByModifier<StructDeclarationSyntax>(root, options));
        if (options.MemberTypes.Contains(SemanticMemberType.@enum))
            typeNodes = typeNodes.Concat(FilterByModifier<EnumDeclarationSyntax>(root, options));
        if (options.MemberTypes.Contains(SemanticMemberType.@interface))
            typeNodes = typeNodes.Concat(FilterByModifier<InterfaceDeclarationSyntax>(root, options));
        if (options.MemberTypes.Contains(SemanticMemberType.@delegate))
            typeNodes = typeNodes.Concat(FilterByModifier<DelegateDeclarationSyntax>(root, options));

        return typeNodes;
    }

    //context: roslyn, read
    internal static SemanticAccessorModifierType? GetClassModifierType<T>(T member)
        where T : MemberDeclarationSyntax
    {
        if (member.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return SemanticAccessorModifierType.@public;
        if (member.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return SemanticAccessorModifierType.@protected;
        if (member.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return SemanticAccessorModifierType.@private;
        if (member.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return SemanticAccessorModifierType.@internal;

        return null;
    }

    //context: roslyn, read
    internal static IEnumerable<T> FilterByModifier<T>(SyntaxNode root, SemanticOptions options)
        where T : MemberDeclarationSyntax
    {
        return root.DescendantNodes()
                   .OfType<T>()
                   .Where(node =>
                   {
                       var modifier = GetClassModifierType(node);
                       return modifier.HasValue && options.ClassModifierTypes.Contains(modifier.Value);
                   })
                   .OrderBy(n => n.SpanStart);
    }
}