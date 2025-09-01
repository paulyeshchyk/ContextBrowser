using System;
using System.Collections.Generic;
using ContextBrowserKit.Options.Export;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;

namespace HtmlKit.Model.Tabsheet;

public class BaseHtmlTabsheetDataProvider : IHtmlTabsheetDataProvider
{
    private readonly Dictionary<Type, IHtmlTabsheetDataModel> _dataModels;
    public IEnumerable<HtmlTabsheetTabInfoWithDataModelType> Tabsheets { get; }

    public BaseHtmlTabsheetDataProvider(Dictionary<Type, IHtmlTabsheetDataModel> dataModels, IEnumerable<HtmlTabsheetTabInfoWithDataModelType> tabs)
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
