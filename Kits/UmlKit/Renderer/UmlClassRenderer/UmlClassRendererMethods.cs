using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace UmlKit.Renderer;

public class UmlClassRendererMethods
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;

    public UmlClassRendererMethods(IAppOptionsStore optionsStore, IContextInfoManager<ContextInfo> contextInfoManager)
    {
        _optionsStore = optionsStore;
        _contextInfoManager = contextInfoManager;
    }

    public async Task<UmlRendererResult<UmlDiagramClass>> RenderAsync(List<ContextInfo> methods, CancellationToken cancellationToken)
    {
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var diagram = new UmlDiagramClass(diagramBuilderOptions);

        int maxLength = UmlDiagramMaxNamelengthExtractor.Extract(methods, [UmlDiagramMaxNamelengthExtractorType.@method]);

        foreach (var method in methods)
        {
            var comp = PumlBuilderHelper.BuildUmlComponent(maxLength, method);
            diagram.Add(comp);
        }

        foreach (var method in methods)
        {
            var references = _contextInfoManager.GetReferencesSortedByInvocation(method);
            foreach (var callee in references)
            {
                if (methods.All(m => m.Name != callee.Name))
                    continue;

                var transitionState = PumlBuilderHelper.BuildUmlTransitionState(method, callee);
                diagram.Add(transitionState);
            }
        }
        var result = new UmlRendererResult<UmlDiagramClass>(diagram, new UmlWriteOptions(alignMaxWidth: -1));
        return result;
    }

}