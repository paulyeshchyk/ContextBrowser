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

public class RoslynSyntaxCompiler : ISyntaxCompiler<MetadataReference>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynSyntaxCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    public CSharpCompilation CreateCompilation(SemanticOptions options, IEnumerable<ISyntaxTreeWrapper> syntaxTrees, string name, IEnumerable<MetadataReference> referencesToLoad)
    {
        var usings = GetValidatedUsingsFromOptions(options);
        var compilationOptions = new CSharpCompilationOptions
        (
            outputKind: OutputKind.DynamicallyLinkedLibrary,
            nullableContextOptions: NullableContextOptions.Enable,
            usings: usings
        );

        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, "Compilation loading", LogLevelNode.Start);
        var compilation = CSharpCompilation.Create(name, options: compilationOptions)
                                           .AddSyntaxTrees(syntaxTrees.Select(st => st.Tree).Cast<SyntaxTree>())
                                           .AddReferences(referencesToLoad);
        LogLoadedReferences(compilation);

        return compilation;
    }

    private void LogLoadedReferences(CSharpCompilation compilation)
    {
        var references = compilation.References;
        foreach (var reference in references)
        {
            _logger.WriteLog(AppLevel.R_Dll, LogLevel.Trace, $"Loaded reference: {reference.Display}");
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
