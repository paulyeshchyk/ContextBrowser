using UmlKit.Infrastructure.Options;
using UmlKit.Model;

namespace UmlKit.Diagrams;

// context: uml, model
public class UmlDiagramClasses : UmlDiagram<UmlState>
{
    private readonly HashSet<UmlState> _states = new();
    private readonly List<UmlTransitionState> _transitions = new();

    public UmlDiagramClasses(ContextTransitionDiagramBuilderOptions options) : base(options)
    {
    }

    public override void AddCallbreakNote(string name)
    {
#warning that is not correct for Class diagram
        Add(new UmlNote(name, UmlNotePosition.Left, $"{name} -> {name}:"));
    }

    public override void AddSelfCallContinuation(string name, string methodName)
    {
#warning not working
    }

    // context: uml, create
    public override UmlState AddParticipant(string name, string? alias = null, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        var result = new UmlState(name, alias);
        _states.Add(result);
        return result;
    }

    public override UmlDiagram<UmlState> AddParticipant(UmlState participant, string alias)
    {
        if(participant is UmlState state)
        {
            _states.Add(state);
            return this;
        }

        throw new ArgumentException($"UmlState is supported only {nameof(participant)}");
    }


    // context: uml, create
    //public override IUmlElement AddTransition(string? from, string? to, bool isAsync = false, string? label = null)
    //{
    //    var theFrom = new UmlState(from);
    //    var theTo = new UmlState(to);
    //    return AddTransition(theFrom, theTo, isAsync, label);
    //}

    public override IUmlElement AddTransition(UmlState from, UmlState to, bool isAsync = false, string? label = null)
    {
        if(from is not UmlState theFrom)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        if(to is not UmlState theTo)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        var result = new UmlTransitionState(theFrom, theTo, new UmlArrow(), label);
        return AddTransition(result);
    }


    public override IUmlElement AddTransition(IUmlTransition<UmlState> transition)
    {
        _transitions.Add((UmlTransitionState)transition);
        Add(transition);
        return transition;
    }

    public override IUmlElement? Activate(string source, string destination, string reason, bool softActivation)
    {
        throw new NotImplementedException();
    }

    public override IUmlElement Activate(string from, string reason, bool softActivation)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Activate(dto, reason, softActivation);
    }

    public override IUmlElement Deactivate(string from)
    {
        var dto = new UmlDeclarableDto("empty declaration", from);
        return Deactivate(dto);
    }

    public override IUmlElement Activate(IUmlDeclarable from, string reason, bool softActivation)
    {
        return new UmlActivate(null, from.Alias, reason, softActivation);
    }

    public override IUmlElement Deactivate(IUmlDeclarable from)
    {
        return new UmlDeactivate(from.Alias);
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

    public override IUmlElement AddLine(string line)
    {
        var result = new UmlLine(line);
        return result;
    }
}