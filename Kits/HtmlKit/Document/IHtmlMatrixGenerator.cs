using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;

namespace HtmlKit.Document;

public interface IHtmlMatrixGenerator
{
    IHtmlMatrix Generate();
}
