using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Model;
using HtmlKit.Model.Containers;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;
using TensorKit.Model;

namespace ExporterKit.Html;

public class HtmlPageWithTabsEntityListBuilder<DTO, TTensor> : HtmlPageWithTabsBuilder<DTO, TTensor>
    where DTO : ContextInfoKeyContainerTensor<TTensor>
    where TTensor : ITensor
{
    private readonly Func<DTO, string> _onGetFileName;

    public HtmlPageWithTabsEntityListBuilder(IContextInfoDataset<ContextInfo, TTensor> contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder, Func<DTO, string> onGetFileName)
        : base(contextInfoDataset, tabbedPageBuilder)
    {
        _onGetFileName = onGetFileName;
    }

    public override Task BuildAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            foreach (var contextInfoItem in _contextInfoDataset)
            {
                var cellData = new ContextInfoKeyContainerTensor<TTensor>
                (
                    contextInfoList: contextInfoItem.Value.Distinct(),
                    contextKey: contextInfoItem.Key);

                var filename = _onGetFileName((DTO)cellData);
                var title = string.Join(" -> ", cellData.ContextKey.Dimensions);

                _tabbedPageBuilder.GenerateFile(title, filename, (DTO)cellData);
            }
        });
    }
}
