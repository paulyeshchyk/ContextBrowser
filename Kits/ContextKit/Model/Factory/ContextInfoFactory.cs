namespace ContextKit.Model.Factory;

// context: ContextInfo, build
// pattern: Factory
public class ContextInfoFactory : IContextFactory<ContextInfo>
{
    public ContextInfo Create(IContextInfo dto)
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
            methodOwner: dto.MethodOwner);
        return result;
    }
}