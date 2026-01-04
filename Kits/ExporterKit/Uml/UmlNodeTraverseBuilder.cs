using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml;

public static class UmlNodeTraverseBuilder
{
    public static UmlNode BuildMindNode(ContextInfo startNode, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        // 1. Создание родительской ноды
        var ownerName = startNode.ClassOwner == null
            ? string.Format("{0}", startNode.ShortName)
            : string.Format("{0}.{1}", startNode.ClassOwner.ShortName, startNode.ShortName); //startNode.ClassOwner?.FullName ?? startNode.FullName;


        var resultNode = new UmlNode(ownerName, url: namingProcessor.ClassOnlyHtmlFilename(startNode.ClassOwner?.FullName))
        {
            Stylename = "grey"
        };

        // Проверка, есть ли дочерние ноды
        if (startNode.Domains.Count == 0)
        {
            // Если доменов нет, то нода остаётся одна, без дочерних элементов
            return resultNode;
        }

        UmlNode? firstChild = null;

        foreach (var domain in startNode.Domains)
        {
            var childNode = new UmlNode(domain, umlUrlBuilder.BuildDomainUrl(domain))
            {
                Stylename = "green"
            };
            resultNode.Children.Add(childNode);

            firstChild ??= childNode;

        }

        return resultNode;
    }
}