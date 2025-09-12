using System;
using ContextKit.Model;

namespace ExporterKit.Infrastucture;

public interface IContextInfoMapperFactory
{
    IContextKeyMap<ContextInfo> CreateMapper(MapperKeyBase type);
}
