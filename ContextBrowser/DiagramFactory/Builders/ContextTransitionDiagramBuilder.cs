using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.UmlKit.Diagrams;
using ContextBrowser.UmlKit.Model;

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

    public bool Build(string domainName, List<ContextInfo> allContexts, ContextClassifier classifier, UmlDiagram diagram)
    {
        // 1. Фильтрация методов текущего домена с валидным контекстом
#warning revert back filter
        var methodsInDomain = allContexts
            .Where(ctx => ctx.ElementType == ContextInfoElementType.method && ctx.Domains.Contains(domainName) && classifier.HasActionAndDomain(ctx))
            .ToList();

        if(!methodsInDomain.Any())
        {
            Console.WriteLine($"[MISS]: не найдено методов для контекста {domainName}");
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

                var transition = new UmlTransitionDto
                {
                    CallerId = caller.DisplayNameAlphaNumericOnly,
                    CalleeId = callee.DisplayNameAlphaNumericOnly,
                    Domain = calleeDomain,

                    CallerName = caller.ClassOwner?.Name,
                    CalleeName = callee.ClassOwner?.Name,
                    CallerMethod = caller.Name,
                    CalleeMethod = callee.Name,
                    RunContext = caller.MethodOwner?.Name == caller.ClassOwner?.Name ? caller.Name : null,
                };

                transitions.Add(transition);
            }
        }

        if(!transitions.Any())
            return false;


        foreach(var t in transitions)
        {
            var callerParticipant = _options.UseClassAsParticipant ? t.CallerName : t.CallerId;
            var calleeParticipant = _options.UseClassAsParticipant ? t.CalleeName : t.CalleeId;

            if(string.IsNullOrWhiteSpace(callerParticipant) || string.IsNullOrWhiteSpace(calleeParticipant))
            {
                Console.WriteLine(string.Format(STransitionDataMismatchErrorTemplate, t));
                continue;
            }

            diagram.AddParticipant(t.Domain);
            diagram.AddParticipant(callerParticipant, UmlParticipantKeyword.Actor);
            diagram.AddParticipant(calleeParticipant, UmlParticipantKeyword.Actor);

            switch(_options.DetailLevel)
            {
                case DiagramDetailLevel.Summary:
                    diagram.AddTransition(t.Domain, t.CallerName, "entry");
                    diagram.AddTransition(t.CallerName, t.CalleeName, "calls");
                    diagram.AddTransition(t.CalleeName, t.Domain, "exit");
                    break;

                case DiagramDetailLevel.Method:
                    diagram.AddTransition(t.Domain, t.CallerName, t.CallerMethod);
                    diagram.AddTransition(t.CallerName, t.CalleeName, t.CalleeMethod);
                    diagram.AddTransition(t.CalleeName, t.Domain, "done");
                    break;

                case DiagramDetailLevel.Full:
                    if(!string.IsNullOrWhiteSpace(t.RunContext))
                    {
                        diagram.AddParticipant(t.RunContext, keyword: UmlParticipantKeyword.Control);
                        diagram.AddTransition(t.CallerName, t.RunContext, t.CallerMethod);
                        diagram.AddTransition(t.RunContext, t.CalleeName, t.CalleeMethod);
                        diagram.AddTransition(t.CalleeName, t.RunContext, "return");
                        diagram.AddTransition(t.RunContext, t.CallerName, "done");
                    }
                    else
                    {
                        diagram.AddTransition(t.CallerName, t.CalleeName, t.CalleeMethod);
                        diagram.AddTransition(t.CalleeName, t.CallerName, "done");
                    }
                    break;
            }
        }

        return true;
    }
}

public record ContextTransitionDiagramBuilderOptions
{
    public bool UseClassAsParticipant { get; init; } = true;

    public bool UseMethodAsLabel { get; init; } = true;

    public DiagramDetailLevel DetailLevel { get; init; } = DiagramDetailLevel.Method;
}

public enum DiagramDetailLevel
{
    Summary,    // Показываем только взаимодействия между контекстами
    Method,     // Показываем имена вызываемых методов
    Full        // Показываем "Run()", возвраты, возможно параметры
}
