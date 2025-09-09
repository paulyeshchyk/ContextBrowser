using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model.Options;

namespace RoslynKit.Extensions;

internal static class CSharpExpressionSyntaxExtensionConverter
{
    /// <summary>
    /// Извлекает имя метода и имя владельца (если есть) из ExpressionSyntax,
    /// работая только с синтаксическим деревом.
    /// </summary>
    /// <param name="invocationExpression">Синтаксический узел вызова метода.</param>
    /// <returns>Кортеж с именем namespace и полным именем владельца</returns>
    public static (string?, bool) ConvertToMethodRawSignature(this ExpressionSyntax initialExpression, SemanticOptions options)
    {
        var nameParts = new List<string>();
        bool isPartial = false;

        var expression = initialExpression;

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
            else if (expression is MemberBindingExpressionSyntax memberBindingExpressionSyntax)
            {
                nameParts.Add("BIND");
                nameParts.Add(expression.GetIdentifier());
                break;
            }
            else
            {
                nameParts.Add(expression.GetIdentifier());
                break; // Прерываем, если тип узла неизвестен.
            }
        }

        if (!nameParts.Any())
        {
            return default;
        }

        // Собираем имя в обратном порядке.
        nameParts.Reverse();

        string manualFullName = string.Join(".", nameParts);
        if (nameParts.Count == 1)
        {
            // предположим, что это функция(напр, nameof)
            return ($"{manualFullName}()", isPartial);
        }
        else
        {
            // В этом случае namespace и ownerName не могут быть надежно определены.
            string namespaceName = options.ExternalNamespaceName;
            var raw = $"{namespaceName}.{manualFullName}()";
            return (raw, isPartial);
        }
    }
}