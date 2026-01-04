using RoslynKit.Model.Meta;
using SemanticKit;
using SemanticKit.Model;

namespace RoslynKit.Assembly;

//context: roslyn, read
public class RoslynSemanticInvocationResolver : ISemanticInvocationResolver<RoslynSyntaxTreeWrapper>
{
    private readonly ISemanticModelStorage<RoslynSyntaxTreeWrapper, ISemanticModelWrapper> _storage;

    //context: roslyn, read
    public RoslynSemanticInvocationResolver(ISemanticModelStorage<RoslynSyntaxTreeWrapper, ISemanticModelWrapper> storage)
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