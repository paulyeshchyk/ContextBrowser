# Руководство по расширению: Регистрация нового Билдера Диаграмм (ContextDiagramBuildersFactory)

Данный документ описывает процесс добавления нового типа построителя диаграмм (ContextDiagramBuilder) в систему, используя механизм внедрения зависимостей (DI) и `IDiagramBuilderRegistration`. Это позволяет расширять функциональность, следуя Принципу Открытости/Закрытости, без изменения кода `ContextDiagramBuildersFactory`.

---

## Процесс регистрации нового Билдера Диаграмм

Для регистрации нового билдера (на примере `TransitionDiagramBuilder`) необходимо выполнить три основных шага:

1.  Определить сущность **Ключа** (`IDiagramBuilderKey`).
2.  Создать **Регистрационную форму** (`IDiagramBuilderRegistration`), которая знает, как построить билдер.
3.  Выполнить **Регистрацию** всех компонентов в контейнере DI.

### Шаг 1: Реализация Сущности Ключа (`IDiagramBuilderKey`)

Каждый билдер должен иметь уникальный тип-ключ, наследующий `IDiagramBuilderKey`. Это обеспечивает типобезопасность.

| Сущность | Интерфейс | Назначение |
| :--- | :--- | :--- |
| **Ключ** | `IDiagramBuilderKey` | Определяет уникальный тип, используемый для идентификации билдера в системе. |

**Пример для `TransitionDiagramBuilder`:**

```csharp
// Файл: TransitionDiagramBuilderKey.cs

public interface IDiagramBuilderKey
{
    string ToKeyString();
}

public record TransitionDiagramBuilderKey : IDiagramBuilderKey
{
    public string ToKeyString() => "context-transition";
}
```
### Шаг 2: Создание Регистрационной Формы (IDiagramBuilderRegistration)

Создайте класс, который реализует обобщенный интерфейс IDiagramBuilderRegistration<TKey>. Этот класс захватывает все постоянные зависимости билдера через свой конструктор DI.

Важно: Необходимо, чтобы IDiagramBuilderRegistration<TKey> наследовал необобщенный интерфейс IDiagramBuilderRegistration для корректной сборки коллекций в DI.

Пример для TransitionDiagramBuilderRegistration:
```c#
// Файл: TransitionDiagramBuilderRegistration.cs

// 1. Обобщенный интерфейс должен наследовать базовый интерфейс (предполагаем, что это сделано)
// public interface IDiagramBuilderRegistration<TKey> : IDiagramBuilderRegistration where TKey : IDiagramBuilderKey { }

public class TransitionDiagramBuilderRegistration : IDiagramBuilderRegistration<TransitionDiagramBuilderKey>
{
    // Статический экземпляр ключа для избежания повторного создания объекта
    private static readonly TransitionDiagramBuilderKey KeyInstance = new TransitionDiagramBuilderKey();

    // Обязательные свойства, требуемые базовым интерфейсом
    public Type KeyType => typeof(TransitionDiagramBuilderKey);
    public string Key => KeyInstance.ToKeyString();

    // !!! ИЗМЕНЕНИЕ: Внедряем Func Factory !!!
    private readonly TransitionDiagramBuilderFactory _builderFactory;

    // Конструктор: DI внедряет специализированную Func Factory
    public TransitionDiagramBuilderRegistration(
        TransitionDiagramBuilderFactory builderFactory)
    {
        _builderFactory = builderFactory;
    }

    // Метод создания: вызывает Func Factory, передавая динамические опции
    public IContextDiagramBuilder CreateBuilder(DiagramBuilderOptions options)
    {
        return _builderFactory(options);
    }
}
```

### Шаг 3: Регистрация в DI-контейнере

Все регистрации выполняются в файле конфигурации сервисов (например, Program.cs или Startup.cs).

#### 3.1. Регистрация вспомогательных зависимостей (ITransitionBuilder)

Зарегистрируйте все компоненты, которые необходимы для создания конечного билдера.
```c#
// Регистрация ITransitionBuilder (для TransitionDiagramBuilder)
services.AddTransient<ITransitionBuilder, OutgoingTransitionBuilder>();
services.AddTransient<ITransitionBuilder, IncomingTransitionBuilder>();
services.AddTransient<ITransitionBuilder, BiDirectionalTransitionBuilder>();
```

#### 3.2. Регистрация Func Factory для Билдера

