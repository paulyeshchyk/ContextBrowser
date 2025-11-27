using System.IO;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
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

    public HtmlDataCellBuilderMethodList(IHtmlHrefManager<MethodListTensor<TDataTensor>> hRefManager, IHtmlCellDataProducer<string, MethodListTensor<TDataTensor>> dataProducer)
    {
        _hRefManager = hRefManager;
        _dataProducer = dataProducer;
    }

    public void BuildDataCell(TextWriter textWriter, MethodListTensor<TDataTensor> cell, HtmlTableOptions options)
    {
        var model = new ActionPerDomainMethodListDataModel<TDataTensor>();

        var index = cell.MethodIndex;
        var list = model.GetMethodsList(cell.DomainPerActionTensorContainer).ToList();
        var cellData = list.ElementAt(index);

        var attrs = new HtmlTagAttributes();
        attrs.Add("class", "left-aligned");

        // Логика отрисовки ячейки
        HtmlBuilderFactory.HtmlBuilderTableCell.Data.With(textWriter, attributes: attrs, () =>
        {
            var methodOwner = cellData.MethodOwner?.Name ?? cellData.Name;

            //var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefCell(cell, options) } };
            var attrs = new HtmlTagAttributes() { { "href", $"class_only_{methodOwner.AlphanumericOnly()}.html?m={cellData.Name}" } };
            HtmlBuilderFactory.A.Cell(textWriter, attrs, cellData.FullName, isEncodable: false);
        });
    }
}