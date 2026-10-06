# DONE

Реализовано и работает. Исторический журнал, не удаляем.

## 1 Kits

### 1.1 LoggerKit
- **Статус:** done
- **Комментарий:** уровни, отступы, зависимости между `AppLevel`, консольный writer
- `IAppLogger<T>` / `AppLogger<T>` / `IndentedAppLogger<T>`
- `AppLoggerLevelStore<T>` — уровни по `TAppLevel`
- `ConsoleLogWriter` — вывод в консоль
- `AppLoggerFactory.DefaultIndentedLogger<T>()`

### 1.2 CommandlineKit
- **Статус:** done
- **Комментарий:** парсинг `--key value`, help, вложенные пути
- `CommandLineParser.TryParse<T>`
- `CommandLineHelpProducer` / `HelpGenerator`
- `CommandLineNodeIterator.SetNestedPropertyValue`
- `CommandlineArgumentsParserService`

### 1.3 ContextBrowserKit
- **Статус:** done
- **Комментарий:** FileUtils, PathAnalyzer, PathFilter, LRUCache
- `Extensions/` — 6 классов
- `Options/` — Export, Import, HtmlTable, IAppOptionsStore
- `Log/` — LogObject, LogWriter, LogLevel
- `LRUCache<TKey, TValue>` — thread-safe

### 1.4 TensorKit
- **Статус:** done
- **Комментарий:** TensorBase, DomainPerActionTensor, Builder, Factory
- `TensorBase` / `ITensor`
- `DomainPerActionTensor` — ключ `(Action, Domain)`
- `TensorBuilder` — нормализация порядка измерений
- `TensorFactory<TKey>`

### 1.5 GraphKit
- **Статус:** partial
- **Комментарий:** Walker, DomainWalker, ItemWalker, DfsWalker_Traversal
- [x] `Walker<T>` — базовый обходчик
- [x] `DomainWalker` — обход по `Domains.Contains(domain)`
- [x] `ItemWalker` — обход по `References` + domain items
- [x] `DfsWalker_Traversal` — generic DFS
- [ ] TODO: итеративный DFS вместо рекурсивного (риск `StackOverflow`)
  - См. [`todo.md`](todo.md), узел `1.5.4`

## 2 Semantic Core

### 2.1 ContextKit
- **Статус:** done
- **Комментарий:** модель, классификаторы, стратегии, коллекторы, кэш, relations, dataset, naming
- `Model/ContextInfo.cs` — центральная модель
- `Model/Classifier/` — Context, Empty, Fake, Tensor classifiers
- `ContextData/Comment/` — ContextStrategy, CoverageStrategy, ValidationDecorator
- `Model/Collector/` — три коллектора
- `Model/CacheManager/` — in-memory + file cache
- `Model/Relations/` — RelationManager + 6 injectors
- `Model/WordTensorBuildStrategy/` — 4 стратегии
- `Model/Dataset/` — Dataset, DatasetBuilder, Filler
- `ContextData/Naming/NamingProcessor` — ~40 методов

### 2.2 SemanticKit
- **Статус:** done
- **Комментарий:** абстракции, pipeline, signature
- `Model/IContextInfoBuilder` / `ContextInfoBuilder` / Dispatcher
- `Model/SemanticSyntaxRouter` / `ISyntaxParser`
- `Model/SemanticCompilationMap` / `CompilationMap` / View
- `Parsers/File/FileParserPipeline` — фазовый pipeline
- `Parsers/Strategy/Declaration/DeclarationFileParser` — Phase 1
- `Parsers/Strategy/Invocation/InvocationFileParser` — Phase 2
- `Model/Signature/` — ISignature, SignatureChainFactory

