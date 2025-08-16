using ContextBrowserKit.Log;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace UmlKit.Renderer.Sequence;

public class SequenceTransitionProcessor<T>
        where T : IUmlParticipant
{
    private readonly DiagramBuilderOptions _options;
    private readonly OnWriteLog? _onWriteLog;
    private readonly IUmlTransitionFactory<T> _factory;

    public SequenceTransitionProcessor(DiagramBuilderOptions options, OnWriteLog? onWriteLog, IUmlTransitionFactory<T> factory)
    {
        _options = options;
        _onWriteLog = onWriteLog;
        _factory = factory;
    }

    public void RenderSingleTransition(RenderContext<T> ctx)
    {
        SequenceRendererUtils.AddParticipants(ctx, UmlParticipantKeyword.Actor);

        _ = SequenceRendererUtils.RenderActivateCaller(ctx);

        _ = SequenceRendererUtils.RenderActivateCallee(ctx);

        _ = SequenceRendererUtils.RenderActivateCalleeInvocation(ctx);

        _ = SequenceRendererUtils.RenderDeactivateCalleeInvocation(ctx);

        _ = SequenceRendererUtils.RenderDeactivateCallee(ctx, "done");

        _ = SequenceRendererUtils.RenderDeactivateCaller(ctx);
    }
}
