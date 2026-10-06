# Pipeline парсинга

## Полный путь от файла до отчёта

### ASCII

```
   Пользователь
       │
       ▼
   ┌──────────────────────────────────────────────────┐
   │  MainService.RunAsync                            │
   └────────┬─────────────────────────────────────────┘
            │
            ▼
   ┌──────────────────────────────────────────────────┐
   │  ContextInfoDatasetProvider.GetDatasetAsync      │
   └────────┬─────────────────────────────────────────┘
            │
            ▼
   ┌──────────────────────────────────────────────────┐
   │  ParsingOrchestrator.ParseAsync                  │
   └────────┬─────────────────────────────────────────┘
            │
            ▼
   ┌──────────────────────────────────────────────────┐
   │  ContextInfoCacheService.GetOrParseAndCacheAsync │
   │  ┌──────────────────────────────────────┐        │
   │  │  кэш пуст?                           │        │
   │  │  ├── да:  FileParserPipeline         │        │
   │  │  └── нет: return cache               │        │
   │  └──────────────────────────────────────┘        │
   └────────┬─────────────────────────────────────────┘
            │
            ▼
   ┌──────────────────────────────────────────────────┐
   │  FileParserPipeline (SortedList<int,IFileParser>)│
   │  ┌──────────────────────────────────────────┐    │
   │  │  Phase 1: DeclarationFileParser          │    │
   │  │    └── RoslynKit → ContextInfoBuilder    │    │
   │  ├──────────────────────────────────────────┤    │
   │  │  Phase 2: InvocationFileParser           │    │
   │  │    └── RoslynInvocationBuilder           │    │
   │  └──────────────────────────────────────────┘    │
   └────────┬─────────────────────────────────────────┘
            │
            ▼
   ┌──────────────────────────────────────────────────┐
   │  ContextInfoDatasetBuilder.BuildAsync            │
   │  ├── ContextInfoFiller (order MinValue)          │
   │  └── ContextInfoFillerEmptyData (order MaxValue) │
   └────────┬─────────────────────────────────────────┘
            │
            ▼
   ┌──────────────────────────────────────────────────┐
   │  MainService                                     │
   │  ├── CompileAllAsync (UML)                       │
   │  ├── CompileAllAsync (HTML)                      │
   │  └── serverStartSignal.Signal()                  │
   └──────────────────────────────────────────────────┘
```

### Mermaid

```mermaid
sequenceDiagram
    participant U as User
    participant M as MainService
    participant P as ParsingOrchestrator
    participant C as ContextInfoCacheService
    participant FP as FileParserPipeline
    participant D as DeclarationFileParser
    participant I as InvocationFileParser
    participant R as RoslynKit
    participant DS as DatasetProvider

    U->>M: RunAsync
    M->>DS: GetDatasetAsync
    DS->>P: ParseAsync
    P->>C: GetOrParseAndCacheAsync
    alt кэш пуст
        C->>FP: ParseAsync(files)
        FP->>D: Phase 1 - ParseFilesAsync
        D->>R: BuildCompilationMap
        R-->>D: SemanticCompilationMap
        D-->>FP: ContextInfo list
        FP->>I: Phase 2 - ParseFilesAsync
        I->>R: ParseInvocations
        R-->>I: links
        I-->>FP: ContextInfo list with refs
        FP-->>C: result
        C->>C: SaveAsync (json)
    end
    C-->>P: IEnumerable<ContextInfo>
    P-->>DS: contexts
    DS->>M: dataset
    M->>M: Compile UML + HTML
```

## Phase 1 — декларации

Задача: **собрать все классы/методы/свойства** в виде `ContextInfo`.

### ASCII

```
   Список .cs файлов
        │
        ▼
   RoslynSyntaxTreeParser
        │
        ▼
   RoslynCompilationBuilder
        │
        ▼
   CSharpCompilation + MetadataReferences
        │
        ▼
   RoslynCompilationMapMapper
        │
        ▼
   SemanticCompilationMap
        │
        ▼
   RoslynDeclarationParser
        │
        ▼
   RoslynSemanticSyntaxRouterBuilder
        │
        ▼
   SemanticSyntaxRouter
        │
        ├── CSharpSyntaxParserTypeClass
        ├── CSharpSyntaxParserMethod
        ├── CSharpSyntaxParserProperty
        ├── CSharpSyntaxParserInterface
        ├── CSharpSyntaxParserEnum
        ├── CSharpSyntaxParserRecord
        └── CSharpSyntaxParserDelegate
                │
                ▼
   ContextInfoBuilderDispatcher
                │
                ▼
   ContextInfoCollector
```

