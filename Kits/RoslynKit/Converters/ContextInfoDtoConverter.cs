using System;
using ContextKit.Model;

namespace RoslynKit.Converters;

public static class ContextInfoDtoConverter
{
    public static IContextInfo ConvertFromSyntaxNodeWrapper<TContext, TWrapper>(TContext? ownerContext, TWrapper syntaxWrap, ISymbolInfo wrapper, ContextInfoElementType ElementType)
        where TWrapper : ISyntaxNodeWrapper
        where TContext : IContextWithReferences<TContext>
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

        return new ContextInfoDto(
              elementType: ElementType,
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