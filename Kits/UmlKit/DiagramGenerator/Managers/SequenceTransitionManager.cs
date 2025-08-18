using UmlKit.Builders;
using UmlKit.Infrastructure.Options;
using UmlKit.Infrastructure.Options.Indication;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Managers;

public static partial class SequenceTransitionManager
{
    // Отвечает за добавление переходов на диаграмму.
    internal static void TryAddTransition<T>(string from, string to, UmlDiagram<T> diagram, DiagramIndicationOption opt, string reason)
        where T : IUmlParticipant
    {
        var fromParticipant = diagram.AddParticipant(from);
        var toParticipant = diagram.AddParticipant(to);
        diagram.AddTransition(fromParticipant, toParticipant, isAsync: opt.UseAsync, reason);
    }

    internal static void SystemCall<P>(IUmlTransitionFactory<P> factory, DiagramBuilderOptions options, UmlDiagram<P> diagram, string caller, string? reason, bool visibleWithoutActivation = false)
        where P : IUmlParticipant
    {
        if(options.Activation.UseActivation || visibleWithoutActivation)
        {
            var from = factory.CreateTransitionObject(string.Empty);
            var to = factory.CreateTransitionObject(caller);

            var reasonDebug = options.Debug ? $"SYSTEM CALL [ {reason} ]" : reason;
            var systemCall = factory.CreateTransition(from, to, reasonDebug);
            diagram.AddTransition(systemCall);
        }
    }
}
