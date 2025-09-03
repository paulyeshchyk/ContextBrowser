namespace HtmlKit.Page;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;

/// <summary>
/// Регистрация одной вкладки: связывает контракт модели (DataModelType),
/// конкретную модель (Model) и уже готовую HtmlTabsheetTabInfoWithDataModelType (Tab).
/// </summary>
public interface IHtmlTabRegistration
{
    /// <summary>Тип контракта модели (ключ в словаре провайдера).</summary>
    Type DataModelType { get; }

    /// <summary>Экземпляр модели (реализация IHtmlTabsheetDataModel).</summary>
    IHtmlTabsheetDataModel Model { get; }

    /// <summary>Готовая информация о вкладке, содержащая делегат рендера и прочее.</summary>
    HtmlTabsheetTabInfoWithDataModelType Tab { get; }
}

/// <summary>
/// Статическая фабрика для простого и декларативного создания регистраций вкладок.
/// Использование:
/// TabRegistration.For<MyModelImpl, IMyModelContract>(..., modelInstance, (writer, model, dto) => { ... });
/// </summary>
public static class TabRegistration
{
    /// <summary>
    /// Создать регистрацию вкладки, где:
    /// TModel — конкретная реализация модели (реализует TContract и IHtmlTabsheetDataModel),
    /// TContract — интерфейс-контракт, под которым вкладка будет доступна провайдеру.
    /// </summary>
    public static IHtmlTabRegistration For< TContract>(
        string tabId,
        string caption,
        bool isActive,
        TContract model,
        Action<TextWriter, TContract, HtmlContextInfoDataCell> build)
        // where TModel : class, TContract
        where TContract : IHtmlTabsheetDataModel
    {
        if (string.IsNullOrWhiteSpace(tabId)) throw new ArgumentException("tabId required", nameof(tabId));
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (build == null) throw new ArgumentNullException(nameof(build));

        var tabsheetTabInfo = new TabsheetTabInfo(tabId: tabId, caption: caption);

        var htmlTab = new HtmlTabsheetTabInfo
        (
            info: tabsheetTabInfo,
            buildHtmlTab: (writer, tabsheetProvider, dto) =>
            {
                var resolved = tabsheetProvider.GetTabsheetDataModel<TContract>();
                build(writer, (TContract)resolved, dto);
            },
            isActive: isActive
        );

        return new HtmlTabRegistrationImpl(
            dataModelType: typeof(TContract),
            model: model,
            tab: new HtmlTabsheetTabInfoWithDataModelType(htmlTab, typeof(TContract))
        );
    }

    private sealed class HtmlTabRegistrationImpl : IHtmlTabRegistration
    {
        public Type DataModelType { get; }
        public IHtmlTabsheetDataModel Model { get; }
        public HtmlTabsheetTabInfoWithDataModelType Tab { get; }

        public HtmlTabRegistrationImpl(Type dataModelType, IHtmlTabsheetDataModel model, HtmlTabsheetTabInfoWithDataModelType tab)
        {
            DataModelType = dataModelType ?? throw new ArgumentNullException(nameof(dataModelType));
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Tab = tab ?? throw new ArgumentNullException(nameof(tab));
        }
    }
}

/// <summary>
/// Провайдер, построенный из набора регистраций. Внутренне адаптируется к BaseHtmlTabsheetDataProvider
/// </summary>
public sealed class ComposableTabsheetDataProvider : BaseHtmlTabsheetDataProvider
{
    public ComposableTabsheetDataProvider(IEnumerable<IHtmlTabRegistration> registrations)
        : base(
            dataModels: (registrations ?? throw new ArgumentNullException(nameof(registrations)))
                         .ToDictionary(r => r.DataModelType, r => r.Model),
            tabs: registrations.Select(r => r.Tab)
          )
    {
    }
}
