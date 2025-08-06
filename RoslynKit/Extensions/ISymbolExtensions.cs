using Microsoft.CodeAnalysis;

namespace RoslynKit.Extensions;

public static class ISymbolExtensions
{
    public static string GetShortestName(this ISymbol symbol)
    {
        return symbol.Name;
        //return symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    }

    public static string GetNameOnly(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);
    }

    /// <summary>
    /// Возвращает полное имя члена, включая пространство имен.
    /// </summary>
    /// <param name="memberDeclaration">Объект MemberDeclarationSyntax.</param>
    /// <param name="semanticModel">Семантическая модель, к которой принадлежит синтаксическое дерево.</param>
    /// <returns>Полное имя члена или null, если символ не может быть разрешен.</returns>
    // context: csharp, read
    public static string GetFullMemberName(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }
}
