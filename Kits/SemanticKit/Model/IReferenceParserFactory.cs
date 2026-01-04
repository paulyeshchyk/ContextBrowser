using ContextKit.Model;

namespace SemanticKit.Model;

public interface IReferenceParserFactory<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    IInvocationParser<ContextInfo> Create();
}

