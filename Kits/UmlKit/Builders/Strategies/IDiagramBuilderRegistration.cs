using System;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.Strategies;

public interface IDiagramBuilderRegistration<TKey> : IDiagramBuilderRegistration
    where TKey : IDiagramBuilderKey
{
}

public interface IDiagramBuilderRegistration
{
    // Ключ, по которому будет храниться билдер (например, "Transition", "MethodFlow")
    string Key { get; } // Нужен для ToDictionary, т.к. DI не собирает обобщения

    Type KeyType { get; }

    // Фабричная функция, которая создаст конкретный билдер.
    // Она должна принимать только те аргументы, которые могут меняться (options),
    // и возвращать созданный IContextDiagramBuilder.
    // Все остальные общие зависимости должны быть внедрены в саму эту фабричную функцию через DI!
    IContextDiagramBuilder CreateBuilder(DiagramBuilderOptions options);
}
