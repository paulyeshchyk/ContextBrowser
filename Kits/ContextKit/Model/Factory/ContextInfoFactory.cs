namespace ContextKit.Model.Factory;

// context: ContextInfo, build
// pattern: Factory
public class ContextInfoFactory<T> : IContextFactory<T>
    where T : ContextInfo
{
    public T Create(IContextInfo dto)
    {
        var result = new ContextInfo
            (
            elementType: dto.ElementType,
             identifier: dto.Identifier,
                   name: dto.Name,
              nameSpace: dto.Namespace,
               fullName: dto.FullName,
              spanStart: dto.SpanStart,
                spanEnd: dto.SpanEnd,
                 symbol: dto.Symbol,
             syntaxNode: dto.SyntaxNode,
             classOwner: dto.ClassOwner,
            methodOwner: dto.MethodOwner)
        { };

        return (T)result;
    }
}