### Mermaid

```mermaid
flowchart TD
    A[Список .cs файлов] --> B[RoslynSyntaxTreeParser]
    B --> C[RoslynCompilationBuilder]
    C --> D[CSharpCompilation + MetadataReferences]
    D --> E[RoslynCompilationMapMapper]
    E --> F[SemanticCompilationMap]
    F --> G[RoslynDeclarationParser]
    G --> H[RoslynSemanticSyntaxRouterBuilder]
    H --> I[SemanticSyntaxRouter]
    I --> J[ISyntaxParser list]
    J --> K1[CSharpSyntaxParserTypeClass]
    J --> K2[CSharpSyntaxParserMethod]
    J --> K3[CSharpSyntaxParserTypeProperty]
    J --> K4[CSharpSyntaxParserInterface]
    J --> K5[CSharpSyntaxParserEnum]
    J --> K6[CSharpSyntaxParserRecord]
    J --> K7[CSharpSyntaxParserDelegate]
    K1 --> L[ContextInfoBuilderDispatcher]
    K2 --> L
    K3 --> L
    K4 --> L
    K5 --> L
    K6 --> L
    K7 --> L
    L --> M[ContextInfoCollector]
```

**Что собирается:**

- `FullName`, `Namespace`, `ShortName`
- `ElementType` (class/method/property/...)
- `ElementVisibility` (public/private/...)
- `ClassOwner`, `MethodOwner`
- `Contexts` (из `// context:` комментариев)
- `Action`, `Domains` (классификация через `ContextClassifier`)

## Phase 2 — вызовы

Задача: **построить граф `References` / `InvokedBy`** между методами.

### ASCII

```
   Phase 1 result
        │
        ▼
   InvocationFileParser
        │
        ▼
   Parallel.ForEachAsync(files)
        │
        ▼
   InvocationParser.ParseInvocationsAsync
        │
        ▼
   RoslynInvocationBuilder
        │
        ▼
   InvocationBuilderValidator
        │
        ▼
   RoslynInvocationLinker
        │
        ▼
   RoslynInvocationSyntaxResolver
        │
        ▼
   GetMethodSymbolAsync
        │
        ▼
   SymbolLookupHandlerChain
        │
        ├── RoslynSymbolLookupHandlerFullname     (прямой поиск по FullName)
        │        │  miss
        │        ▼
        ├── RoslynSymbolLookupHandlerMethod       (fallback по сигнатуре)
        │        │  miss
        │        ▼
        └── RoslynSymbolLookupHandlerInvocation   (fake callee, если CreateFailedCallees)
                 │
                 ▼
   добавить References / InvokedBy
```

### Mermaid

```mermaid
flowchart TD
    A[Phase 1 result] --> B[InvocationFileParser]
    B --> C[Parallel.ForEachAsync files]
    C --> D[InvocationParser.ParseInvocationsAsync]
    D --> E[RoslynInvocationBuilder]
    E --> F[InvocationBuilderValidator]
    F --> G[RoslynInvocationLinker]
    G --> H[RoslynInvocationSyntaxResolver]
    H --> I[GetMethodSymbolAsync]
    I --> J[SymbolLookupHandlerChain]
    J --> K1[RoslynSymbolLookupHandlerFullname]
    K1 -->|miss| K2[RoslynSymbolLookupHandlerMethod]
    K2 -->|miss| K3[RoslynSymbolLookupHandlerInvocation]
    K3 -->|miss + CreateFailedCallees| L[создать fake callee]
    K3 --> M[добавить References/InvokedBy]
```

**Что собирается:**

- Для каждого метода — список вызываемых методов
- Обратные связи `InvokedBy`
- Fake-callee для внешних вызовов (System.*, NuGet и т.д.)

## Phase 3 — построение dataset

После Phase 1+2 мы имеем `IEnumerable<ContextInfo>`. Из него:

### ASCII

