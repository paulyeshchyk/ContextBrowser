using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace UmlKit.Renderer;

public class UmlClassRendererActionPerDomainClass
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;

    public UmlClassRendererActionPerDomainClass(IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
    }

    public async Task<UmlRendererResult<UmlDiagramClass>> RenderAsync(List<ContextInfo> contextInfoList, CancellationToken cancellationToken)
    {
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var diagram = new UmlDiagramClass(diagramBuilderOptions);

        // Группируем по Namespace, затем по ClassOwnerFullName
        var allElements = UmlClassDiagramDataMapper.Map(contextInfoList).ToList();

        int maxLength = UmlDiagramMaxNamelengthExtractor.Extract(allElements, new() { UmlDiagramMaxNamelengthExtractorType.@namespace, UmlDiagramMaxNamelengthExtractorType.@entity, UmlDiagramMaxNamelengthExtractorType.@method });

        var namespaces = allElements.GroupBy(e => e.Namespace).ToList();

        foreach (var nsGroup in namespaces)
        {
            var package = PumlBuilderHelper.BuildUmlPackage(nsGroup.Key, _umlUrlBuilder, maxLength);

            var classes = nsGroup.GroupBy(e => e.ClassName);

            foreach (var classGroup in classes)
            {
                foreach (var cls in classGroup)
                {
                    var umlClass = PumlBuilderHelper.BuildUmlEntityClass(cls.ContextInfo, classGroup, maxLength, _namingProcessor);
                    package.Add(umlClass);
                }
            }
            diagram.Add(package);
            diagram.AddRelations(package.Elements.OrderBy(e => e.Key).Select(e => e.Value));
        }
        diagram.AddRelations(PumlBuilderSquaredLayout.Build(namespaces.Select(g => g.Key.AlphanumericOnly())));

        var result = new UmlRendererResult<UmlDiagramClass>(diagram, new UmlWriteOptions(alignMaxWidth: maxLength));
        return await Task.FromResult(result);
    }
}

public static class UmlClassDiagramDataMapper
{
    public static IEnumerable<UmlClassDiagramElementDto> Map(List<ContextInfo> contextInfoList)
    {
        var result = contextInfoList.Select(m =>
        {
            string namespaceName;
            string className;

            if (m.ElementType != ContextInfoElementType.method && m.ElementType != ContextInfoElementType.@delegate)
            {
                namespaceName = m.Namespace;
                className = m.Name;
                return new UmlClassDiagramElementDto(namespaceName, className, m);
            }

            // Case 1: Method has a ClassOwner.
            if (!string.IsNullOrEmpty(m.ClassOwner?.FullName))
            {
                namespaceName = m.ClassOwner.Namespace;
                className = m.ClassOwner.Name;
                return new UmlClassDiagramElementDto(namespaceName, className, m);
            }

            // Case 2: Method has no ClassOwner. Parse from FullName.
            var parts = m.FullName.Split('.');
            if (parts.Length < 2)
            {
                // Handle edge case for malformed names
                namespaceName = "GLOBAL";
                className = "UnknownClass";
                return new UmlClassDiagramElementDto(namespaceName, className, m);
            }

            // The class name is the second to last part
            className = string.Join(".", parts.Take(parts.Length - 1));

            // The method name is the last part
            // For namespace, we need to extract from the class name.
            var classNameParts = className.Split('.');
            namespaceName = string.Join(".", classNameParts.Take(classNameParts.Length - 1));

            var theClassNameParts = className.Split('.');
            if (theClassNameParts == null || theClassNameParts.Length == 0)
            {
                return new UmlClassDiagramElementDto(namespaceName, "UnknownClass", m);
            }

            var theClassName = string.Join(".", theClassNameParts[^1]);
            return new UmlClassDiagramElementDto(namespaceName, theClassName, m);
        });
        return result;
    }
}
