using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Syntax;

public interface ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    // ���������, ����� �� ���� ������ ���������� ������ �������������� ����.
    bool CanParse(MemberDeclarationSyntax syntax);

    // ��������� ������� ��������������� ����.
    void Parse(MemberDeclarationSyntax syntax, SemanticModel model);
}