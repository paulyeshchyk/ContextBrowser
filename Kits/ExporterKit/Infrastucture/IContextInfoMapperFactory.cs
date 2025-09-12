using System;
using ContextKit.Model;
using ContextKit.Model.Collector;

namespace ExporterKit.Infrastucture;

public interface IContextInfoMapperFactory
{
    IContextKeyMap<ContextInfo, IContextKey> GetMapper(MapperKeyBase type);
}

public interface IContextInfoFlatMapperFactory
{
    IContextKeyIndex<ContextInfo> GetMapper(MapperKeyBase type);
}
