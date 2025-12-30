using System.Collections.Generic;
using ContextKit.Model;

namespace HtmlKit.Matrix;

public interface IHtmlMatrix
{
    public List<ILabeledValue> rows { get; }

    public List<ILabeledValue> cols { get; }

    public IHtmlMatrix Transpose();
}