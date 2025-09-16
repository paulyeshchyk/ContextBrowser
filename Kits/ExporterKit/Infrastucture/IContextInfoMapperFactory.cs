using System;
using ContextKit.Model;
using ContextKit.Model.Collector;
using TensorKit.Model;

namespace ExporterKit.Infrastucture;

public interface IContextInfoMapperFactory
{
    DomainPerActionKeyMap<ContextInfo, DomainPerActionTensor> GetMapper(MapperKeyBase type);
}

public interface IContextInfoIndexerFactory
{
    IKeyIndexBuilder<ContextInfo> GetMapper(MapperKeyBase type);
}
