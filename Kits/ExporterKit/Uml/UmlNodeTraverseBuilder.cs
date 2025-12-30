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
        string ownerName;
        if (startNode.ClassOwner == null)
        {
            ownerName = string.Format("{0}", startNode.ShortName);
        }
        else
        {
            ownerName = string.Format("{0}.{1}", startNode.ClassOwner.ShortName, startNode.ShortName);//startNode.ClassOwner?.FullName ?? startNode.FullName;
        }


        var resultNode = new UmlNode(ownerName, alias: null, url: namingProcessor.ClassOnlyHtmlFilename(startNode.ClassOwner?.FullName));
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