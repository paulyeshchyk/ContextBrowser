using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using RoslynKit.Model.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, build
public class RoslynCompilationMapMapper : ICompilationMapMapper<RoslynSyntaxTreeWrapper>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynCompilationMapMapper(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }


    // context: roslyn, build, compilation
    public SemanticCompilationMap<RoslynSyntaxTreeWrapper> MapSemanticModelToCompilationMap(IEnumerable<RoslynSyntaxTreeWrapper> syntaxTrees, ICompilationWrapper compilation)
    {
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Making semantic models for {syntaxTrees.Count()} syntax tries", LogLevelNode.Start);

        var compiledMaps = syntaxTrees
            .Select(tree => MapSemanticModelToSingleMap(compilation, tree))
            .ToList();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, "Making semantic models done", LogLevelNode.End);

        return new SemanticCompilationMap<RoslynSyntaxTreeWrapper>(compiledMaps);
    }

    // context: roslyn, build, compilation
    internal CompilationMap<RoslynSyntaxTreeWrapper> MapSemanticModelToSingleMap(ICompilationWrapper compilation, RoslynSyntaxTreeWrapper tree)
    {
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Trace, $"Making semantic model for: {tree.FilePath}");
        var model = compilation.GetSemanticModel(tree);
        var compilationMap = new CompilationMap<RoslynSyntaxTreeWrapper>(tree, model);
        return compilationMap;
    }
}
