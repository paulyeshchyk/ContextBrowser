using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using HtmlKit.Model.Containers;

namespace HtmlKit.Builders.Page.Tabs.CoBuilders;

public class HtmlPageWithTabsNamespaceEntityBuilder<DTO, TTensor> : HtmlPageWithTabsBuilder<DTO, TTensor>
    where DTO : ContextInfoKeyContainerNamespace
    where TTensor : notnull
{
    private readonly Func<string, string> _onGetFileName;

    public HtmlPageWithTabsNamespaceEntityBuilder(IContextInfoDataset<ContextInfo, TTensor> contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder, Func<string, string> onGetFileName)
        : base(contextInfoDataset, tabbedPageBuilder)
    {
        _onGetFileName = onGetFileName;
    }

    public override Task BuildAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var entitiesList = _contextInfoDataset.GetAll()
                .Where(c => (c.ElementType == ContextInfoElementType.@class) || (c.ElementType == ContextInfoElementType.@struct) || (c.ElementType == ContextInfoElementType.record))
                .Cast<IContextInfo>();

            var namespaces = entitiesList.Select(e => e.Namespace).Distinct();
            foreach (var ns in namespaces)
            {
                var filtered = entitiesList.Where(e => e.Namespace.Equals(ns));

                var filename = _onGetFileName(ns);
                var title = $" Namespace {ns}";
                var cellData = new ContextInfoKeyContainerNamespace(
                    contextInfoList: filtered,
                    contextKey: ns);

                _tabbedPageBuilder.GenerateFile(title, filename, (DTO)cellData);
            }
        }
        , cancellationToken);
    }
}
