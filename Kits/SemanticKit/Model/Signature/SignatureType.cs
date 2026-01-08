using System;
using SemanticKit.Model.Signature;

namespace SemanticKit.Model.Signature;

// В SemanticKit
/// <summary>
/// Представляет уникальный идентификатор для конкретного синтаксического контекста или языка.
/// </summary>
public interface ISignatureTypeIdentifier
{
    // Строковый ключ, который будет использоваться для регистрации/поиска в DI.
    string Key { get; }

    // (Опционально) Человекочитаемое имя
    string DisplayName { get; }
}