using System;

namespace ContextKit.Model;

public interface IContextInfoDtoConverter<TContext, TWrapper>
        where TContext : IContextWithReferences<TContext>
        where TWrapper : ISyntaxNodeWrapper

{
    IContextInfo Convert(TContext? ownerContext, TWrapper syntaxWrap, ISymbolInfo wrapper, ContextInfoElementType elementType);
}


public class ContextInfoDtoConverter<TContext> : IContextInfoDtoConverter<TContext, ISyntaxNodeWrapper>
        where TContext : IContextWithReferences<TContext>
{
    public IContextInfo Convert(TContext? ownerContext, ISyntaxNodeWrapper syntaxWrap, ISymbolInfo wrapper, ContextInfoElementType elementType)
    {
        var nameSpace = wrapper.Namespace;
        var identifier = wrapper.Identifier;

        string fullName = wrapper.GetFullName();
        if (string.IsNullOrEmpty(fullName))
        {
            throw new Exception("incorrect wrapper");
        }
        string name = wrapper.GetName();
        string shortName = wrapper.GetShortName();

#warning read it from wrapper.Modifiers
        var elementVisibility = ContentInfoElementVisibility.@public;

        return new ContextInfoDto(
              elementType: elementType,
        elementVisibility: elementVisibility,
                 fullName: fullName,
                     name: name,
                shortName: shortName,
                nameSpace: nameSpace,
               identifier: identifier,
            symbolWrapper: wrapper,
            syntaxWrapper: syntaxWrap,
               classOwner: ownerContext,
              methodOwner: ownerContext);
    }
}