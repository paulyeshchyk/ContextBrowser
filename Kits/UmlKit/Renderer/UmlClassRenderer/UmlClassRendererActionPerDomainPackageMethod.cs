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
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace UmlKit.Renderer;

public class UmlClassRendererActionPerDomainPackageMethod
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;

    public UmlClassRendererActionPerDomainPackageMethod(IAppOptionsStore optionsStore, INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder)
    {
        _optionsStore = optionsStore;
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
    }

    public Task<UmlRendererResult<UmlDiagramClass>> RenderAsync(List<ContextInfo> contextInfoList, CancellationToken cancellationToken)
    {
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        // Создаем новую диаграмму
        var diagram = new UmlDiagramClass(diagramBuilderOptions);

        int maxLength = UmlDiagramMaxNamelengthExtractor.Extract(contextInfoList,
        [
            UmlDiagramMaxNamelengthExtractorType.@method,
            UmlDiagramMaxNamelengthExtractorType.@namespace,
            UmlDiagramMaxNamelengthExtractorType.@entity,
            UmlDiagramMaxNamelengthExtractorType.property
        ]);

        var classesonly = contextInfoList.Where(item => item.ElementType.IsEntityDefinition()).ToList();
        var methsOnly = contextInfoList.Where(item => item.ElementType == ContextInfoElementType.@method).ToList();
        var propsOnly = contextInfoList.Where(item => item.ElementType == ContextInfoElementType.property).ToList();
        var propsOnlyClassOwners = contextInfoList.Where(item => item.ElementType == ContextInfoElementType.property).Select(i => i.ClassOwner).ToList();
        var methsOnlyClassOwners = contextInfoList.Where(item => item.ElementType == ContextInfoElementType.@method).Select(i => i.ClassOwner).ToList();

        var classes = classesonly.Concat(methsOnlyClassOwners)
                                 .Concat(propsOnlyClassOwners)
                                 .Where(c => c != null && c is ContextInfo)
                                 .Cast<ContextInfo>()
                                 .Where(c => !string.IsNullOrWhiteSpace(c.Namespace))
                                 .Distinct()
                                 .ToList();

        var namespaces = classes.Select(c => c.Namespace).Distinct();

        foreach (var ns in namespaces)
        {
            var umlPackage = PumlBuilderHelper.BuildUmlPackage(ns, _umlUrlBuilder);

            var classesInNamespace = classes.Where(c => c.Namespace == ns).Distinct().ToList();

            foreach (var contextInfo in classesInNamespace)
            {
                var propsList = propsOnly.Where(p => p.ClassOwner?.FullName.Equals(contextInfo.FullName) ?? false).Distinct();
                var methsList = methsOnly.Where(p => p.ClassOwner?.FullName.Equals(contextInfo.FullName) ?? false).Distinct();

                var umlClass = PumlBuilderHelper.BuildUmlEntityClass(contextInfo, methsList, propsList, maxLength, _namingProcessor);

                umlPackage.Add(umlClass);
            }

            diagram.Add(umlPackage);
            diagram.AddRelations(PumlBuilderSquaredLayout.Build(classesInNamespace.Select(cls => cls.FullName.AlphanumericOnly())));
        }

        diagram.AddRelations(PumlBuilderSquaredLayout.Build(namespaces.Select(ns => ns.AlphanumericOnly())));
        var result = new UmlRendererResult<UmlDiagramClass>(diagram, new UmlWriteOptions(alignMaxWidth: -1));
        return Task.FromResult(result);
    }
}