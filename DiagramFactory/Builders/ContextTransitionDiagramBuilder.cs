using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.UmlKit.Diagrams;

namespace ContextBrowser.DiagramFactory.Builders;

public class ContextTransitionDiagramBuilder : IContextDiagramBuilder
{
    public string Name => "context-transition";

    private const string STransitionDataMismatchErrorTemplate = "[Ошибка] Недостаточно информации для построения связей для {0}";
    private readonly ContextTransitionDiagramBuilderOptions _options;
    public ContextTransitionDiagramBuilder(ContextTransitionDiagramBuilderOptions? options = default)
    {
        _options = options ?? new ContextTransitionDiagramBuilderOptions();
    }

    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram target)
    {
        // 1. Фильтрация методов текущего домена с валидным контекстом
        var methodsInDomain = allContexts
            .Where(ctx => ctx.ElementType == ContextInfoElementType.method && ctx.Domains.Contains(domainName) && classifier.HasActionAndDomain(ctx))
            .ToList();

        if(!methodsInDomain.Any())
        {
            Console.WriteLine($"[MISS]: не найдено методов для домена {domainName}");
            return false;
        }

        // 2. Уникальные переходы
        var transitions = new HashSet<UmlTransitionDto>();

        //выведем в консоль бракованные caller
        foreach(var caller in methodsInDomain.Where(m => !m.References.Any()))
        {
            Console.WriteLine($"[MISS]: не найдено методов для домена {domainName} и caller {caller.Name}");
        }

        foreach(var caller in methodsInDomain.Where(m => m.References.Any()))
        {
            foreach(var callee in caller.References)
            {
                if(callee.ElementType != ContextInfoElementType.method)
                    continue;

                var calleeDomain = callee.Domains.FirstOrDefault();
                if(string.IsNullOrWhiteSpace(calleeDomain))
                    continue;

                var transition = new UmlTransitionDto(
                    caller: caller.DisplayNameAlphaNumericOnly,
                    callee: callee.DisplayNameAlphaNumericOnly,
                    domain: calleeDomain)
                {
                    CallerName = caller.ClassOwner?.Name ?? "???",
                    CalleeName = callee.ClassOwner?.Name ?? "???",
                    MethodName = callee.Name
                };

                transitions.Add(transition);
            }
        }

        if(!transitions.Any())
            return false;

        foreach(var t in transitions)
        {
            var callerParticipant = _options.UseClassAsParticipant ? t.CallerName : t.caller;
            var calleeParticipant = _options.UseClassAsParticipant ? t.CalleeName : t.callee;

            if(string.IsNullOrWhiteSpace(callerParticipant) || string.IsNullOrWhiteSpace(calleeParticipant))
            {
                Console.WriteLine(string.Format(STransitionDataMismatchErrorTemplate, t));
                continue;
            }

            target.AddParticipant(t.domain);
            target.AddParticipant(callerParticipant);
            target.AddParticipant(calleeParticipant);

            target.AddTransition(t.domain, callerParticipant, "entry");

            var arrowLabel = _options.UseMethodAsLabel
                ? (t.MethodName ?? "calls")
                : "calls";

            target.AddTransition(callerParticipant, calleeParticipant, arrowLabel);
            target.AddTransition(calleeParticipant, t.domain, "exit");
        }

        return true;
    }
}

public record ContextTransitionDiagramBuilderOptions
{
    public bool UseClassAsParticipant { get; init; } = true;

    public bool UseMethodAsLabel { get; init; } = true;
}

public readonly record struct UmlTransitionDto(string caller, string callee, string domain)
{
    public string? CallerName { get; init; } = null;

    public string? CalleeName { get; init; } = null;

    public string? MethodName { get; init; } = null;

    public static implicit operator (string Caller, string Callee, string Domain)(UmlTransitionDto value)
        => (value.caller, value.callee, value.domain);

    public static implicit operator UmlTransitionDto((string Caller, string Callee, string Domain) value)
        => new(value.Caller, value.Callee, value.Domain);

    public override int GetHashCode()
        => HashCode.Combine(caller, callee, domain);

    public bool Equals(UmlTransitionDto other)
        => string.Equals(caller, other.caller, StringComparison.OrdinalIgnoreCase)
        && string.Equals(callee, other.callee, StringComparison.OrdinalIgnoreCase)
        && string.Equals(domain, other.domain, StringComparison.OrdinalIgnoreCase);
}
