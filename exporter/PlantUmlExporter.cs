using ContextBrowser.Parser;
using System.Text;
using System.Text.RegularExpressions;

namespace ContextBrowser.exporter;

// context: uml, build
public static class PlantUmlExporter
{
    // context: build, uml
    public static void GenerateUml(List<ContextInfo> elements, string outputPath)
    {
        var classes = elements.Where(e => e.ElementType == "class").ToList();
        var methods = elements.Where(e => e.ElementType == "method").ToList();

        var grouped = classes.GroupBy(c => c.Namespace ?? "Global");

        var sb = new StringBuilder();
        sb.AppendLine("@startuml");

        foreach(var nsGroup in grouped)
        {
            sb.AppendLine($"package \"{nsGroup.Key}\" {{");

            foreach(var cls in nsGroup)
            {
                var className = CleanName(cls.Name);
                var classContexts = cls.Contexts.Count > 0
                    ? $"<<{string.Join(", ", cls.Contexts.OrderBy(c => c))}>>"
                    : $"<<NoContext>>";

                sb.AppendLine($"  component \"{className}\" {classContexts} {{");

                var methodsInClass = methods.Where(m => m.ClassOwner == cls.Name);
                foreach(var method in methodsInClass)
                {
                    var methodContexts = string.Join(", ", method.Contexts.Distinct().OrderBy(x => x));
                    var methodName = CleanName(method.Name);
                    if(!string.IsNullOrWhiteSpace(methodContexts))
                        sb.AppendLine($"    [{methodName}] <<{methodContexts}>>");
                }

                sb.AppendLine("  }");
            }

            sb.AppendLine("}");
        }

        sb.AppendLine("@enduml");
        File.WriteAllText(outputPath, sb.ToString());
    }

    // context: uml, build
    private static string? CleanName(string? rawName)
    {
        if(string.IsNullOrEmpty(rawName))
            return rawName;

        return Regex.Replace(rawName, @"<(.+?)>", "[$1]")
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
    }
}
