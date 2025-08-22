using Microsoft.CodeAnalysis;

namespace RoslynKit.Extensions;

public static class ISymbolExtensions
{
    public static string GetShortestName1(this ISymbol symbol)
    {
        var format = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
            localOptions: SymbolDisplayLocalOptions.None,
            parameterOptions: SymbolDisplayParameterOptions.None,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeExplicitInterface,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

        return symbol.ToDisplayString(format);
    }

    public static string GetNameSpace(this ISymbol symbol)
    {
        INamespaceSymbol containingNamespace = symbol.ContainingNamespace;
        return containingNamespace.ToDisplayString();
    }

    public static string GetShortestName(this ISymbol symbol)
    {
        // если это метод Invoke у делегата
        if (symbol is IMethodSymbol ms && ms.MethodKind == MethodKind.DelegateInvoke)
        {
            var delegateName = ms.ContainingType?.ToDisplayString(
                new SymbolDisplayFormat(
                    globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
                    memberOptions: SymbolDisplayMemberOptions.None,
                    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers));

            // можно вернуть только имя делегата (OnWriteLog)
            // return delegateName ?? ms.Name;

            // или делегат + Invoke (OnWriteLog.Invoke)
            return $"{delegateName}.{ms.Name}";
        }

        // стандартная обработка для всего остального
        var format = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            memberOptions: SymbolDisplayMemberOptions.None,
            propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
            localOptions: SymbolDisplayLocalOptions.None,
            parameterOptions: SymbolDisplayParameterOptions.None,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

        return symbol.ToDisplayString(format);
    }

    public static string GetMinimallyQualifiedName(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    }

    public static string GetFullMemberName(this ISymbol symbol)
    {
        if (symbol is IMethodSymbol ms)
        {
            var typeName = ms.ContainingType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
            return $"{typeName}.{ms.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}";
        }

        return symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }
}
