using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandlineKit;
using ContextBrowser.Infrastructure;
using ContextBrowser.Services;
using ContextBrowser.Services.ContextInfoProvider;
using ContextBrowser.Services.Parsing;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Comment;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ContextKit.Model.CacheManager;
using ContextKit.Model.Collector;
using ContextKit.Model.Factory;
using ContextKit.Model.WordTensorBuildStrategy;
using ExporterKit.Csv;
using ExporterKit.Html.Containers;
using ExporterKit.Html.Pages;
using ExporterKit.Html.Pages.CoCompiler;
using ExporterKit.Html.Pages.CoCompiler.DomainPerAction;
using ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;
using ExporterKit.Infrastucture;
using ExporterKit.Uml;
using HtmlKit.Document;
using HtmlKit.Helpers;
using HtmlKit.Matrix;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoslynKit.Assembly;
using RoslynKit.Phases;
using RoslynKit.Phases.Invocations;
using RoslynKit.Tree;
using RoslynKit.Wrappers.Extractor;
using SemanticKit.Model;
using TensorKit.Factories;
using TensorKit.Model;
using UmlKit.Compiler;
using UmlKit.Compiler.Orchestrant;

namespace ContextBrowser;

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