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
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;
using TensorKit.Model;

namespace ExporterKit.Html;

public class HtmlPageWithTabsEntityBuilder<DTO, TKey> : HtmlPageWithTabsBuilder<DTO, TKey>
    where DTO : EntitynameContainer
    where TKey : notnull
{
    private readonly Func<string, string> _onGetFileName;

    public HtmlPageWithTabsEntityBuilder(IContextInfoDataset<ContextInfo, TKey> contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder, Func<string, string> onGetFileName)
        : base(contextInfoDataset, tabbedPageBuilder)
    {
        _onGetFileName = onGetFileName;
    }

    public override Task BuildAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var entitiesList = _contextInfoDataset.GetAll()
                .Where(c => (c.ElementType == ContextInfoElementType.@class) || (c.ElementType == ContextInfoElementType.@struct) || (c.ElementType == ContextInfoElementType.@record) || (c.ElementType == ContextInfoElementType.@interface))
                .Cast<IContextInfo>();

            foreach (var contextInfoItem in entitiesList)
            {
                var filename = _onGetFileName(contextInfoItem.FullName);
                var title = $" Class {contextInfoItem.FullName}";
                var cellData = new EntitynameContainer(
                    contextInfoList: new List<IContextInfo>() { contextInfoItem },
                    contextKey: contextInfoItem.FullName);

                _tabbedPageBuilder.GenerateFile(title, filename, (DTO)cellData);
            }
        });
    }
}
