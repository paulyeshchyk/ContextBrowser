using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureParser;

public class CSharpSignatureParserChain : ISignatureParserChain<CSharpIdentifier>, ISignatureParserChain
{
    public Type IdentifierType => typeof(CSharpIdentifier);

    public IEnumerable<ISignatureParserRegex> Parsers { get; }

    public CSharpSignatureParserChain(ISignatureRegexMatcher regexMatcher, IAppOptionsStore optionsStore)
    {
        // Создаем и регистрируем последовательность попыток парсинга C#
        Parsers = new List<ISignatureParserRegex>
        {
            // 1. Стандартная сигнатура с явным Namespace (самая сложная)
            new CSharpSignatureParserStandardWithNamespace(regexMatcher, optionsStore),

            // 2. Стандартная сигнатура без явного Namespace
            new CSharpSignatureParserStandardWithoutNamespace(regexMatcher, optionsStore),

            // 3. Обработка сигнатур делегатов (DelegateParser)
            new CSharpSignatureParserDelegate(regexMatcher, optionsStore),

            // 4. Средний случай (Внешние вызовы)
            new CSharpSignatureParserExternal(regexMatcher, optionsStore),

            // 5. Простой случай (фейковые сигнатуры, catch-all)
            new CSharpSignatureParserFake(regexMatcher, optionsStore),

            // 6. Финальный Fallback (возвращает Error)
            new CSharpSignatureParserFinal()
        };
    }
}