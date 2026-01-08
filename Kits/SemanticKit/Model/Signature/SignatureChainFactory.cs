using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace SemanticKit.Model.Signature;

public class SignatureChainFactory
{
    private readonly Dictionary<Type, ISignatureParserChain> _chains;

    public SignatureChainFactory(IEnumerable<ISignatureParserChain> chains)
    {
        // Создаем словарь для быстрого поиска
        _chains = chains
            .GroupBy(c => c.IdentifierType)
            .ToDictionary(
                g => g.Key,         // Ключ: IdentifierType (typeof(CSharpIdentifier))
                g => g.First()      // Значение: Первая цепь, найденная для этого ключа
            );
    }

    public ISignatureParserChain<TIdentifier> GetChain<TIdentifier>()
        where TIdentifier : ISignatureTypeIdentifier
    {
        if (_chains.TryGetValue(typeof(TIdentifier), out var chain))
        {
            // Кастинг на обобщенный тип, который ожидает потребитель
            return (ISignatureParserChain<TIdentifier>)chain;
        }
        throw new InvalidOperationException($"Цепочка для типа '{typeof(TIdentifier).Name}' не найдена.");
    }
}