---
title: Kits
sectionType: Page
pureTitle: Kits
sectionIndex: ""
---

# Карта Kit-ов

12 библиотек проекта. Каждый Kit — изолированная зона ответственности.

## Список

| #   | Kit                   | Назначение                                | Зависит от                                              |
| --- | --------------------- | ----------------------------------------- | ------------------------------------------------------- |
| 1   | **LoggerKit**         | логирование с уровнями и отступами        | ContextBrowserKit                                       |
| 2   | **CommandlineKit**    | парсинг CLI, help-генерация               | ContextBrowserKit                                       |
| 3   | **ContextBrowserKit** | базовые утилиты, опции, лог-примитивы     | TensorKit                                               |
| 4   | **TensorKit**         | многомерные тензоры-ключи                 | —                                                       |
| 5   | **GraphKit**          | обходчики графов (Walker, DFS)            | ContextKit                                              |
| 6   | **ContextKit**        | модель `ContextInfo`, классификаторы, кэш | ContextBrowserKit, TensorKit, LoggerKit, CommandlineKit |
| 7   | **SemanticKit**       | абстракции парсинга (языко-агностичные)   | ContextKit, LoggerKit                                   |
| 8   | **RoslynKit**         | конкретный парсер C# через Roslyn         | SemanticKit, ContextKit, LoggerKit                      |
| 9   | **UmlKit**            | PlantUML-модель и рендереры               | ContextKit, GraphKit, TensorKit                         |
| 10  | **HtmlKit**           | HTML-билдеры и writer-ы                   | ContextKit, GraphKit, TensorKit                         |
| 11  | **ExporterKit**       | оркестрация экспорта                      | UmlKit, HtmlKit, SemanticKit, ContextKit                |
| 12  | **CustomServers**     | запуск HTTP и PlantUML серверов           | — (только BCL)                                          |

## Детали по Kit-ам

### 1. LoggerKit

Логирование с уровнями и иерархией.

- `IAppLogger<T>` — основной интерфейс
- `AppLogger<T>` — базовая реализация
- `IndentedAppLogger<T>` — с отступами и зависимостями между AppLevel
- `AppLoggerLevelStore<T>` — уровни по `TAppLevel`
- `ConsoleLogWriter` — вывод в консоль
- `LogConfiguration<TAppLevel, TLogLevel>` — конфигурация уровней
- `AppLoggerFactory.DefaultIndentedLogger<T>()` — фабричный метод

**Ключевая идея:** уровни задаются per-AppLevel (`App`, `R_Syntax`, `P_Uml`, ...),
а зависимости между уровнями дают красивый вывод с отступами.

### 2. CommandlineKit

Парсинг `--key value` в типизированные объекты.

- `CommandLineParser.TryParse<T>` — парсинг
- `CommandLineHelpProducer` / `HelpGenerator` — генерация справки
- `CommandLineArgumentAttribute` — атрибут для полей
- `CommandLineNodeIterator.SetNestedPropertyValue` — вложенные пути (`--a.b.c`)

**Ключевая идея:** поддерживаются вложенные свойства через точки,
значения конвертируются (`IEnumerable<string>`, `Enum`, primitives).

### 3. ContextBrowserKit

Базовые утилиты, нужные всем.

- `Extensions/` — FileUtils, PathAnalyzer, PathFilter, StringExtensions, DeepClone
- `Options/` — Export, Import, Log, HtmlTable, IAppOptionsStore
- `Log/` — LogObject, LogWriter, LogLevel, LogLevelNode
- `Filters/FilterPatterns` — фильтры путей
- `LRUCache<TKey, TValue>` — thread-safe LRU

**Ключевая идея:** ничего специфичного, только общеупотребимое.

### 4. TensorKit

Многомерные тензоры-ключи.

- `TensorBase` — абстрактный тензор
- `DomainPerActionTensor` — конкретный `(Action, Domain)`
- `TensorBuilder` — нормализация порядка измерений (`Standard` / `Transposed`)
- `TensorFactory<TKey>` — создание тензоров

**Ключевая идея:** тензор — это ключ в `Dictionary`. Матрица `Action × Domain`
это `Dictionary<DomainPerActionTensor, List<ContextInfo>>`.

### 5. GraphKit

Обходчики графов.

- `Walker<T>` — базовый обходчик с `Visited`
- `DomainWalker` — обход по `Domains.Contains(domain)`
- `ItemWalker` — обход по `References` + domain items
- `DfsWalker_Traversal` — generic DFS

**Ключевая идея:** обходы графа для генерации mindmap и drill-down.
⚠️ `DfsWalker_Traversal` рекурсивный — риск `StackOverflow` на больших графах.

### 6. ContextKit

Ядро модели — `ContextInfo` и всё вокруг него.

- `Model/ContextInfo.cs` — центральная модель
- `Model/Classifier/` — `ContextClassifier`, fake/empty classifiers
- `ContextData/Comment/` — стратегии парсинга комментариев
- `Model/Collector/` — коллекторы с thread-safe
- `Model/CacheManager/` — кэш на файл + in-memory
- `Model/Factory/` — фабрики и адаптеры
- `Model/Relations/` — injectors графа
- `Model/WordTensorBuildStrategy/` — 4 стратегии раскладки в тензор
- `ContextData/Naming/NamingProcessor` — имена файлов/URL

**Ключевая идея:** ContextInfo — это то, чем оперирует всё остальное.
ContextKit не знает про Roslyn, только про модель.

