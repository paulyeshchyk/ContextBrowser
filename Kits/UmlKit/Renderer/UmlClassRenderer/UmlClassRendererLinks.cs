using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using UmlKit.Builders;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace UmlKit.Renderer;

public class UmlClassRendererLinks
{
    private readonly IAppOptionsStore _optionsStore;

    public UmlClassRendererLinks(IAppOptionsStore optionsStore) => _optionsStore = optionsStore;

    public Task<UmlRendererResult<UmlDiagramClass>> RenderAsync(HashSet<(string From, string To)> links, CancellationToken cancellationToken)
    {
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();
        var diagram = new UmlDiagramClass(diagramBuilderOptions);
        var relations = links.Select(e => PumlBuilderHelper.BuildUmlRelation(diagram, e.From, e.To, diagramBuilderOptions));
        diagram.AddRange(relations);

        var opts = new UmlWriteOptions(alignMaxWidth: -1);
        var result = new UmlRendererResult<UmlDiagramClass>(diagram, opts);

        return Task.FromResult(result);
    }
}
