using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.ContextData;
using ContextKit.Model;
using TensorKit.Model;
using UmlKit.Builders;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;
using UmlKit.Renderer;

namespace UmlKit.Renderer;

public class UmlClassRendererClassOnly
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly IUmlUrlBuilder _umlUrlBuilder;
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;

    public UmlClassRendererClassOnly(IAppOptionsStore optionsStore, IUmlUrlBuilder umlUrlBuilder, IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider)
    {
        _optionsStore = optionsStore;
        _umlUrlBuilder = umlUrlBuilder;
        _datasetProvider = datasetProvider;
    }

    public async Task<UmlRendererResult<UmlDiagramClass>> RenderAsync(IContextInfo classownerInfo, CancellationToken cancellationToken)
    {
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();
        var diagram = new UmlDiagramClass(diagramBuilderOptions);

        var package = PumlBuilderHelper.BuildUmlPackage(classownerInfo.Namespace, _umlUrlBuilder);
        diagram.Add(package);

        var umlClass = PumlBuilderHelper.BuildUmlEntity(classownerInfo);
        package.Add(umlClass);

        var contextInfoDataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        var onGetMethods = GetMethods(contextInfoDataset);
        var classMethods = onGetMethods(classownerInfo);
        PumlBuilderHelper.AddUmlMethods(umlClass, classMethods);

        var onGetProps = GetProperties(contextInfoDataset);
        var classProperties = onGetProps(classownerInfo);
        PumlBuilderHelper.AddUmlProperties(umlClass, classProperties);

        var result = new UmlRendererResult<UmlDiagramClass>(diagram, new UmlWriteOptions(alignMaxWidth: -1));
        return result;
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetProperties(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.property && c.ClassOwner?.FullName == contextInfo.FullName).DistinctBy(e => e.FullName).OrderBy(e => e.ShortName);
    }

    private static Func<IContextInfo, IEnumerable<IContextInfo>> GetMethods(IContextInfoDataset<ContextInfo, DomainPerActionTensor> contextInfoDataSet)
    {
        return (contextInfo) => contextInfoDataSet.GetAll().Where(c => c.ElementType == ContextInfoElementType.method && c.ClassOwner?.FullName == contextInfo.FullName).DistinctBy(e => e.FullName).OrderBy(e => e.ShortName);
    }

}
