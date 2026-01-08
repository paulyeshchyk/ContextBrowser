using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureParser;

public partial class CSharpSignatureParser : ISignatureParser<CSharpIdentifier>
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly IEnumerable<ISignatureParserRegex> _parsingAttempts;

    public CSharpSignatureParser(IAppOptionsStore optionsStore, SignatureChainFactory factory)
    {
        _optionsStore = optionsStore;

        // Получаем СВОЮ цепочку, используя идентификатор C#
        var chain = factory.GetChain<CSharpIdentifier>();

        _parsingAttempts = chain.Parsers;
    }

    public SignatureDefault Parse(string input)
    {
        foreach (var attempt in _parsingAttempts)
        {
            var result = attempt.Parse(input);
            if (result.Success)
            {
                return result.Result;
            }
        }

        // Эта точка кода теоретически не должна быть достигнута, если FinalFallbackParser всегда возвращает Success
        throw new InvalidOperationException("Цепочка парсинга завершилась без результата. Проверьте, что FinalFallbackParser присутствует.");
    }
}