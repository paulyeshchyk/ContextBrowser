using System.Collections.Generic;
using System.IO;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using TensorKit.Model;

namespace HtmlKit.Writer;

public interface IHtmlContentInjector<TTensor>
    where TTensor : notnull
{
    string Inject(TTensor container, int cnt);
}
