using System;
using System.Collections.Generic;

namespace HtmlKit.Model.Tabsheet;

public class BaseHtmlTabsheetDataProvider<DTO> : IHtmlTabsheetDataProvider<DTO>
{
    private readonly Dictionary<Type, IHtmlTabsheetDataModel> _dataModels;

    public IEnumerable<IHtmlTabsheetTabInfoWithDataModelType<DTO>> Tabsheets { get; }

    public BaseHtmlTabsheetDataProvider(Dictionary<Type, IHtmlTabsheetDataModel> dataModels, IEnumerable<HtmlTabsheetTabInfoWithDataModelType<DTO>> tabs)
    {
        _dataModels = dataModels ?? throw new ArgumentNullException(nameof(dataModels));
        Tabsheets = tabs;
    }

    public T GetTabsheetDataModel<T>()
        where T : IHtmlTabsheetDataModel
    {
        if (_dataModels.TryGetValue(typeof(T), out var model))
        {
            return (T)model;
        }

        throw new KeyNotFoundException($"Model of type {typeof(T).Name} not found.");
    }
}
