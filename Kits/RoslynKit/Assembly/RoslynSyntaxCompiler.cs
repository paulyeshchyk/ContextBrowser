using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model.Meta;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, build
public class RoslynSyntaxCompiler : ISyntaxCompiler<MetadataReference, RoslynSyntaxTreeWrapper, CSharpCompilation>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynSyntaxCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: roslyn, build
    public CSharpCompilation CreateCompilation(SemanticOptions options, IEnumerable<RoslynSyntaxTreeWrapper> syntaxTrees, string name, IEnumerable<MetadataReference> referencesToLoad)
    {
        var usings = GetValidatedUsingsFromOptions(options);
        var compilationOptions = new CSharpCompilationOptions
        (
            outputKind: OutputKind.DynamicallyLinkedLibrary,
            nullableContextOptions: NullableContextOptions.Enable,
            usings: usings
        );

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Creating compilation for {syntaxTrees.Count()} tries", LogLevelNode.Start);

        var compilation = CSharpCompilation.Create(name, options: compilationOptions)
                                           .AddSyntaxTrees(syntaxTrees.Select(st => st.Tree).Cast<SyntaxTree>())
                                           .AddReferences(referencesToLoad);
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, string.Empty, LogLevelNode.End);

        LogLoadedReferences(compilation);

        return compilation;
    }

    // context: roslyn, read
    internal void LogLoadedReferences(CSharpCompilation compilation)
    {
        var references = compilation.References;
        foreach (var reference in references)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Trace, $"Loaded reference: {reference.Display}");
        }
    }


    private static string[] GetValidatedUsingsFromOptions(SemanticOptions options)
    {
        string[] sDefaultUsing = ["System"];

        var result = string.IsNullOrWhiteSpace(options.GlobalUsings)
            ? sDefaultUsing
            : options.GlobalUsings.Split(";").Select(s => s.Trim()).ToArray();
        return (result.Length == 0)
            ? sDefaultUsing
            : result;
    }

}
