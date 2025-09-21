using System.IO;
using System.Linq;
using System.Threading;
using ExporterKit.Html.Containers;
using ExporterKit.Html.Pages.CoCompiler;
using HtmlKit.Builders.Core;
using HtmlKit.Helpers;
using HtmlKit.Model;
using HtmlKit.Model.Containers;
using HtmlKit.Options;
using HtmlKit.Page;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace HtmlKit.Document;

public class HtmlDataCellBuilderMethodList : IHtmlDataCellBuilder<MethodListTensor>
{
    private readonly IHtmlCellDataProducer<string, MethodListTensor> _dataProducer;
    private readonly IHrefManager<MethodListTensor> _hRefManager;

    public HtmlDataCellBuilderMethodList(IHrefManager<MethodListTensor> hRefManager, IHtmlCellDataProducer<string, MethodListTensor> dataProducer)
    {
        _hRefManager = hRefManager;
        _dataProducer = dataProducer;
    }

    public void BuildDataCell(TextWriter textWriter, MethodListTensor cell, HtmlTableOptions options)
    {
        var model = new ActionPerDomainMethodListDataModel();

        var index = cell.MethodIndex;
        var list = model.GetMethodsList(cell.DomainPerActionTensorContainer).ToList();
        var cellData = list.ElementAt(index);

        var attrs = new HtmlTagAttributes();
        attrs.Add("class", "left-aligned");

        // Логика отрисовки ячейки
        HtmlBuilderFactory.HtmlBuilderTableCell.Data.With(textWriter, attributes: attrs, () =>
        {
            var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefCell(cell, options) } };
            HtmlBuilderFactory.A.Cell(textWriter, attrs, cellData.FullName, isEncodable: false);
        });
    }
}