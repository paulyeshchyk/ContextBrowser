using System;

namespace ContextKit.Model;

// context: model, ContextInfo
public enum ContextInfoElementType
{
    @none,
    @method,
    @class,
    @struct,
    @record,
    @enum,
    @namespace,
    @interface,
    @property,
    @delegate
}

public static class ContextInfoElementTypeExtensions
{
    public static bool IsTypeDefinition(this ContextInfoElementType elementType)
    {
        return elementType == ContextInfoElementType.@class
               || elementType == ContextInfoElementType.@struct
               || elementType == ContextInfoElementType.@record
               || elementType == ContextInfoElementType.@interface
               || elementType == ContextInfoElementType.@enum;
    }

    public static bool IsEntityDefinition(this ContextInfoElementType elementType)
    {
        return elementType == ContextInfoElementType.@class
               || elementType == ContextInfoElementType.@struct
               || elementType == ContextInfoElementType.@record
               || elementType == ContextInfoElementType.@interface;
    }

    public static bool IsMemberDefinition(this ContextInfoElementType elementType)
    {
        return elementType == ContextInfoElementType.@method
               || elementType == ContextInfoElementType.@property
               || elementType == ContextInfoElementType.@delegate;
    }
}