using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public abstract class HtmlBuilderPumlBase : HtmlBuilder
    {
        private const string SRenderPlantumlJs = "<script type=\"module\">import enableElement from \"./../render-plantuml.js\";enableElement();</script>";

        public string RendererMode { get; set; }

        public string Server { get; set; }

        protected HtmlBuilderPumlBase(string server, string? rendererMode = null)
            : base("render-plantuml", string.Empty)
        {
            Server = server;
            RendererMode = string.IsNullOrEmpty(rendererMode) ? "svg" : rendererMode;
        }

        public override Task StartAsync(TextWriter sb, IHtmlTagAttributes? attrs = null, CancellationToken cancellationToken = default)
        {
            sb.Write(SRenderPlantumlJs);
            return Task.CompletedTask;
        }

        public override Task EndAsync(TextWriter sb, CancellationToken cancellationToken = default)
        {
            // nothing to do
            return Task.CompletedTask;
        }
    }

    public class HtmlBuilderPumlContent : HtmlBuilderPumlBase
    {
        public string Content { get; set; }

        public HtmlBuilderPumlContent(string content, string server, string? rendererMode = null)
            : base(server, rendererMode)
        {
            Content = content;
        }

        public override async Task CellAsync(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true, CancellationToken cancellationToken = default)
        {
            var rendererAttributes = new HtmlTagAttributes
            {
                { "rendererMode", RendererMode },
                { "server", Server }
            };

            await WriteContentTagAsync(sb, rendererAttributes, Content, isEncodable: false, cancellationToken).ConfigureAwait(false);
        }
    }

    public class HtmlBuilderPumlReference : HtmlBuilderPumlBase
    {
        public string Src { get; set; }

        public HtmlBuilderPumlReference(string src, string server, string? rendererMode = null)
            : base(server, rendererMode)
        {
            Src = src;
        }

        public override async Task CellAsync(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true, CancellationToken cancellationToken = default)
        {
            var rendererAttributes = new HtmlTagAttributes
            {
                { "rendererMode", RendererMode },
                { "server", Server },
                { "src", Src }
            };

            await WriteContentTagAsync(sb, rendererAttributes, string.Empty, isEncodable: true, cancellationToken).ConfigureAwait(false);
        }
    }
}