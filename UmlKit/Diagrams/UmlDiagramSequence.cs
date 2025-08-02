using UmlKit.Model;

namespace UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramSequence : UmlDiagram
{
    private readonly HashSet<UmlParticipant> _participants = new();
    private readonly List<UmlSequence> _transitions = new();
    private readonly List<UmlTransitionBlock> transitionBlocks = new();

    public override IUmlElement AddParticipant(string? name, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        var result = new UmlParticipant(name, keyword);
        _participants.Add(result);
        return result;
    }

    public override UmlDiagram AddParticipant(IUmlElement participant)
    {
        if(participant is UmlParticipant theParticipant)
        {
            _participants.Add(theParticipant);
            return this;
        }

        throw new ArgumentException($"UmlParticipant is supported only {nameof(participant)}");
    }

    // context: uml, create
    public override IUmlElement AddTransition(string? from, string? to, string? label = null)
    {
        var theFrom = new UmlParticipant(from);
        var theTo = new UmlParticipant(to);

        return AddTransition(theFrom, theTo, label);
    }

    // context: uml, create
    public override IUmlElement AddTransition(IUmlDeclarable from, IUmlDeclarable to, string? label = null)
    {
        if(from is not UmlParticipant theFrom || to is not UmlParticipant theTo)
            throw new ArgumentException("Only UmlParticipant is supported for transition");

        // Активируем ТОЛЬКО FROM
        var block = transitionBlocks.FirstOrDefault(b => b.Subject.ShortName == from.ShortName);
        if(block == null)
        {
            block = new UmlTransitionBlock(from);
            transitionBlocks.Add(block);
        }

        if(!block.IsActivated)
        {
            block.Activate = new UmlActivate(from.ShortName);
            block.Deactivate = new UmlDeactivate(from.ShortName);
            block.MarkActivated();
        }

        var transition = new UmlSequence(theFrom, theTo, label);
        _transitions.Add(transition);

        return transition;
    }

    public override IUmlElement? Activate(string from)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Activate(dto);
    }

    public override IUmlElement? Deactivate(string from)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Deactivate(dto);
    }

    public override IUmlElement? Activate(IUmlDeclarable from)
    {
        var tb = transitionBlocks.FirstOrDefault(tb => tb.Subject.ShortName == from.ShortName);
        if(tb == null)
        {
            tb = new UmlTransitionBlock(from);
            transitionBlocks.Add(tb);
        }

        if(tb.IsActivated)
            return null;

        var result = new UmlActivate(from.ShortName);
        tb.Activate = result;
        tb.MarkActivated();
        return result;
    }

    public override IUmlElement? Deactivate(IUmlDeclarable from)
    {
        var tb = transitionBlocks.FirstOrDefault(tb => tb.Subject.ShortName == from.ShortName);
        if(tb == null)
        {
            tb = new UmlTransitionBlock(from);
            transitionBlocks.Add(tb);
        }

        if(!tb.IsActivated)
            return null;

        var result = new UmlDeactivate(from.ShortName);
        tb.Deactivate = result;
        tb.MarkDeactivated();
        return result;
    }

    public override void WriteBody(TextWriter writer)
    {
        foreach(var participant in _participants.Distinct())
            participant.WriteTo(writer);

        writer.WriteLine();

        foreach(var transition in _transitions)
        {
            var block = transitionBlocks
                .FirstOrDefault(tb => tb.Subject.ShortName == transition.From.ShortName);

            // Важно: сначала activate, потом сам переход
            if(block?.Activate is UmlActivate activate)
            {
                activate.WriteTo(writer);
            }

            transition.WriteTo(writer);
        }

        // В конце — деактивация всех, кто был активирован
        foreach(var block in transitionBlocks)
        {
            if(block.Deactivate is UmlDeactivate deactivate)
            {
                deactivate.WriteTo(writer);
            }
        }
    }
}