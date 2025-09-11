using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;

namespace ExporterKit.Html;

public class HtmlPageWithTabsEntityListBuilder<DTO> : HtmlPageWithTabsBuilder<DTO>
where DTO : ContextKeyContainer
{
    private readonly Func<DTO, string> _onGetFileName;

    public HtmlPageWithTabsEntityListBuilder(IContextInfoDataset<ContextInfo> contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder, Func<DTO, string> onGetFileName)
        : base(contextInfoDataset, tabbedPageBuilder)
    {
        _onGetFileName = onGetFileName;
    }

    public override void Build()
    {
        foreach (var contextInfoItem in _contextInfoDataset)
        {
            var cellData = new ContextKeyContainer
            (
                contextInfoList: contextInfoItem.Value.Distinct(),
                contextKey: contextInfoItem.Key);

            var filename = _onGetFileName((DTO)cellData);
            var title = $" {cellData.ContextKey.Action}  ->  {cellData.ContextKey.Domain} ";

            _tabbedPageBuilder.GenerateFile(title, filename, (DTO)cellData);
        }
    }
}
