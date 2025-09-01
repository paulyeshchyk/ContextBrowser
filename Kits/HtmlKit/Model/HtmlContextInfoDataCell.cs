using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Matrix;

namespace HtmlKit.Model;

public class HtmlContextInfoDataCell
{
    public ContextInfoDataCell DataCell { get; init; }
    public IEnumerable<ContextInfo> Methods { get; init; }

    public HtmlContextInfoDataCell(ContextInfoDataCell dataCell, IEnumerable<ContextInfo> methods)
    {
        DataCell = dataCell;
        Methods = methods;
    }
}
