using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Parser.Phases;

internal static class InvocationExts
{
    /// <summary>
    /// Извлекает имя метода и имя владельца (если есть) из InvocationExpressionSyntax,
    /// работая только с синтаксическим деревом.
    /// </summary>
    /// <param name="invocationExpression">Синтаксический узел вызова метода.</param>
    /// <returns>Кортеж с именем метода и именем владельца. Имя владельца может быть null.</returns>
    public static InvocationSyntaxWrapper GetMethodInfoFromSyntax(this InvocationExpressionSyntax invocationExpression, ISymbol? symbol, OnWriteLog? onWriteLog)
    {
        string methodName;
        string ownerName;
        bool isPartial;
        var expression = invocationExpression.Expression;

        if(expression is MemberAccessExpressionSyntax memberAccess)
        {
            isPartial = false;
            methodName = memberAccess.Name.Identifier.Text;
            ownerName = memberAccess.Expression.ToString();
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Found new owner and methodname for invocation [{invocationExpression}]: [{ownerName}.{methodName}]");
        }
        else if(expression is IdentifierNameSyntax identifierName)
        {
            isPartial = true;
            methodName = identifierName.Identifier.Text;
            ownerName = "__not_parsed__";
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Composed new owner and methodname for invocation [{invocationExpression}]: [{ownerName}.{methodName}]");
        }
        else
        {
            isPartial = true;
            methodName = "__not_parsed__";
            ownerName = "__not_parsed__";
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Invocation was not identified [{invocationExpression}]");
        }

        return new InvocationSyntaxWrapper(isPartial: isPartial, fullName: $"{ownerName}.{methodName}", shortName: methodName, spanStart: invocationExpression.Span.Start, spanEnd: invocationExpression.Span.End);
    }
}