### 7. SemanticKit

Абстракции парсинга, языко-агностичные.

- `Model/IContextInfoBuilder<TContext>` — интерфейс билдера
- `Model/ContextInfoBuilder<TContext, TSyntax, TWrapper>` — базовый
- `Model/ContextInfoBuilderDispatcher<TContext>` — маршрутизатор
- `Model/ISyntaxParser<TContext>` / `SemanticSyntaxRouter`
- `Parsers/File/FileParserPipeline` — фазовая обработка
- `Parsers/Strategy/Declaration/DeclarationFileParser` — Phase 1
- `Parsers/Strategy/Invocation/InvocationFileParser` — Phase 2
- `Model/Signature/` — `ISignature`, `SignatureChainFactory`

**Ключевая идея:** это контракт. RoslynKit и (будущий) TypeScriptKit
реализуют интерфейсы SemanticKit.

### 8. RoslynKit

Конкретный парсер C# через Roslyn.

- `Assembly/` — загрузка сборок, компиляция
- `Syntax/Parsers/` — C#-специфичные парсеры (по типу узла)
- `Phases/ContextInfoBuilder/` — C#-билдеры контекста
- `Lookup/` — chain of responsibility для поиска символов
- `Signature/SignatureBuilder/` — построение сигнатур
- `Signature/SignatureParser/` — парсинг сигнатур через regex-цепочку
- `Assembly/Strategy/Invocation/` — resolution вызовов

**Ключевая идея:** всё, что знает про `SyntaxTree`, `SemanticModel`, `ISymbol`,
живёт здесь.

### 9. UmlKit

PlantUML-модель и рендереры.

- `PlantUmlSpecification/` — POCO для элементов PlantUML
- `Builders/` — композиция диаграмм
- `Renderers/` — конкретные рендереры (class/mindmap/transition)
- `Managers/` — управление стеком активаций (для sequence)
- `Infrastructure/Options/` — настройки диаграмм

**Ключевая идея:** UmlKit не знает про Roslyn. Он работает с `ContextInfo` и
строит PUML-строки.

### 10. HtmlKit

HTML-билдеры и writer-ы.

- `Builders/Core/` — базовая инфраструктура
- `Builders/Page/` — конкретные билдеры (Div, A, Table, ...)
- `Builders/Page/Tabs/` — табы, регистрация, композиция
- `Document/` — `HtmlTensorWriter`, `HtmlPageProducer`
- `Matrix/` — `IHtmlMatrix` и реализации
- `Model/Tabsheet/` — модель табов

**Ключевая идея:** `HtmlTensorWriter<TTensor>` — универсальный writer для матриц.
Он принимает на вход `IHtmlMatrix`, `IHtmlDataCellBuilder`, `IHtmlHrefManager` —
и всё, что нужно для отрисовки — через DI.

### 11. ExporterKit

Оркестрация экспорта.

- `Html/Pages/` — 7 компиляторов страниц + оркестратор
- `Uml/DiagramCompiler/` — ~20 компиляторов диаграмм
- `Uml/DiagramCompileOptions/` — стратегии compile options
- `Csv/` — CSV-генератор heatmap
- `DotDiagram/` — вспомогательный экспорт в DOT

**Ключевая идея:** ExporterKit знает про UmlKit, HtmlKit, SemanticKit.
Это единственный Kit, который видит всё сразу.

### 12. CustomServers

Запуск HTTP и PlantUML серверов.

- `CustomEnvironment` — `CopyResources`, `RunServers`
- `WindowsServer` / `MacOsServer` — платформо-специфичные
- HTTP-сервер (`http-server -p 5500`)
- PlantUML picoweb (`java -jar ... -picoweb`)
- `ProcessInfoFactory` — фабрики ProcessStartInfo

**Ключевая идея:** всё, что связано с внешними процессами — здесь.
Ничего из BCL, кроме `Process`, не используется.

## Быстрая навигация по частым задачам

| Задача                               | Иди в                                                                 |
| ------------------------------------ | --------------------------------------------------------------------- |
| Поменять формат `// context:`        | `ContextKit/ContextData/Comment/Stategies/ContextStrategy.cs`         |
| Добавить новый action                | `AppOptions.Classifier.StandardActions`                               |
| Добавить новую стратегию раскладки   | `ContextKit/Model/WordTensorBuildStrategy/`                           |
| Добавить новый тип диаграммы         | `ExporterKit/Uml/DiagramCompiler/` + регистрация в `HostConfigurator` |
| Добавить вкладку в HTML              | `ExporterKit/Html/Pages/CoCompiler/TabsheetFactory.cs`                |
| Изменить цвет heatmap                | `HtmlKit/Helpers/HeatmapColorBuilder.cs`                              |
| Изменить логику парсинга комментария | `ContextKit/ContextData/Comment/`                                     |
| Добавить новый language parser       | `SemanticKit` + новый Kit                                             |

## Правила для Kit-ов

1. **Каждый Kit самодостаточен.** Не должно быть «полу-Kit», которое
   работает только с одним потребителем.
2. **Слои не нарушаются.** См. [`10-architecture/02-dependencies.md`](../10-architecture/02-dependencies.md).
3. **Kit не зависит от приложения.** Только приложение зависит от Kit-ов.
4. **Публичное API — минимально.** Всё, что не нужно потребителю,
   должно быть `internal`.