```
   ContextInfo list
        │
        ▼
   ContextInfoDatasetBuilder
        │
        ├── ContextInfoFiller (order = int.MinValue)
        │        │
        │        ▼
        │   WordTensorBuildStrategy:
        │   ├── VerbNoun     (priority 1)
        │   ├── VerbOnly     (priority 2)
        │   ├── NounOnly     (priority 2)
        │   └── Unclassified (priority 3)
        │
        └── ContextInfoFillerEmptyData (order = int.MaxValue)
                 │
                 ▼
   IContextInfoDataset<ContextInfo, DomainPerActionTensor>
```

### Mermaid

```mermaid
flowchart LR
    A[ContextInfo list] --> B[ContextInfoDatasetBuilder]
    B --> C[ContextInfoFiller - order MinValue]
    C --> D[WordTensorBuildStrategy VerbNoun / VerbOnly / NounOnly]
    C --> E[ContextInfoFillerEmptyData - order MaxValue]
    E --> F[IContextInfoDataset]
```

Каждый `ContextInfo` разворачивается в декартово произведение
`(all-actions) × (all-domains)` и добавляется в ячейки.

## Phase 4 — экспорт

Параллельно запускаются два оркестратора:

- **UML** — `UmlDiagramCompilerOrchestrator` → ~20 `IUmlDiagramCompiler`
- **HTML** — `HtmlCompilerOrchestrator` → 7 `IHtmlPageCompiler`

Оба читают `IContextInfoDataset<ContextInfo, DomainPerActionTensor>`.

### Порядок экспорта

```
   IContextInfoDataset
        │
        ├── UmlDiagramCompilerOrchestrator.CompileAllAsync
        │        │
        │        └── Task.WhenAll(IUmlDiagramCompiler[])
        │
        └── HtmlCompilerOrchestrator.CompileAllAsync
                 │
                 └── foreach compiler: await CompileAsync
```

UML-компиляторы работают параллельно, HTML-компиляторы — последовательно.
HTML последовательно, потому что делят один dataset и пишут в общую папку.

## Cache

### ASCII

```
   GetOrParseAndCacheAsync
        │
        ▼
   in-memory cache filled?
        │
        ├── да: return in-memory
        │
        └── нет
             │
             ▼
        file cache exists?
             │
             ├── да + RenewCache=false: read json
             │       │
             │       ▼
             │   onRelationCallback
             │   (ConvertToContextInfoAsync)
             │       │
             │       ▼
             │   return
             │
             └── нет или RenewCache=true
                  │
                  ▼
             parse job
                  │
                  ▼
             SaveOnBackground
                  │
                  ▼
             return
```

### Mermaid

```mermaid
flowchart TD
    A[GetOrParseAndCacheAsync] --> B{in-memory cache filled?}
    B -->|yes| C[return in-memory]
    B -->|no| D{file cache exists?}
    D -->|yes + RenewCache=false| E[read json]
    D -->|no или RenewCache=true| F[parse job]
    F --> G[SaveOnBackground]
    E --> H[onRelationCallback]
    G --> C
    H --> C
```

**Что кэшируется:** `List<ContextInfoSerializableModel>` — плоский список без графа.
Граф (`References`, `InvokedBy`, `Owns`) восстанавливается через
`ContextInfoRelationManager.ConvertToContextInfoAsync` **после** чтения из кэша.

## Сводная таблица Phase

| Phase | Компонент | Что | Параллельно? |
|-------|-----------|-----|--------------|
| 1 | `DeclarationFileParser` | декларации | `Task.WhenAll` по файлам |
| 2 | `InvocationFileParser` | вызовы | `Parallel.ForEachAsync` |
| 3 | `ContextInfoDatasetBuilder` | тензоры | последовательно, filler по order |
| 4a | `UmlDiagramCompilerOrchestrator` | .puml | `Task.WhenAll` |
| 4b | `HtmlCompilerOrchestrator` | .html | последовательно |

## Где искать узкие места

| Симптом | Компонент | Настройка |
|---------|-----------|-----------|
| Медленный парсинг | `InvocationFileParser` | `SemanticOptions.MaxDegreeOfParallelism` |
| Долгая компиляция UML | `UmlDiagramCompilerOrchestrator` | кэш `TransitionDiagramBuilderCache` |
| Большой JSON-кэш | `ContextFileCacheStrategy` | `RenewCache=false` |
| OOM при парсинге | `RoslynAssemblyFetcher` | фильтры `TrustedFilters` / `RuntimeFilters` |