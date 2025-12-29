using System.Collections.Generic;
using System.Linq;
using HtmlKit.Model.Containers;

namespace HtmlKit.Matrix;

public class HtmlMatrixMethods<TDataTensor> : IHtmlMatrix
    where TDataTensor : notnull
{
    public List<object> rows { get; }

    public List<object> cols { get; }

    public ContextInfoKeyContainerTensor<TDataTensor> OwnerTensor { get; }

    public HtmlMatrixMethods(IEnumerable<int> methods, ContextInfoKeyContainerTensor<TDataTensor> ownerTensor)
    {
        OwnerTensor = ownerTensor;
        rows = methods.Cast<object>().ToList();

        // У одноколоночной таблицы всего одна колонка с заголовком
        cols = new List<object> { ownerTensor };
    }

    // Транспонирование для одноколоночной таблицы не имеет смысла,
    // но метод всё равно должен быть реализован.
    public IHtmlMatrix Transpose()
    {
        return new HtmlMatrixDomainPerAction(cols, rows);
    }
}