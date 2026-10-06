# ContextBrowser

> **Навигатор по контекстам кода.** Анализирует C#- и TypeScript-исходники,
> извлекает семантические теги из комментариев (`// context: <action>, <domain>`),
> строит граф связей и генерирует навигационные отчёты в HTML и PlantUML.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)]()
[![Roslyn](https://img.shields.io/badge/Roslyn-4.11-blue)]()
[![PlantUML](https://img.shields.io/badge/PlantUML-picoweb-green)]()

---

## Зачем это нужно

Кодовая база без семантической навигации умирает быстрее, чем её успевают
поддерживать. `ContextBrowser` восстанавливает **семантический каркас** проекта,
используя **комментарии самих авторов** как источник истины.

### Цели проекта

| # | Цель | Горизонт |
|---|------|---------|
| 1 | **Основная:** анализ кода на основе авторских комментариев (`// context:`), построение навигационных отчётов | сейчас |
| 2 | **Киллер-фича:** автоматическое выявление паттернов программирования (Strategy, Factory, Visitor, …) на основе накопленного контекста | далёкое будущее |

Подробнее см. [`docs/01-goals.md`](docs/01-goals.md).

---

## Что уже работает

- ✅ Парсинг C# через Roslyn (`Kits/RoslynKit`)
- ✅ Извлечение контекстов из комментариев (`Kits/ContextKit`)
- ✅ Граф связей `References / InvokedBy / Owns / Properties`
- ✅ Матрица `Action × Domain` (`Kits/TensorKit`)
- ✅ Экспорт в PlantUML: class / sequence / state / mindmap / packages
- ✅ Экспорт в HTML с heatmap по `coverage`
- ✅ Локальный HTTP-сервер + PlantUML picoweb
- ✅ Кэш результатов парсинга (in-memory + файл JSON)

Планы см. [`docs/release/`](docs/release/).

---

## Архитектура в двух словах

Program.cs → AppOptionsResolver → ConsoleRunner | WebAppRunner
└─ HostConfigurator (DI) → MainService
├─ ParsingOrchestrator ──► RoslynKit + SemanticKit + ContextKit
├─ UmlDiagramCompilerOrchestrator ──► ExporterKit.Uml → UmlKit
└─ HtmlCompilerOrchestrator ──► ExporterKit.Html → HtmlKit


Слои и правила зависимостей: [`docs/10-architecture/01-layers.md`](docs/10-architecture/01-layers.md),
[`docs/10-architecture/02-dependencies.md`](docs/10-architecture/02-dependencies.md).

---

## Формат контекста

```csharp
// context: create, loader
public class Loader { … }

```
Action — одно из предопределённых: create, read, update, delete, validate, share, build, model, execute, convert

Domain — любое существительное-тег (roslyn, uml, html, graph, …)

[Подробности](docs/20-domains/01-context-model.md)


## Быстрый старт

### 1. Собрать

```sh
dotnet build ContextBrowser.sln -c Release
```

### 2. Запустить в консольном режиме

```sh
ContextBrowser.exe --project ContextBrowser
# или через явный конфиг
ContextBrowser.exe --appOptionsFilePath .config/options.json --renewAppOptions true
```

### 3. Запустить в web-режиме

```sh
ContextBrowser.exe --executionMode WebApp
```

### 4. Локальные сервисы (опционально, для просмотра отчёта)

```sh
# HTTP-сервер для HTML
npx http-server -p 5500 --no-cache

# PlantUML picoweb для рендера диаграмм
java -jar plantuml-1.2025.4.jar -picoweb
```

Docker-вариант PlantUML и детали [см. docs/00-overview.md](docs/00-overview.md.).

## Документация

| Раздел | Для кого |
| --- | --- |
| docs/index.md | навигация по всей документации
| docs/00-overview.md | быстрый тур по проекту
| docs/01-goals.md | цели, приоритеты, roadmap
| docs/10-architecture/ | слои, зависимости, pipeline
| docs/20-domains/ | модель контекста, парсинг, экспорт
| docs/30-components/ | описание Kit-ов и приложения
| docs/40-contexts/ | карта контекстов, нумерованный mindmap
| docs/release/ | DONE / in-progress / TODO / backlog
| docs/managers/ | для менеджеров: зачем поддерживать проект

## Лицензия и поддержка

Внутренний проект. Владелец: [см. docs/managers/index.md](docs/managers/index.md)

.
При сомнениях — писать владельцу, а не «в общий чат».