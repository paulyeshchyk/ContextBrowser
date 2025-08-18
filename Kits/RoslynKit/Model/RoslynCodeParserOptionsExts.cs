using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Model;

// context: csharp, build
internal static class RoslynCodeParserOptionsExts
{
    //context: csharp, read
    public static IEnumerable<MemberDeclarationSyntax> GetMemberDeclarationSyntaxies(this CompilationUnitSyntax root, RoslynCodeParserOptions options)
    {
        IEnumerable<MemberDeclarationSyntax> typeNodes = Enumerable.Empty<MemberDeclarationSyntax>();
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@class))
            typeNodes = typeNodes.Concat(FilterByModifier<ClassDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@record))
            typeNodes = typeNodes.Concat(FilterByModifier<RecordDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@struct))
            typeNodes = typeNodes.Concat(FilterByModifier<StructDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@enum))
            typeNodes = typeNodes.Concat(FilterByModifier<EnumDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@interface))
            typeNodes = typeNodes.Concat(FilterByModifier<InterfaceDeclarationSyntax>(root, options));
        if(options.MemberTypes.Contains(RoslynCodeParserMemberType.@delegate))
            typeNodes = typeNodes.Concat(FilterByModifier<DelegateDeclarationSyntax>(root, options));

        return typeNodes;
    }

    //context: csharp, read
    internal static RoslynCodeParserAccessorModifierType? GetClassModifierType<T>(T member)
        where T : MemberDeclarationSyntax
    {
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            return RoslynCodeParserAccessorModifierType.@public;
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            return RoslynCodeParserAccessorModifierType.@protected;
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            return RoslynCodeParserAccessorModifierType.@private;
        if(member.Modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            return RoslynCodeParserAccessorModifierType.@internal;

        return null;
    }

    //context: csharp, read
    internal static IEnumerable<T> FilterByModifier<T>(SyntaxNode root, RoslynCodeParserOptions options)
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