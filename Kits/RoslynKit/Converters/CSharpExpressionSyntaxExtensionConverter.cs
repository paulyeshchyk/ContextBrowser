using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax;
using SemanticKit.Model.Options;

namespace RoslynKit.Converters;

internal static class CSharpExpressionSyntaxExtensionConverter
{
    /// <summary>
    /// Извлекает имя метода и имя владельца (если есть) из ExpressionSyntax,
    /// работая только с синтаксическим деревом.
    /// </summary>
    /// <param name="initialExpression">Синтаксический узел вызова метода.</param>
    /// <param name="options"></param>
    /// <returns>Кортеж с именем namespace и полным именем владельца</returns>
    public static string? ConvertToMethodRawSignature(this ExpressionSyntax initialExpression, SemanticOptions options)
    {
        var nameParts = new List<string>();

        var expression = initialExpression;

        // Перебираем синтаксические узлы, чтобы собрать полное имя.
        while (true)
        {
            if (expression is MemberAccessExpressionSyntax memberAccess)
            {
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
            return null;
        }

        // Собираем имя в обратном порядке.
        nameParts.Reverse();

        string manualFullName = string.Join(".", nameParts);
        if (nameParts.Count == 1)
        {
            // предположим, что это функция(напр, nameof)
            return $"{manualFullName}()";
        }
        else
        {
            // В этом случае namespace и ownerName не могут быть надежно определены.
            string namespaceName = options.ExternalNaming.NamespaceName;
            var raw = $"{namespaceName}.{manualFullName}()";
            return raw;
        }
    }
}