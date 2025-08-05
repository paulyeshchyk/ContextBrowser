using ContextBrowser.DiagramFactory.Model;
using ContextKit.Model;
using LoggerKit;
using RoslynKit.Model;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public class IncomingTransitionBuilder : ITransitionBuilder
{
    private readonly OnWriteLog? _onWriteLog;
    private readonly RoslynCodeParserOptions _options;

    public IncomingTransitionBuilder(RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _onWriteLog = onWriteLog;
    }

    public DiagramDirection Direction => DiagramDirection.Incoming;

    public IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        foreach (var callee in domainMethods)
        {
            foreach (var caller in callee.InvokedBy ?? Enumerable.Empty<ContextInfo>())
            {
                if (caller.ElementType != ContextInfoElementType.method)
                    continue;

                var result = UmlTransitionDtoBuilder.CreateTransition(caller, callee, _onWriteLog, _options);
                if (result != null)
                {
                    yield return (UmlTransitionDto)result;
                }
            }
        }
    }
}
