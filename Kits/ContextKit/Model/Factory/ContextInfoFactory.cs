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
               fullName: dto.FullName,
              shortName: dto.ShortName,
              nameSpace: dto.Namespace,
              spanStart: dto.SpanStart,
                spanEnd: dto.SpanEnd,
             symbolInfo: dto.SymbolWrapper,
             syntaxNode: dto.SyntaxWrapper,
             classOwner: dto.ClassOwner,
            methodOwner: dto.MethodOwner)
        {
        };

        return (T)result;
    }
}