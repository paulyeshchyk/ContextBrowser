using ContextBrowser.DiagramFactory.Model;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Diagrams;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionRenderer;

public class TransitionTreeRenderer : TransitionRenderer
{
    public TransitionTreeRenderer(OnWriteLog? onWriteLog) : base(onWriteLog)
    {
    }

    public bool RenderAllTransitions_WithContextTree(
        UmlDiagram diagram,
        SortedList<int, UmlTransitionDto>? allTransitions,
        ContextTransitionDiagramBuilderOptions options,
        string domain)
    {
        if((allTransitions == null) || !allTransitions.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Err, $"No transitions provided for [{domain}]");
            return false;
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"Rendering transitions for [{domain}] with context tree mode: {options.TreeBuilderMode}", LogLevelNode.Start);

        var rendered = new HashSet<string>();

        switch(options.TreeBuilderMode)
        {
            case ContextTransitionTreeBuilderMode.FromParentToChild:
                var byCaller = allTransitions.Values
                    .GroupBy(t => t.GetCallerKey())
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach(var transition in allTransitions.Values)
                    if(!rendered.Contains(transition.GetTreeKey()))
                        RenderFromParent(transition, diagram, options, byCaller, rendered);
                break;

            case ContextTransitionTreeBuilderMode.FromChildToParent:
                var byCallee = allTransitions.Values
                    .GroupBy(t => t.GetCalleeKey())
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach(var transition in allTransitions.Values)
                    if(!rendered.Contains(transition.GetTreeKey()))
                        RenderFromChild(transition, diagram, options, byCallee, rendered);
                break;

            case ContextTransitionTreeBuilderMode.BiDirectional:
                var map = allTransitions.Values.ToLookup(t => t.GetCallerKey())
                    .ToDictionary(g => g.Key, g => g.ToList());
                var reverse = allTransitions.Values.ToLookup(t => t.GetCalleeKey())
                    .ToDictionary(g => g.Key, g => g.ToList());
                _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[PREPARE] RenderAllTransitions_WithContextTree: map keys [{map.Keys}] ");
                _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[PREPARE] RenderAllTransitions_WithContextTree: reverse keys [{reverse.Keys}] ");


                _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[PREPARE] RenderAllTransitions_WithContextTree: Started looping in transitions", LogLevelNode.Start);
                foreach(var transition in allTransitions.Values)
                {
                    if(rendered.Contains(transition.GetTreeKey()))
                    {
                        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[MISS] RenderAllTransitions_WithContextTree: Rendered list contains no key {transition.GetTreeKey()}");
                        continue;
                    }
                    RenderFromParent(transition, diagram, options, map, rendered);
                    RenderFromChild(transition, diagram, options, reverse, rendered);
                }
                _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
                break;
        }

        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return true;
    }

    private void RenderFromParent(
        UmlTransitionDto transition,
        UmlDiagram diagram,
        ContextTransitionDiagramBuilderOptions options,
        Dictionary<string, List<UmlTransitionDto>> map,
        HashSet<string> rendered)
    {
        var key = transition.GetTreeKey();
        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[START] RenderFromParent key: [{key}]");
        if(!rendered.Add(key))
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Warn, $"[MISS]: RenderFromParent key not found in rendered list: [{key}]");
            return;
        }

        var ctx = new RenderContext(transition, diagram, options, new Stack<string>(), _onWriteLog);
        RenderFullTransition(ctx);

        var calleekey = transition.GetCalleeKey();
        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[PREPARE] RenderFromParent looping through children map callee key:[{calleekey}]", LogLevelNode.Start);
        if(map.TryGetValue(calleekey, out var children))
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[MAP HIT] {calleekey} has {children.Count} children:");
            foreach(var c in children)
                _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"→ {c.GetTreeKey()}");

            foreach(var child in children)
                RenderFromParent(child, diagram, options, map, rendered);
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Warn, $"[MISS]: RenderFromParent child not found [{calleekey}]", LogLevelNode.End);
        }
        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private void RenderFromChild(
        UmlTransitionDto transition,
        UmlDiagram diagram,
        ContextTransitionDiagramBuilderOptions options,
        Dictionary<string, List<UmlTransitionDto>> map,
        HashSet<string> rendered)
    {
        var key = transition.GetTreeKey();
        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[START] RenderFromChild key: [{key}]");
        if(!rendered.Add(key))
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Warn, $"[MISS]: RenderFromChild key not found in rendered list: [{key}]");
            return;
        }

        var ctx = new RenderContext(transition, diagram, options, new Stack<string>(), _onWriteLog);
        RenderFullTransition(ctx);

        var callerkey = transition.GetCallerKey();
        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"RenderFromChild looping through parent map callee key:[{callerkey}]", LogLevelNode.Start);

        if(map.TryGetValue(callerkey, out var parents))
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"[MAP HIT] {callerkey} has {parents.Count} parents:");
            foreach(var c in parents)
                _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, $"→ {c.GetTreeKey()}");
            foreach(var parent in parents)
                RenderFromChild(parent, diagram, options, map, rendered);
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Warn, $"[MISS]: RenderFromParent parent not found [{callerkey}]", LogLevelNode.End);
        }
        _onWriteLog?.Invoke(AppLevel.P_Rnd, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}

internal static class UmlTransitionDtoTreeExt
{
    public static string GetTreeKey(this UmlTransitionDto t)
    => $"{t.CallerClassName}.{t.CallerMethod}->{t.CalleeClassName}.{t.CalleeMethod}";

    public static string GetCallerKey(this UmlTransitionDto t) => $"{t.CallerId}.{t.CallerMethod}";

    public static string GetCalleeKey(this UmlTransitionDto t) => $"{t.CalleeId}.{t.CalleeMethod}";
}
