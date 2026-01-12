using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Naming;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.Builders.TransitionFactory;
using UmlKit.DiagramGenerator.Managers;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Renderer;

public class UmlTransitionRendererFlatParticipant : UmlTransitionRendererFlat<UmlParticipant>
{
    public UmlTransitionRendererFlatParticipant(IAppLogger<AppLevel> logger, INamingProcessor namingProcessor, IAppOptionsStore optionsStore, IUmlTransitionFactory<UmlParticipant> factory)
    : base(logger, factory, namingProcessor, optionsStore)
    {
    }

    public override UmlDiagram<UmlParticipant> CreateDiagram(DiagramBuilderOptions options)
    {
        return new UmlDiagramSequence(options);
    }
}
