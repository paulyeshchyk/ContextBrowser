using ContextKit.Model;

namespace ExporterKit.Infrastucture;

public interface IContextInfoMapperFactory<TTensor>
    where TTensor : notnull
{
    IContextInfo2DMap<ContextInfo, TTensor> GetMapper(MapperKeyBase type);
}

public interface IContextInfoIndexerFactory
{
    IKeyIndexBuilder<ContextInfo> GetMapper(MapperKeyBase type);
}
