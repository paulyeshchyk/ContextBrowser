using UmlKit.Model;
using UmlKit.Model.Options;

namespace UmlKit.Diagrams;

// context: uml, model
public class UmlDiagramClasses : UmlDiagram
{
    private readonly HashSet<UmlState> _states = new();
    private readonly List<UmlTransition> _transitions = new();

    public UmlDiagramClasses(ContextTransitionDiagramBuilderOptions options) : base(options)
    {
    }

    public override void AddSelfCallBreak(string name)
    {
#warning that is not correct for Class diagram
        Add(new UmlNote(name, UmlNotePosition.Left, $"{name} -> {name}:"));
    }

    public override void AddSelfCallContinuation(string name)
    {
#warning not working
        //throw new NotImplementedException();
    }

    // context: uml, create
    public override IUmlElement AddParticipant(string? name, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        var result = new UmlState(name);
        _states.Add(result);
        return result;
    }

    public override UmlDiagram AddParticipant(IUmlElement participant)
    {
        if(participant is UmlState state)
        {
            _states.Add(state);
            return this;
        }

        throw new ArgumentException($"UmlState is supported only {nameof(participant)}");
    }

    // context: uml, create
    public override IUmlElement AddTransition(string? from, string? to, string? label = null)
    {
        var theFrom = new UmlState(from);
        var theTo = new UmlState(to);
        return AddTransition(theFrom, theTo, label);
    }

    public override IUmlElement AddTransition(IUmlDeclarable from, IUmlDeclarable to, string? label = null)
    {
        if(from is not UmlState theFrom)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        if(to is not UmlState theTo)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        var result = new UmlTransition(theFrom, theTo, new UmlArrow(), label);
        _transitions.Add(result);
        return result;
    }

    public override IUmlElement Activate(string from)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Activate(dto);
    }

    public override IUmlElement Deactivate(string from)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Deactivate(dto);
    }

    public override IUmlElement Activate(IUmlDeclarable from)
    {
        return new UmlActivate(from.ShortName);
    }

    public override IUmlElement Deactivate(IUmlDeclarable from)
    {
        return new UmlDeactivate(from.ShortName);
    }

    // context: uml, share
    public override void WriteBody(TextWriter writer)
    {
        writer.WriteLine();

        foreach(var element in _elements.Values)
        {
            element.WriteTo(writer);
        }
    }
}