Зарегистрируйте Func Factory (TransitionDiagramBuilderFactory), которая знает, как создать конечный билдер, используя все его зависимости, кроме динамических options.
```c#
// Определение типа делегата (Фабрики)
public delegate TransitionDiagramBuilder TransitionDiagramBuilderFactory(DiagramBuilderOptions options);

services.AddTransient<TransitionDiagramBuilderFactory>(provider => (options) =>
{
    // Получение всех постоянных зависимостей (DI разрешает их автоматически)
    var transitionBuilders = provider.GetRequiredService<IEnumerable<ITransitionBuilder>>();
    var logger = provider.GetRequiredService<IAppLogger<AppLevel>>();
    var optionsStore = provider.GetRequiredService<IAppOptionsStore>();

    // Создание конечного билдера
    return new TransitionDiagramBuilder(options, transitionBuilders, logger, optionsStore);
});
```

#### 3.3. Регистрация Регистрационной Формы и Фабрики

Зарегистрируйте вашу регистрационную форму под базовым интерфейсом-маркером и саму главную фабрику.
```c#
// КЛЮЧЕВОЙ ШАГ: Регистрация формы под базовым интерфейсом
services.AddSingleton<IDiagramBuilderRegistration, TransitionDiagramBuilderRegistration>();

// Регистрация главной Фабрики
services.AddSingleton<IContextDiagramBuildersFactory, ContextDiagramBuildersFactory>();
```

# Альтернативный способ регистрации билдера
## Шаг 1: Определение специализированной фабрики (Func Factory)

Мы определим тип, который будет создавать TransitionDiagramBuilder и принимать DiagramBuilderOptions.

```c#
// Это тип делегата (фабрики), который мы будем использовать для создания билдера
// Все постоянные зависимости (Logger, Builders) уже внедрены в этот Func через замыкание (closure) в DI.
public delegate TransitionDiagramBuilder TransitionDiagramBuilderFactory(DiagramBuilderOptions options);
```

## Шаг 2: Изменение регистрации в DI (Использование Func Factory)

Мы заменяем регистрацию зависимостей TransitionDiagramBuilder и его самого на регистрацию TransitionDiagramBuilderFactory.

```c#
// --------------------------------------------------------
// Файл регистрации сервисов (Startup/Program.cs)
// --------------------------------------------------------

// Шаг 1: Регистрируем вспомогательные билдеры (ITransitionBuilder)
services.AddTransient<ITransitionBuilder, OutgoingTransitionBuilder>();
services.AddTransient<ITransitionBuilder, IncomingTransitionBuilder>();
services.AddTransient<ITransitionBuilder, BiDirectionalTransitionBuilder>();

// Шаг 2: Регистрируем Func Factory для TransitionDiagramBuilder
// Когда кто-то запрашивает TransitionDiagramBuilderFactory, DI-контейнер создает Func,
// который захватывает (внедряет) все остальные зависимости: Logger, OptionsStore, и коллекцию ITransitionBuilder.
services.AddTransient<TransitionDiagramBuilderFactory>(provider => (options) =>
{
    // Разрешаем все зависимости, кроме options, которые будут переданы в Func
    var transitionBuilders = provider.GetRequiredService<IEnumerable<ITransitionBuilder>>();
    var logger = provider.GetRequiredService<IAppLogger<AppLevel>>();
    var optionsStore = provider.GetRequiredService<IAppOptionsStore>();

    // Создаем экземпляр TransitionDiagramBuilder, передавая 'options'
    return new TransitionDiagramBuilder(options, transitionBuilders, logger, optionsStore);
});

// Шаг 3: Регистрируем TransitionDiagramBuilderRegistration
services.AddSingleton<IDiagramBuilderRegistration, TransitionDiagramBuilderRegistration>();

// ... и сама Фабрика ContextDiagramBuildersFactory
services.AddSingleton<IContextDiagramBuildersFactory, ContextDiagramBuildersFactory>();
```

## Шаг 3: Упрощение TransitionDiagramBuilderRegistration

Теперь этот класс становится максимально простым: он просто запрашивает фабричный делегат и использует его.

```c#
public class TransitionDiagramBuilderRegistration : IDiagramBuilderRegistration<TransitionDiagramBuilderKey>
{
    // ... KeyType, Key (статические или явно определенные) ...
    private static readonly TransitionDiagramBuilderKey KeyInstance = new TransitionDiagramBuilderKey();
    public Type KeyType => typeof(TransitionDiagramBuilderKey);
    public string Key => KeyInstance.ToKeyString();

    // !!! ИЗМЕНЕНИЕ: Внедряем Func Factory !!!
    private readonly TransitionDiagramBuilderFactory _builderFactory;

    public TransitionDiagramBuilderRegistration(
        TransitionDiagramBuilderFactory builderFactory)
    {
        _builderFactory = builderFactory;
    }

    public IContextDiagramBuilder CreateBuilder(DiagramBuilderOptions options)
    {
        // Вызываем фабрику, передавая только runtime-зависимость (options)
        return _builderFactory(options);
    }
}
```
