using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace SemanticKit.Model.Signature;

// В SemanticKit
/// <summary>
/// Базовый интерфейс-маркер для всех цепочек парсеров.
/// Используется в DI для сбора всех цепочек в одну коллекцию.
/// </summary>
public interface ISignatureParserChain
{
    // Обязательное свойство, которое позволит Фабрике получить TIdentifier
    Type IdentifierType { get; }
    IEnumerable<ISignatureParserRegex> Parsers { get; }
}

/// <summary>
/// Представляет специфическую цепочку обработчиков для парсинга сигнатур.
/// Например, одна реализация для C#, другая для Angular.
/// </summary>
public interface ISignatureParserChain<TIdentifier>
    where TIdentifier : ISignatureTypeIdentifier
{
    /// <summary>
    /// Коллекция специализированных парсеров (обработчиков) в порядке их вызова.
    /// </summary>
    IEnumerable<ISignatureParserRegex> Parsers { get; }
}