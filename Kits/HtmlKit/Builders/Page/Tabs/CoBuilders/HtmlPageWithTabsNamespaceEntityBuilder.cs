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
    private readonly Func<string, string> _onGetTitle;

    public HtmlPageWithTabsNamespaceEntityBuilder(IContextInfoDataset<ContextInfo, TTensor> contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder, Func<string, string> onGetFileName, Func<string, string> onGetTitle)
        : base(contextInfoDataset, tabbedPageBuilder)
    {
        _onGetFileName = onGetFileName;
        _onGetTitle = onGetTitle;
    }

    public override async Task BuildAsync(CancellationToken cancellationToken)
    {
        var entitiesList = _contextInfoDataset.GetAll()
            .Where(c => c.ElementType.IsEntityDefinition() || c.MethodOwnedByItSelf == true)
            .Cast<IContextInfo>()
            .ToList();

        var namespaces = entitiesList.Select(e => e.Namespace).Distinct();
        foreach (var ns in namespaces)
        {
            var filtered = entitiesList.Where(e => e.Namespace.Equals(ns));

            var filename = _onGetFileName(ns);
            var title = _onGetTitle(ns);
            var cellData = new ContextInfoKeyContainerNamespace(
                contextInfoList: filtered,
                contextKey: ns);

            await _tabbedPageBuilder.GenerateFileAsync(title, filename, (DTO)cellData, cancellationToken).ConfigureAwait(false);
        }
    }
}
