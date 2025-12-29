using System.IO;
using System.Linq;
using UmlKit.Infrastructure.Options;

namespace UmlKit.PlantUmlSpecification;

public class UmlDiagramMindmap : UmlDiagram<UmlNode>
{
    protected override string SUmlStartTag { get => "@startmindmap"; }

    protected override string SUmlEndTag { get => "@endmindmap"; }

    public UmlDiagramMindmap(DiagramBuilderOptions options, string diagramId) : base(options, diagramId)
    {
    }

    public override void WriteBody(TextWriter writer, UmlWriteOptions writeOptions)
    {
        foreach (var element in Elements.OrderBy(e => e.Key).Select(e => e.Value))
        {
            element.WriteTo(writer, writeOptions);
        }
    }

    public override IUmlElement? Activate(string destination, string reason, bool softActivation) => throw new System.NotImplementedException();

    public override IUmlElement? Activate(string source, string destination, string reason, bool softActivation) => throw new System.NotImplementedException();

    public override IUmlElement? Activate(IUmlDeclarable from, string reason, bool softActivation) => throw new System.NotImplementedException();

    public override void AddCallbreakNote(string name) => throw new System.NotImplementedException();

    public override IUmlElement AddLine(string line) => throw new System.NotImplementedException();

    public override UmlNode AddParticipant(string name, string? alias = null, string? url = null, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant) => throw new System.NotImplementedException();

    public override UmlDiagram<UmlNode> AddParticipant(UmlNode participant, string alias) => throw new System.NotImplementedException();

    public override void AddSelfCallContinuation(string name, string methodName) => throw new System.NotImplementedException();

    public override IUmlElement AddTransition(IUmlTransition<UmlNode> transition) => throw new System.NotImplementedException();

    public override IUmlElement AddTransition(UmlNode from, UmlNode to, bool isAsync = false, string? label = null) => throw new System.NotImplementedException();

    public override IUmlElement? Deactivate(string from) => throw new System.NotImplementedException();

    public override IUmlElement? Deactivate(IUmlDeclarable from) => throw new System.NotImplementedException();
}
