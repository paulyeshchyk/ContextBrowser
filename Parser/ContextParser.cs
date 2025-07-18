using ContextBrowser.Extensions;

namespace ContextBrowser.Parser;

// context: parser, file, folder, csharp
public class ContextParser
{
    private const string TargetExtension = ".cs";

    // context: read
    public static List<ContextInfo> Parse(string rootPath)
    {
        var pathType = PathAnalyzer.GetPathType(rootPath);
        return pathType switch
        {
            PathAnalyzer.PathType.File => ParseFile(rootPath),
            PathAnalyzer.PathType.Directory => ParseDirectory(rootPath),
            PathAnalyzer.PathType.SymbolicLink => throw new NotImplementedException(),
            PathAnalyzer.PathType.NonExistentPath => throw new NotImplementedException(),
            PathAnalyzer.PathType.PlainText => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    // context: read, folder
    private static List<ContextInfo> ParseDirectory(string rootPath)
    {
        var results = new List<ContextInfo>();
        var files = Directory.GetFiles(rootPath, $"*{TargetExtension}", SearchOption.AllDirectories);

        foreach(var file in files)
        {
            results.AddRange(ParseFile(file));
        }

        return results;
    }

    // context: read, file
    private static List<ContextInfo> ParseFile(string filePath)
    {
        var result = new List<ContextInfo>();
        var lines = File.ReadAllLines(filePath);

        ContextInfo? currentClass = null;
        ContextInfo? currentMethod = null;
        string? currentNamespace = null;
        List<string> pendingContexts = new();

        foreach(var line in lines)
        {
            var nsMatch = line.MatchNamespace();
            if(nsMatch?.Success ?? false)
            {
                currentNamespace = nsMatch.Groups[1].Value.Trim().TrimEnd(';');
                continue;
            }

            var contextMatch = line.MatchContext();
            if(contextMatch?.Success ?? false)
            {
                var tags = contextMatch.Groups[1].Value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim().ToLower());

                foreach(var tag in tags)
                {
                    if(currentMethod != null)
                        currentMethod.Contexts.Add(tag);
                    else if(currentClass != null)
                        currentClass.Contexts.Add(tag);
                }
                pendingContexts.AddRange(tags);
                continue;
            }

            var classMatch = line.MatchClass();
            if(classMatch?.Success ?? false)
            {
                var theName = classMatch.Groups[1].Value.Trim();
                currentClass = new ContextInfo
                {
                    ElementType = "class",
                    Name = theName,//.Escaped()
                    Namespace = currentNamespace
                };

                foreach(var tag in pendingContexts.Distinct())
                    currentClass.Contexts.Add(tag);
                pendingContexts.Clear();

                result.Add(currentClass);
                currentMethod = null;
                continue;
            }

            var methodMatch = line?.MatchMethod();
            if(methodMatch?.Success ?? false)
            {
                var theName = methodMatch.Groups[1].Value.Trim();
                currentMethod = new ContextInfo
                {
                    ElementType = "method",
                    Name = theName,//.Escaped()
                    Namespace = currentNamespace,
                    ClassOwner = currentClass?.Name
                };

                foreach(var tag in pendingContexts.Distinct())
                    currentMethod.Contexts.Add(tag);
                pendingContexts.Clear();

                result.Add(currentMethod);
                continue;
            }
        }

        return result;
    }
}
