using System;
using System.Collections.Generic;
using System.Linq;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions;

public interface IDiagramCompileOptionsFactory
{
    IDiagramCompileOptions Create(DiagramKind kind, ILabeledValue meta);
}

public class DiagramCompileOptionsFactory : IDiagramCompileOptionsFactory
{
    private readonly IEnumerable<IDiagramCompileOptionsStrategy> _strategies;

    public DiagramCompileOptionsFactory(IEnumerable<IDiagramCompileOptionsStrategy> strategies)
    {
        _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
    }

    public IDiagramCompileOptions Create(DiagramKind kind, ILabeledValue meta)
    {
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(kind));
        if (strategy == null)
            throw new InvalidOperationException($"No compile options strategy registered for kind: {kind}");

        return strategy.Create(meta);
    }
}