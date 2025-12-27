using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderBreadcrumb : HtmlBuilder
    {
        private readonly BreadcrumbNavigationItem InitialNavigationItem;

        public HtmlBuilderBreadcrumb(BreadcrumbNavigationItem initialNavigationItem) : base("div", "breadcrumb")
        {
            InitialNavigationItem = initialNavigationItem;
        }

        protected override async Task WriteContentTagAsync(TextWriter writer, IHtmlTagAttributes? attributes, string content = "", bool isEncodable = true, CancellationToken cancellationToken = default)
        {
            //Отрисовываем панель
            await Page.HtmlBuilderFactory.Div.WithAsync(writer, new HtmlTagAttributes { { "id", "nav-panel" } }, cancellationToken: cancellationToken).ConfigureAwait(false);

            //Строим контейнер для навигационных ссылок
            await BuildPageData(writer, InitialNavigationItem.Url, InitialNavigationItem.Name, cancellationToken).ConfigureAwait(false);

            // располагаем скрипт только здесь и нигде иначе
            await Page.HtmlBuilderFactory.Script.CellAsync(writer, innerHtml: Resources.JsBreadcrumbScript, isEncodable: false, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task BuildPageData(TextWriter writer, string pageUrl, string pageName, CancellationToken cancellationToken)
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(InitialNavigationItem);

            await Page.HtmlBuilderFactory.Div.CellAsync(writer,
                attributes: new HtmlTagAttributes { { "id", "page-data" }, { "style", "display:none;" } },
                innerHtml: jsonString, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

public class BreadcrumbNavigationItem
{
    public string Url { get; set; }

    public string Name { get; set; }

    public BreadcrumbNavigationItem(string url, string name)
    {
        Url = url;
        Name = name;
    }
}