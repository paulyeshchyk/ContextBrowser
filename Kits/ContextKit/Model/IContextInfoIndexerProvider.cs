﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

public interface IContextInfoIndexerProvider
{
    Task<DomainPerActionKeyIndexer<ContextInfo>> GetIndexerAsync(MapperKeyBase mapperType, CancellationToken cancellationToken);
}
