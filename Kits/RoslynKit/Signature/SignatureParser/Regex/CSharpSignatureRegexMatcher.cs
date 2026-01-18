using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature;

/// <summary>
/// Интерфейс для сущности, выполняющей регулярные выражения, специфичные для парсинга C# сигнатур.
/// </summary>
public interface ISignatureRegexMatcher
{
    /// <summary>
    /// Ищет первое вхождение namespace в строке.
    /// </summary>
    /// <param name="input">Входная строка сигнатуры.</param>
    /// <returns>Объект Match.</returns>
    Match MatchNamespace(string input);

    /// <summary>
    /// Пытается сопоставить полную сигнатуру (ResultType, FullName, Arguments).
    /// </summary>
    /// <param name="input">Входная строка, начинающаяся с namespace.</param>
    /// <returns>Объект Match.</returns>
    Match MatchFullSignature(string input);

    /// <summary>
    /// Пытается сопоставить Namespace + Class, используемый для обработки внешних вызовов.
    /// </summary>
    /// <param name="input">Входная строка сигнатуры.</param>
    /// <returns>Объект Match.</returns>
    Match MatchNamespaceAndClass(string input);

    Match SplitTypeAndClass(string input);

    Match MatchDelegateSignature(string input);
}

public partial class CSharpSignatureRegexMatcher : ISignatureRegexMatcher
{
    // Строки регулярных выражений (предполагаем, что они вынесены в CSharpSignatureRegex, как в предыдущем ответе,
    // или же они могут быть инкапсулированы здесь как приватные константы,
    // если не нужно их выносить в ресурсы).
    // Для чистоты, будем считать, что они по-прежнему берутся из CSharpSignatureRegex:

    [GeneratedRegex(CSharpSignatureRegex.SNamespaceClassMethodArgsRegexTemplate, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex FullSignatureRegex();

    [GeneratedRegex(CSharpSignatureRegex.NamespaceClassRegex, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex NamespaceClassOnlyRegex();

    [GeneratedRegex(CSharpSignatureRegex.NamespaceRegex)]
    private static partial Regex NamespaceOnlyRegex();

    [GeneratedRegex(CSharpSignatureRegex.FullTypeSplitRegex)]
    private static partial Regex TypeSplitRegex();

    [GeneratedRegex(CSharpSignatureRegex.DelegateSignatureRegex, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex DelegateSignatureOnlyRegex();

    public Match MatchDelegateSignature(string input)
    {
        return DelegateSignatureOnlyRegex().Match(input);
    }

    public Match MatchNamespace(string input)
    {
        return NamespaceOnlyRegex().Match(input);
    }

    public Match MatchFullSignature(string input)
    {
        return FullSignatureRegex().Match(input);
    }

    public Match MatchNamespaceAndClass(string input)
    {
        return NamespaceClassOnlyRegex().Match(input);
    }

    public Match SplitTypeAndClass(string input)
    {
        return TypeSplitRegex().Match(input);
    }
}
