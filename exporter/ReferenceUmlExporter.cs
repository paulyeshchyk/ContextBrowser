using ContextBrowser.model;
using System.Text;

namespace ContextBrowser.ContextCommentsParser;

public static class ReferenceUmlExporter
{
    // context: build, uml, links
    public static void GenerateMethodLinks(List<ContextInfo> elements, string outputPath)
    {
        var methods = elements.Where(e => e.ElementType == "method").ToList();

        var sb = new StringBuilder();
        sb.AppendLine("@startuml");

        // Объявляем все методы как компоненты
        foreach(var method in methods)
        {
            var methodName = method.Name;
            sb.AppendLine($"component \"{methodName}\"");
        }

        sb.AppendLine();

        // Добавляем связи
        foreach(var method in methods)
        {
            foreach(var callee in method.References)
            {
                // Ищем целевой метод в списке
                var target = methods.FirstOrDefault(m => m.Name == callee);
                if(target != null)
                {
                    sb.AppendLine($"{method.Name} --> {target.Name}");
                }
            }
        }

        sb.AppendLine("@enduml");
        File.WriteAllText(outputPath, sb.ToString());
    }
}