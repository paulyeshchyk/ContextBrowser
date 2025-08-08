using ContextKit.Model;
using LoggerKit;
using RoslynKit.Model;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public class BiDirectionalTransitionBuilder : ITransitionBuilder
{
    public DiagramDirection Direction => DiagramDirection.BiDirectional;

    private readonly OutgoingTransitionBuilder _outgoing;
    private readonly IncomingTransitionBuilder _incoming;
    private readonly RoslynCodeParserOptions _options;

    public BiDirectionalTransitionBuilder(RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _outgoing = new OutgoingTransitionBuilder(options, onWriteLog);
        _incoming = new IncomingTransitionBuilder(options, onWriteLog);
    }

    public GroupedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        var outgoing = _outgoing.BuildTransitions(domainMethods, allContexts);
        var incoming = _incoming.BuildTransitions(domainMethods, allContexts);
        var result = outgoing.Concat(incoming);

        var theList = _options.ShowForeignInstancies
            ? result
            : result.GroupBy(r => r.Id).Distinct().Select(g => g.FirstOrDefault());
        var resultList = new GroupedTransitionList();
        foreach(var a in theList)
        {
            resultList.Add(a);
        }
        return resultList;
    }
}