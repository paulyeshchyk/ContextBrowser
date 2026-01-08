using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using HtmlKit.Model.Containers;

namespace HtmlKit.Builders.Page.Tabs.CoBuilders;

public class HtmlPageWithTabsEntityBuilder<TDto, TTensor> : HtmlPageWithTabsBuilder<TDto, TTensor>
    where TDto : ContextInfoKeyContainerEntityName
    where TTensor : notnull
{
    private readonly Func<string, string> _onGetFileName;

    public HtmlPageWithTabsEntityBuilder(IContextInfoDataset<ContextInfo, TTensor> contextInfoDataset, HtmlTabbedPageBuilder<TDto> tabbedPageBuilder, Func<string, string> onGetFileName)
        : base(contextInfoDataset, tabbedPageBuilder)
    {
        _onGetFileName = onGetFileName;
    }

    public override async Task BuildAsync(CancellationToken cancellationToken)
    {
        var entitiesList = _contextInfoDataset.GetAll()
            .Where(c => c.ElementType.IsEntityDefinition())
            .Cast<IContextInfo>();

        foreach (var contextInfoItem in entitiesList)
        {
            var classNameWithNameSpace = $"{contextInfoItem.Namespace}.{contextInfoItem.ShortName}";

            var filename = _onGetFileName(classNameWithNameSpace);
            var title = $" Class {contextInfoItem.FullName}";
            var cellData = new ContextInfoKeyContainerEntityName(
                contextInfoList: new List<IContextInfo>() { contextInfoItem },
                contextKey: classNameWithNameSpace);

            await _tabbedPageBuilder.GenerateFileAsync(title, filename, (TDto)cellData, cancellationToken).ConfigureAwait(false);
        }
    }
}
