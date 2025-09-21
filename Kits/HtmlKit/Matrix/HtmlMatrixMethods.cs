using System.Collections.Generic;
using System.Linq;
using HtmlKit.Model.Containers;
using TensorKit.Model.DomainPerAction;

namespace HtmlKit.Matrix;

public class HtmlMatrixMethods : IHtmlMatrix
{
    public List<object> rows { get; }

    public List<object> cols { get; }

    public ContextInfoKeyContainerTensor<DomainPerActionTensor> OwnerTensor { get; }

    public HtmlMatrixMethods(IEnumerable<int> methods, ContextInfoKeyContainerTensor<DomainPerActionTensor> ownerTensor)
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