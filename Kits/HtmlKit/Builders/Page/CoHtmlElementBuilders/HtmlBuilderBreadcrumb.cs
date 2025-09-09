using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Tag;
using HtmlKit.Classes;

namespace HtmlKit.Page;

public static partial class HtmlBuilderFactory
{
    private class HtmlBuilderBreadcrumb : HtmlBuilder
    {
        private readonly BreadcrumbNavigationItem InitialNavigationItem;

        public HtmlBuilderBreadcrumb(BreadcrumbNavigationItem initialNavigationItem) : base("div", "breadcrumb")
        {
            InitialNavigationItem = initialNavigationItem;
        }

        protected override void WriteContentTag(TextWriter writer, IHtmlTagAttributes? attributes, string content = "", bool isEncodable = true)
        {
            //Отрисовываем панель
            HtmlBuilderFactory.Div.With(writer, new HtmlTagAttributes { { "id", "nav-panel" } });

            //Строим контейнер для навигационных ссылок
            BuildPageData(writer, InitialNavigationItem.Url, InitialNavigationItem.Name);

            // располагаем скрипт только здесь и нигде иначе
            HtmlBuilderFactory.Script.Cell(writer, innerHtml: Resources.JsBreadcrumbScript, isEncodable: false);
        }

        public void BuildPageData(TextWriter writer, string pageUrl, string pageName)
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(InitialNavigationItem);

            HtmlBuilderFactory.Div.Cell(writer,
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