using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.Builder;

public static class PumlBuilderMindNode
{
    public static UmlNode BuildMindNode(ContextInfo startNode, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        var resultNode = BuildNode(startNode, namingProcessor);

        // Проверка, есть ли дочерние ноды
        if (startNode.Domains.Count != 0)
        {
            BuildDomains(startNode, resultNode, umlUrlBuilder);
        }
        return resultNode;
    }

    private static UmlNode BuildNode(ContextInfo startNode, INamingProcessor namingProcessor)
    {
        // 1. Создание родительской ноды
        var ownerName = startNode.ClassOwner == null
            ? string.Format("{0}", startNode.ShortName)
            : string.Format("{0}.{1}", startNode.ClassOwner.ShortName, startNode.ShortName);


        var contextInfo = startNode.ClassOwner ?? startNode;
        var classNameWithNameSpace = $"{contextInfo.Namespace}.{contextInfo.ShortName}";
        var resultNode = new UmlNode(ownerName, url: namingProcessor.ClassOnlyHtmlFilename(classNameWithNameSpace))
        {
            Stylename = "grey"
        };
        return resultNode;
    }


    private static void BuildDomains(ContextInfo startNode, UmlNode resultNode, IUmlUrlBuilder umlUrlBuilder)
    {
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
    }
}