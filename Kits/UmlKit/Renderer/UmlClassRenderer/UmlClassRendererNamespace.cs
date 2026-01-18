using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace UmlKit.Renderer;

public class UmlClassRendererNamespace<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly INamingProcessor _namingProcessor;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly IContextInfoDatasetProvider<TDataTensor> _datasetProvider;
    private readonly IAppOptionsStore _optionsStore;

    public UmlClassRendererNamespace(INamingProcessor namingProcessor, IUmlUrlBuilder umlUrlBuilder, IContextInfoDatasetProvider<TDataTensor> datasetProvider, IAppOptionsStore optionsStore)
    {
        _namingProcessor = namingProcessor;
        _umlUrlBuilder = umlUrlBuilder;
        _datasetProvider = datasetProvider;
        _optionsStore = optionsStore;
    }

    public async Task<UmlRendererResult<UmlDiagramClass>> RenderAsync(string nameSpace, List<IContextInfo> classesList, CancellationToken cancellationToken)
    {
        var maxLength = Math.Max(nameSpace.Length, classesList.Count != 0 ? classesList.Max(ns => ns.Name.Length) : 0);

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var package = PumlBuilderHelper.BuildUmlPackage(nameSpace, _umlUrlBuilder, maxLength);
        foreach (var contextInfo in classesList)
        {
            var umlClass = PumlBuilderHelper.BuildUmlEntityClass(contextInfo, GetMethods(dataset), GetProperties(dataset), maxLength, _namingProcessor);

            package.Add(umlClass);
        }

        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        var diagram = new UmlDiagramClass(diagramBuilderOptions);

        diagram.Add(package);

        var opts = new UmlWriteOptions(alignMaxWidth: -1);
        var result = new UmlRendererResult<UmlDiagramClass>(diagram, opts);

        return result;
    }

    internal static Func<IContextInfo, IEnumerable<IContextInfo>> GetProperties(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return(contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.property && c.ClassOwner?.FullName == contextInfo.FullName).DistinctBy(e => e.FullName).OrderBy(e => e.ShortName);
    }

    internal static Func<IContextInfo, IEnumerable<IContextInfo>> GetMethods(IContextInfoDataset<ContextInfo, TDataTensor> contextInfoDataSet)
    {
        return(contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.method && c.ClassOwner?.FullName == contextInfo.FullName).DistinctBy(e => e.FullName).OrderBy(e => e.ShortName);
    }
}
