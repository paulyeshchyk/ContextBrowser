using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using HtmlKit.Model.Containers;
using TensorKit.Model;

namespace HtmlKit.Builders.Page.Tabs.CoBuilders;

public class HtmlPageWithTabsEntityListBuilder<DTO, TTensor> : HtmlPageWithTabsBuilder<DTO, TTensor>
    where DTO : ContextInfoKeyContainerTensor<TTensor>
    where TTensor : ITensor
{
    private readonly Func<DTO, string> _onGetFileName;
    private readonly Func<DTO, string> _onGetTitle;

    public HtmlPageWithTabsEntityListBuilder(IContextInfoDataset<ContextInfo, TTensor> contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder, Func<DTO, string> onGetFileName, Func<DTO, string> onGetTitle)
        : base(contextInfoDataset, tabbedPageBuilder)
    {
        _onGetFileName = onGetFileName;
        _onGetTitle = onGetTitle;
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
                var title = _onGetTitle((DTO)cellData);

                _tabbedPageBuilder.GenerateFile(title, filename, (DTO)cellData);
            }
        });
    }
}
