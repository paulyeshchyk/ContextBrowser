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
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Html;

#warning remove domainperaction
public class HtmlPageWithTabsNamespaceEntityBuilder<DTO, TKey> : HtmlPageWithTabsBuilder<DTO, TKey>
    where DTO : NamespacenameContainer
    where TKey : DomainPerActionTensor
{
    private readonly Func<string, string> _onGetFileName;

    public HtmlPageWithTabsNamespaceEntityBuilder(IContextInfoDataset<ContextInfo, TKey> contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder, Func<string, string> onGetFileName)
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
                var cellData = new NamespacenameContainer(
                    contextInfoList: filtered,
                    contextKey: ns);

                _tabbedPageBuilder.GenerateFile(title, filename, (DTO)cellData);
            }
        }
        , cancellationToken);
    }
}
