using ContextKit.ContextData.Naming;
using ContextKit.Model;
using UmlKit.Builders.Url;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml;

public static class UmlNodeTraverseBuilder
{
    public static UmlNode BuildMindNode(ContextInfo startNode, INamingProcessor namingProcessor)
    {
        // 1. Создание родительской ноды
        var ownerName = startNode.FullName;//startNode.ClassOwner?.FullName ?? startNode.FullName;
        var resultNode = new UmlNode(ownerName, alias: null, url: namingProcessor.ClassOnlyHtmlFilename(ownerName));
        resultNode.Stylename = "grey";

        // Проверка, есть ли дочерние ноды
        if (startNode.Domains.Count == 0)
        {
            // Если доменов нет, то нода остаётся одна, без дочерних элементов
            return resultNode;
        }

        UmlNode? firstChild = null;
        UmlNode? previousChild = null;

        foreach (var domain in startNode.Domains)
        {
            var childNode = new UmlNode(domain, alias: null, UmlUrlBuilder.BuildDomainUrl(domain));
            childNode.Stylename = "green";
            resultNode.Children.Add(childNode);

            if (firstChild == null)
            {
                firstChild = childNode;
            }

            previousChild = childNode;
        }

        return resultNode;
    }
}