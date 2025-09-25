using System.IO;
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

        protected override void WriteContentTag(TextWriter writer, IHtmlTagAttributes? attributes, string content = "", bool isEncodable = true)
        {
            //Отрисовываем панель
            Page.HtmlBuilderFactory.Div.With(writer, new HtmlTagAttributes { { "id", "nav-panel" } });

            //Строим контейнер для навигационных ссылок
            BuildPageData(writer, InitialNavigationItem.Url, InitialNavigationItem.Name);

            // располагаем скрипт только здесь и нигде иначе
            Page.HtmlBuilderFactory.Script.Cell(writer, innerHtml: Resources.JsBreadcrumbScript, isEncodable: false);
        }

        public void BuildPageData(TextWriter writer, string pageUrl, string pageName)
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(InitialNavigationItem);

            Page.HtmlBuilderFactory.Div.Cell(writer,
                attributes: new HtmlTagAttributes { { "id", "page-data" }, { "style", "display:none;" } },
                innerHtml: jsonString);
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