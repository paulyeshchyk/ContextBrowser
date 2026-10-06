# Глоссарий

## Домен

| Термин | Значение |
|--------|----------|
| **Контекст** (context) | Тег из комментария `// context: word1, word2`. Одно слово = один контекст. |
| **Действие** (action) | Контекст-глагол. Одно из `create/read/update/delete/validate/share/build/model/execute/convert` или `_fakeAction`. |
| **Домен** (domain) | Контекст-существительное. Любое слово, не попавшее в actions. |
| **ContextInfo** | Класс/метод/свойство + его контексты. Основная единица модели. |
| **Тензор** (tensor) | Пара `(action, domain)`, ключ ячейки матрицы. |
| **Ячейка** (cell) | Множество `ContextInfo`, попавших в один тензор. |

## Архитектура

| Термин | Значение |
|--------|----------|
| **Kit** | Библиотека проекта (Kits/*). 12 штук. |
| **Pipeline** | Последовательность фаз парсинга (declaration → invocation). |
| **Phase 1** | Парсинг деклараций (классы, методы, свойства). |
| **Phase 2** | Парсинг вызовов (invocations) и построение графа ссылок. |
| **Orchestrator** | Координатор, собирающий композитную работу из многих стратегий. |
| **Provider** | Ленивый сервис-обёртка над данными (dataset, indexer, mapper). |
| **Filler** | Стратегия заполнения ячейки тензора (с приоритетом). |
| **Strategy** (в контексте ContextBrowser) | Реализация `IWordTensorBuildStrategy` — решает, как разложить контексты в тензоры. |
| **Builder** (в контексте ContextBrowser) | Реализация `IUmlDiagramCompiler` / `IContextDiagramBuilder` — генерирует конкретный тип диаграммы. |

## Кэш

| Термин | Значение |
|--------|----------|
| **File cache** | `roslyn.json` — сериализованный список `ContextInfoSerializableModel`. |
| **In-memory cache** | `Task<IEnumerable<ContextInfo>>` внутри `ContextInfoCacheService`. |
| **LRU cache** | `LRUCache<TKey,TValue>` в `ContextBrowserKit`. |
| **Renew** | Флаг `--renewAppOptions` — перезаписать кэш. |

## Экспорт

| Термин | Значение |
|--------|----------|
| **Puml** | Файл `.puml` — PlantUML-скрипт. |
| **Coverage** | Число из комментария `// coverage: N`. Участвует в heatmap. |
| **Heatmap** | Цветовая индикация среднего coverage в ячейке. |
| **Summary row/col** | Итоговая строка/колонка в HTML-таблице. |
| **UnclassifiedPriority** | Куда ставить «неклассифицированные» (без пары action/domain) элементы: `Highest / Lowest / None`. |
| **SummaryPlacement** | Где размещать summary: `AfterFirst / AfterLast / None`. |

## Нумерация

| Термин | Значение |
|--------|----------|
| **Node ID** | Номер вида `<Parent>.<Child>.<Grandchild>`. Уникален в пределах уровня. |
| **Level** | Уровень вложенности в mindmap / документе. Уровень 0 — корень. |