### 2.3 RoslynKit
- **Статус:** partial (C# покрыт)
- **Комментарий:** Assembly, syntax parsers, context builders, converters, lookup, signature, invocation
- [x] `Assembly/` — 8 сервисов
- [x] `Syntax/Parsers/` — 8 C#-парсеров узлов
- [x] `Phases/ContextInfoBuilder/` — 8 C#-билдеров
- [x] `Converters/` — 3 конвертера
- [x] `Lookup/` — 4 handler-а + factory
- [x] `Signature/` — chain + regex + builder
- [x] `Assembly/Strategy/Invocation/` — 5 сервисов
- [ ] TODO: TypeScript-парсер
  - См. [`todo.md`](todo.md), узел `2.3.9`

## 3 Export

### 3.1 UmlKit
- **Статус:** partial
- **Комментарий:** PlantUmlSpecification, builders, renderers, managers
- [x] `PlantUmlSpecification/` — ~25 классов
- [x] `Builders/` — ~15 классов
- [x] `Renderers/` — 13 реализаций
- [x] `Managers/` — 5 классов (sequence activation)
- [ ] TODO: `UmlTransitionRendererHierarchial` (заготовка)
  - Узел `3.1.3.14`
- [ ] TODO: `UmlDiagramMindmap` (большинство методов `NotImplementedException`)
  - Узел `3.1.1.5`

### 3.2 HtmlKit
- **Статус:** done
- **Комментарий:** Builders, Document, Matrix, Tabsheet, Pages
- `Builders/` — ~15 классов (Html, Div, A, Table, ...)
- `Document/` — `HtmlTensorWriter`, PageProducer, CellStyleBuilder
- `Matrix/` — IHtmlMatrix + 3 реализации
- `Model/Tabsheet/` — модель табов
- `Pages/` — `HtmlTabbedPageBuilder`, `HtmlTabsheetBuilder`, TabRegistration

### 3.3 ExporterKit
- **Статус:** partial
- **Комментарий:** Html pages (~7 компиляторов), Uml compilers (~20), Csv, DiagramCompileOptions
- [x] `Html/Pages/` — 7 компиляторов + оркестратор
- [x] `Uml/DiagramCompiler/` — ~20 компиляторов
- [x] `Uml/DiagramCompileOptions/` — 4 стратегии
- [x] `Csv/` — `CsvGenerator`, `ContextInfoCsvExporter`
- [ ] TODO: `DependencyDiagramBuilder`, `MethodFlowDiagramBuilder`, `MethodOnlyDiagramBuilder`

## 4 Application

### 4.1 ContextBrowser
- **Статус:** partial
- **Комментарий:** Console и WebApp режимы, HostConfigurator, AppOptions, MainService
- [x] `Program.cs` — entry point
- [x] `ConsoleRunner` — режим Console
- [x] `WebAppRunner` — режим WebApp + Kestrel
- [x] `HostConfigurator` — DI (~100 регистраций)
- [x] `AppOptions` + JsonConverters + Preset-проекты
- [x] `MainService` + pipeline
- [x] `ContextInfoProvider` — dataset / indexer / mapper
- [ ] TODO: `#warning args to be checked` в ConsoleRunner и WebAppRunner
  - Узлы `4.1.2.1`, `4.1.2.2`

### 4.2 CustomServers
- **Статус:** done
- **Комментарий:** Windows / macOS, HTTP + PlantUML
- `CustomEnvironment.CopyResources` / `RunServers`
- `WindowsServer` / `MacOsServer`
- `CustomServerDetector.GetOSPlatform`
- `ProcessInfoFactory` для каждой ОС

## 5 Samples / Tests

### 5.1 ContextSamples
- **Статус:** done
- **Комментарий:** 6 сценариев (S1–S6)
- S1 Actor → Transition (простой вызов)
- S2 A.Foo → B.Bar (invocation)
- S3 FlowOrchestrator ↔ AnotherService (bidirectional)
- S4 TaskService + Validator / Builder / Notifier / Repository
- S5 BrokenOrchestra (broken contexts)
- S6 Alpha → Beta → Gamma (циклические ссылки)

### 5.2 ContextBrowserTests
- **Статус:** partial
- **Комментарий:** 4 файла тестов
- [x] `LRUCacheTests` — 5 тестов
- [x] `ContextInfoTraversalTests` — DomainWalker
- [x] `RoslynCodeParserTests` — 2 теста
- [ ] TODO: `Test1` — заглушки, надо наполнить
  - Узел `5.2.4`

## Сводка

| Категория | Done | Partial | Todo |
|-----------|------|---------|------|
| Kits | 4 | 1 | 0 |
| Semantic Core | 2 | 1 | 0 |
| Export | 1 | 2 | 0 |
| Application | 1 | 1 | 0 |
| Samples / Tests | 1 | 1 | 0 |
| **Итого** | **9** | **6** | **0** |