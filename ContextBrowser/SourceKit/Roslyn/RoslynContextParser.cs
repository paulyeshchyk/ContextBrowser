using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.extensions;
using ContextBrowser.Extensions;
using ContextBrowser.LoggerKit;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, parser, directory, contextInfo, build
public sealed class RoslynContextParser
{
    private const string TargetExtension = ".cs";

    // context: csharp, read, directory, contextInfo
    // layer: 900
    public static List<ContextInfo> Parse(string rootPath, RoslynCodeParserOptions options, OnWriteLog? onWriteLog = null)
    {
        var pathType = PathAnalyzer.GetPathType(rootPath);
        return pathType switch
        {
            PathAnalyzer.PathType.File => ParseFile(rootPath, options, onWriteLog),
            PathAnalyzer.PathType.Directory => ParseDirectory(rootPath, options, onWriteLog),
            _ => throw new NotImplementedException()
        };
    }

    // context: read, directory, csharp, contextInfo
    public static List<ContextInfo> ParseDirectory(string rootPath, RoslynCodeParserOptions options, OnWriteLog? onWriteLog = null)
    {
        var files = Directory.GetFiles(rootPath, $"*{TargetExtension}", SearchOption.AllDirectories);

        return ParseFilesList(options, onWriteLog, files);
    }

    // context: read, directory, csharp, contextInfo
    internal static List<ContextInfo> ParseFilesList(RoslynCodeParserOptions options, OnWriteLog? onWriteLog, string[] files)
    {
        var semanticModelStorage = new RoslynCodeSemanticTreeModelStorage();

        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Phase", LogLevelNode.Start);

        var contextCollector = new ContextInfoCollector<ContextInfo>();
        var modelBuilder = new RoslynCodeSemanticTreeModelBuilder(semanticModelStorage);

        Phase1(options, onWriteLog, files, semanticModelStorage, contextCollector, modelBuilder);

        var allContexts = contextCollector.Collection;

        Phase2(options, onWriteLog, files, semanticModelStorage, modelBuilder, allContexts);

        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return allContexts;
    }

    // context: build, directory, csharp, contextInfo
    internal static void Phase2(RoslynCodeParserOptions options, OnWriteLog? onWriteLog, string[] files, RoslynCodeSemanticTreeModelStorage semanticModelStorage, RoslynCodeSemanticTreeModelBuilder modelBuilder, List<ContextInfo> allContexts)
    {
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Phase 2", LogLevelNode.Start);

        // 2. Второй проход: строим ссылки на основе уже собранных
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(allContexts);
        var semanticInvocationResolver = new RoslynCodeSemanticInvocationResolver(semanticModelStorage);

        var referenceParser = new RoslynPhaseParserReferenceResolver<ContextInfo>(referenceCollector, modelBuilder, semanticInvocationResolver, onWriteLog);

        foreach(var file in files)
        {
            referenceParser.ParseFile(file, options);
        }
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    // context: build, directory, csharp, contextInfo
    internal static void Phase1(RoslynCodeParserOptions options, OnWriteLog? onWriteLog, string[] files, RoslynCodeSemanticTreeModelStorage semanticModelStorage, ContextInfoCollector<ContextInfo> contextCollector, RoslynCodeSemanticTreeModelBuilder modelBuilder)
    {
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Phase 1", LogLevelNode.Start);
        // 1. Первый проход: собираем все context'ы (типы и методы), без ссылок
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier(), onWriteLog);
        var factory = new ContextInfoFactory<ContextInfo>();
        var phaseParser = new RoslynPhaseParserContextBuilder<ContextInfo>(contextCollector, factory, processor, modelBuilder, onWriteLog);

        phaseParser.ParseFiles(files, options);
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    // context: read, directory, csharp, contextInfo
    internal static List<ContextInfo> ParseFile(string filePath, RoslynCodeParserOptions option, OnWriteLog? onWriteLog = null)
    {
        return ParseFilesList(option, onWriteLog, new[] { filePath });
    }
}