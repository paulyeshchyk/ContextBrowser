using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model.Options;

namespace RoslynKit.Extensions;

internal static class InvocationExpressionSyntaxExtensions
{
    /// <summary>
    /// Извлекает имя метода и имя владельца (если есть) из InvocationExpressionSyntax,
    /// работая только с синтаксическим деревом.
    /// </summary>
    /// <param name="invocationExpression">Синтаксический узел вызова метода.</param>
    /// <returns>Кортеж с именем метода и именем владельца. Имя владельца может быть null.</returns>
    public static CSharpInvocationSyntaxWrapper? GetMethodInfoFromSyntax(this InvocationExpressionSyntax invocationExpression, SemanticOptions options, OnWriteLog? onWriteLog)
    {
        var expression = invocationExpression.Expression;
        var nameParts = new List<string>();
        bool isPartial = false;

        // Перебираем синтаксические узлы, чтобы собрать полное имя.
        while (expression != null)
        {
            if (expression is MemberAccessExpressionSyntax memberAccess)
            {
                isPartial = true;
                nameParts.Add(memberAccess.Name.Identifier.Text);
                expression = memberAccess.Expression;
            }
            else if (expression is IdentifierNameSyntax identifierName)
            {
                nameParts.Add(identifierName.Identifier.Text);
                break; // Достигли самой верхней части имени.
            }
            else
            {
                break; // Прерываем, если тип узла неизвестен.
            }
        }

        if (!nameParts.Any())
        {
            onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Trace, $"Unknown invocation discovered\n {invocationExpression}");
            return default;
        }

        // Собираем имя в обратном порядке.
        nameParts.Reverse();
        string manualFullName = string.Join(".", nameParts);

        // В этом случае namespace и ownerName не могут быть надежно определены.
        string namespaceName = options.ExternalNamespaceName;

        return new CSharpInvocationSyntaxWrapper(
            isPartial: isPartial,
            fullName: $"{namespaceName}.{manualFullName}",
            shortName: manualFullName,
            spanStart: invocationExpression.Span.Start,
            spanEnd: invocationExpression.Span.End,
            nameSpace: namespaceName);
    }
}
