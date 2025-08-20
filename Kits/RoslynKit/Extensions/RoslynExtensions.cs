using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Extensions;

// context: roslyn, read
public static class RoslynExtensions
{
    // Вспомогательный метод для получения NamespaceDeclarationSyntax
    // Может быть полезен, если вам нужно только имя пространства имен, а не полное имя символа
    // context: roslyn, read
    public static string GetNamespaceDeclarationName(this MemberDeclarationSyntax memberDeclaration)
    {
        var namespaceDeclaration = memberDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        if (namespaceDeclaration == null)
        {
            // Если нет явного объявления пространства имен, это может быть в глобальном пространстве имен
            return string.Empty; // Или null, в зависимости от того, как вы хотите это представить
        }
        return namespaceDeclaration.Name.ToString();
    }
}