namespace SemanticKit.Model;

public interface ISemanticInvocationResolver
{
    ISemanticModelWrapper? Resolve(ISyntaxTreeWrapper? syntaxTree);
}

//context: roslyn, read
public class SemanticInvocationResolver : ISemanticInvocationResolver
{
    private readonly ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> _storage;

    //context: roslyn, read
    public SemanticInvocationResolver(ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> storage)
    {
        _storage = storage;
    }

    //context: roslyn, read
    public ISemanticModelWrapper? Resolve(ISyntaxTreeWrapper? syntaxTree)
    {
        return syntaxTree == null
            ? null
            : _storage.GetModel(syntaxTree);
    }
}