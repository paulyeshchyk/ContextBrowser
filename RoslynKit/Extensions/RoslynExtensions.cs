using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Extensions;

// context: csharp, read
public static class RoslynExtensions
{
    public static string? GetShortestName(this ISymbol? symbol)
    {
        return symbol?.Name;
        //return symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    }

    public static string? GetNameOnly(this ISymbol? symbol)
    {
        return symbol?.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);
    }

    /// <summary>
    /// Возвращает полное имя члена, включая пространство имен.
    /// </summary>
    /// <param name="memberDeclaration">Объект MemberDeclarationSyntax.</param>
    /// <param name="semanticModel">Семантическая модель, к которой принадлежит синтаксическое дерево.</param>
    /// <returns>Полное имя члена или null, если символ не может быть разрешен.</returns>
    // context: csharp, read
    public static string? GetFullMemberName(this ISymbol? symbol)
    {
        return symbol?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }

    /// <summary>
    /// Пример получения полного имени для NamedTypeDeclarationSyntax (Class, Struct, Interface, Record, Enum).
    /// </summary>
    // context: csharp, read
    public static string? GetFullTypeName(this TypeDeclarationSyntax typeDeclaration, SemanticModel semanticModel)
    {
        if(typeDeclaration == null || semanticModel == null)
        {
            return null;
        }

        INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;

        if(typeSymbol == null)
        {
            return null;
        }

        // Получаем полное имя, включая пространство имен и вложенные типы
        return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    // Вспомогательный метод для получения NamespaceDeclarationSyntax
    // Может быть полезен, если вам нужно только имя пространства имен, а не полное имя символа
    // context: csharp, read
    public static string GetNamespaceDeclarationName(this MemberDeclarationSyntax memberDeclaration)
    {
        var namespaceDeclaration = memberDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        if(namespaceDeclaration == null)
        {
            // Если нет явного объявления пространства имен, это может быть в глобальном пространстве имен
            return string.Empty; // Или null, в зависимости от того, как вы хотите это представить
        }
        return namespaceDeclaration.Name.ToString();
    }
}