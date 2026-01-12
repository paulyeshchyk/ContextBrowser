using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;
using UmlKit.Builders.Model;
using UmlKit.DataProviders;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.TransitionDirection;

public class OutgoingTransitionBuilder : ITransitionBuilder
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;


    public OutgoingTransitionBuilder(IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> contextInfoManager)
    {
        _logger = logger;
        _contextInfoManager = contextInfoManager;
    }

    public DiagramDirection Direction => DiagramDirection.Outgoing;

    public GrouppedSortedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        var resultList = new GrouppedSortedTransitionList();
        _logger.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, "Iterating domain methods", LogLevelNode.Start);
        foreach (var ctx in domainMethods.OrderBy(m => m.SpanStart))
        {
            var theKey = ctx.Identifier;

            _logger.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, $"Getting references for method [{ctx.Name}]", LogLevelNode.Start);
            var references = _contextInfoManager.GetReferencesSortedByInvocation(ctx);
            foreach (var callee in references)
            {
                if (callee.ElementType != ContextInfoElementType.method)
                {
                    _logger.WriteLog(AppLevel.P_Tran, LogLevel.Warn, $"Найдена ссылка, записанная в Reference, но не являющаяся методом [{callee.Name}]");
                    continue;
                }
                var result = UmlTransitionDtoBuilder.CreateTransition(ctx, callee, _logger, theKey);
                if (result == null)
                {
                    _logger.WriteLog(AppLevel.P_Tran, LogLevel.Err, "Объект UmlTransitionDto не создан");
                    continue;
                }

                resultList.Add(result, theKey.ToString());
            }
            _logger.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        }
        _logger.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return resultList;
    }
}
