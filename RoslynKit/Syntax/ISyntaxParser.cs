using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Syntax;

public interface ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    // Проверяет, может ли этот парсер обработать данный синтаксический узел.
    bool CanParse(MemberDeclarationSyntax syntax);

    // Выполняет парсинг синтаксического узла.
    void Parse(MemberDeclarationSyntax syntax, SemanticModel model);
}