using System.Collections.Generic;

namespace HtmlKit.Matrix;

public interface IHtmlMatrix
{
    public List<object> rows { get; }

    public List<object> cols { get; }

    public IHtmlMatrix Transpose();
}
