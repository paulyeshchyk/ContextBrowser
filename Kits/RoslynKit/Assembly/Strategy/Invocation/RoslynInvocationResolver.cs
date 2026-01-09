using RoslynKit.Model.Meta;
using SemanticKit.Model;
using SemanticKit.Parsers.Strategy.Invocation;

namespace RoslynKit.Assembly.Strategy.Invocation;

//context: roslyn, read
public class RoslynInvocationResolver : IInvocationResolver<RoslynSyntaxTreeWrapper>
{
    private readonly ISemanticModelStorage<RoslynSyntaxTreeWrapper, ISemanticModelWrapper> _storage;

    //context: roslyn, read
    public RoslynInvocationResolver(ISemanticModelStorage<RoslynSyntaxTreeWrapper, ISemanticModelWrapper> storage)
    {
        _storage = storage;
    }

    //context: roslyn, read
    public ISemanticModelWrapper? Resolve(RoslynSyntaxTreeWrapper? syntaxTree)
    {
        return syntaxTree == null
            ? null
            : _storage.GetModel(syntaxTree);
    }
}