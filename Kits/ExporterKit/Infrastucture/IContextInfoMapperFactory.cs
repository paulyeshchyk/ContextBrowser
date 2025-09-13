using System;
using ContextKit.Model;
using ContextKit.Model.Collector;

namespace ExporterKit.Infrastucture;

public interface IContextInfoMapperFactory
{
    IContextKeyMap<ContextInfo, IContextKey> GetMapper(MapperKeyBase type);
}

public interface IContextInfoIndexerFactory
{
    IContextKeyIndexer<ContextInfo> GetMapper(MapperKeyBase type);
}
