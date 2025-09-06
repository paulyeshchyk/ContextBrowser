using System;
using ContextKit.Model;
using UmlKit.Model;

namespace ExporterKit.Infrastucture;

public static class ContextInfoExt
{
    public static UmlEntityType ConvertToUmlEntityType(this ContextInfoElementType elementType)
    {
        return elementType switch
        {
            ContextInfoElementType.@class => UmlEntityType.@class,
            ContextInfoElementType.@record => UmlEntityType.@record,
            ContextInfoElementType.@struct => UmlEntityType.@struct,
            ContextInfoElementType.@interface => UmlEntityType.@interface,
            ContextInfoElementType.@enum => UmlEntityType.@enum,
            ContextInfoElementType.@delegate => UmlEntityType.@delegate,
            ContextInfoElementType.none => UmlEntityType.@none,
            ContextInfoElementType.method => UmlEntityType.@method,
            ContextInfoElementType.@namespace => UmlEntityType.@namespace,
            ContextInfoElementType.property => UmlEntityType.@property,
            _ => throw new NotImplementedException()
        };
    }
}
