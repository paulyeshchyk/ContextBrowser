using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Naming;
using ExporterKit.Html.Containers;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Document;
using HtmlKit.Helpers;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;

public class HtmlDataCellBuilderMethodList<TDataTensor> : IHtmlDataCellBuilder<MethodListTensor<TDataTensor>>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IHtmlCellDataProducer<string, MethodListTensor<TDataTensor>> _dataProducer;
    private readonly IHtmlHrefManager<MethodListTensor<TDataTensor>> _hRefManager;
    private readonly INamingProcessor _namingProcessor;

    public HtmlDataCellBuilderMethodList(IHtmlHrefManager<MethodListTensor<TDataTensor>> hRefManager, IHtmlCellDataProducer<string, MethodListTensor<TDataTensor>> dataProducer, INamingProcessor namingProcessor)
    {
        _hRefManager = hRefManager;
        _dataProducer = dataProducer;
        _namingProcessor = namingProcessor;

    }

    public async Task BuildDataCell(TextWriter textWriter, MethodListTensor<TDataTensor> cell, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        var model = new ActionPerDomainMethodListDataModel<TDataTensor>(_namingProcessor);

        var index = cell.MethodIndex;
        var list = model.GetMethodsList(cell.DomainPerActionTensorContainer).ToList();
        var contextInfo = list.ElementAt(index);

        var attrs = new HtmlTagAttributes { { "class", "left-aligned" } };

        // Логика отрисовки ячейки
        await HtmlBuilderFactory.HtmlBuilderTableCell.Data.WithAsync(textWriter, attributes: attrs, (token) =>
        {
            var methodOwner = contextInfo.MethodOwner?.FullName ?? contextInfo.Name;

            //var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefCell(cell, options) } };
            var htmlPage = _namingProcessor.ClassOnlyHtmlFilename(methodOwner);
            var tagAttrs = new HtmlTagAttributes() { { "href", $"{htmlPage}?m={contextInfo.Name}" } };

            HtmlBuilderFactory.A.CellAsync(textWriter, tagAttrs, contextInfo.Name, isEncodable: false, cancellationToken: token);
            return Task.CompletedTask;
        }, cancellationToken).ConfigureAwait(false);
    }
}