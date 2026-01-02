using System;
using System.Collections.Generic;
using ContextKit.Model;
using ExporterKit.Infrastucture;

namespace ContextBrowser.Infrastructure;

public class ContextInfoFlatMapperFactory : IContextInfoIndexerFactory
{
    private readonly IDictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>> _mappers;

    public ContextInfoFlatMapperFactory(IDictionary<MapperKeyBase, IKeyIndexBuilder<ContextInfo>> mappers)
    {
        _mappers = mappers;
    }

    public IKeyIndexBuilder<ContextInfo> GetMapper(MapperKeyBase type)
    {
        return _mappers.TryGetValue(type, out var mapper)
            ? mapper
            : throw new NotImplementedException($"Mapper for type {type} is not registered.");
    }
}