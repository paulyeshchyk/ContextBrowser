using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Extensions;

internal static class InvocationExts
{
    /// <summary>
    /// Извлекает имя метода и имя владельца (если есть) из InvocationExpressionSyntax,
    /// работая только с синтаксическим деревом.
    /// </summary>
    /// <param name="invocationExpression">Синтаксический узел вызова метода.</param>
    /// <returns>Кортеж с именем метода и именем владельца. Имя владельца может быть null.</returns>
    public static CSharpInvocationSyntaxWrapper GetMethodInfoFromSyntax(this InvocationExpressionSyntax invocationExpression, ISymbol? symbol, RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
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
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Found new owner and methodname for invocation [{invocationExpression}]: [{ownerName}.{methodName}]");
        }
        else if(expression is IdentifierNameSyntax identifierName)
        {
            isPartial = true;
            methodName = identifierName.Identifier.Text;
            ownerName = options.FakeOwnerName;
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Composed new owner and methodname for invocation [{invocationExpression}]: [{ownerName}.{methodName}]");
        }
        else
        {
            isPartial = true;
            methodName = options.FakeMethodName;
            ownerName = options.FakeOwnerName;
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Invocation was not identified [{invocationExpression}]");
        }

        return new CSharpInvocationSyntaxWrapper(isPartial: isPartial, fullName: $"{ownerName}.{methodName}", shortName: methodName, spanStart: invocationExpression.Span.Start, spanEnd: invocationExpression.Span.End);
    }
}
