using System;
using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using RoslynKit.Assembly;
using SemanticKit.Model;
using SemanticKit.Parsers.Syntax;

namespace ContextBrowser.Services.Parsing;

public class SemanticSyntaxRouterBuilderRegistry<TContext> : ISyntaxRouterBuilderRegistry<TContext>
where TContext : IContextWithReferences<TContext>
{
    private readonly IEnumerable<ISyntaxRouterBuilder<TContext>> _builders;

    // Внедряем ВСЮ коллекцию
    public SemanticSyntaxRouterBuilderRegistry(IEnumerable<ISyntaxRouterBuilder<TContext>> builders)
    {
        _builders = builders;
    }

    public ISyntaxRouterBuilder<TContext> GetRouterBuilder(string technology)
    {
        // Здесь потребуется логика, чтобы определить, какой именно билдер
        // является C# (например, через проверку типа или введение еще одного интерфейса)

        if (technology.Equals("csharp", StringComparison.OrdinalIgnoreCase))
        {
            return _builders.OfType<RoslynSemanticSyntaxRouterBuilder<TContext>>().First();
        }
        if (technology.Equals("angular", StringComparison.OrdinalIgnoreCase))
        {
            // return _builders.OfType<AngularPhaseParserDependenciesFactory<TContext>>().First();
        }

        throw new ArgumentException($"Builder for technology '{technology}' not found.");
    }
}