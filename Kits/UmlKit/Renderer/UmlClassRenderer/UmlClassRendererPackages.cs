using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextKit.ContextData;
using ContextKit.Model;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Builders.Url;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace UmlKit.Renderer;

public class UmlClassRendererPackages
{
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IUmlUrlBuilder _umlUrlBuilder;

    public UmlClassRendererPackages(IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IAppOptionsStore optionsStore, IUmlUrlBuilder umlUrlBuilder)
    {
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
        _umlUrlBuilder = umlUrlBuilder;
    }

    public Task<UmlRendererResult<UmlDiagramClass>> RenderAsync(List<ContextInfo> elements, CancellationToken cancellationToken)
    {

        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();
        var diagram = new UmlDiagramClass(diagramBuilderOptions);

        var classes = elements.Where(e => e.ElementType.IsEntityDefinition()).ToList();
        var methods = elements.Where(e => !e.ElementType.IsEntityDefinition()).ToList();

        var grouped = classes.GroupBy(c => c.Namespace);

        foreach (var nsGroup in grouped)
        {
            var namespaceName = nsGroup.Key;
            var package = PumlBuilderHelper.BuildUmlPackage(namespaceName, _umlUrlBuilder);

            foreach (var contextInfo in nsGroup)
            {
                PumlBuilderHelper.AddComponentGroup(methods, package, contextInfo);
            }

            diagram.Add(package);
            diagram.AddRelations(PumlBuilderSquaredLayout.Build(methods.Select(m => m.FullName)));
        }

        return Task.FromResult(new UmlRendererResult<UmlDiagramClass>(diagram, new UmlWriteOptions(alignMaxWidth: -1)));
    }
}
