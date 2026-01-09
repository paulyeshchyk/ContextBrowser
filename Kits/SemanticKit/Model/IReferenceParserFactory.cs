using ContextKit.Model;
using SemanticKit.Parsers.Strategy.Invocation;

namespace SemanticKit.Model;

public interface IReferenceParserFactory<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    IInvocationFileParser<ContextInfo> Create();
}

