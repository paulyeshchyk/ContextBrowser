using System;
using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using RoslynKit.Assembly;
using RoslynKit.Phases;
using RoslynKit.Phases.Syntax;
using SemanticKit.Model;


namespace ContextBrowser.Services.Parsing;

public class SemanticSyntaxRouterBuilderRegistry<TContext> : ISemanticSyntaxRouterBuilderRegistry<TContext>
where TContext : IContextWithReferences<TContext>
{
    private readonly IEnumerable<ISemanticSyntaxRouterBuilder<TContext>> _builders;

    // Внедряем ВСЮ коллекцию
    public SemanticSyntaxRouterBuilderRegistry(IEnumerable<ISemanticSyntaxRouterBuilder<TContext>> builders)
    {
        _builders = builders;
    }

    public ISemanticSyntaxRouterBuilder<TContext> GetRouterBuilder(string technology)
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