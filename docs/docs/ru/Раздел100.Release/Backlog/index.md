---
title: Backlog
sectionType: Page
pureTitle: Backlog
sectionIndex: ""
---

# Backlog

Идеи вне текущего приоритета. Раз в квартал — ревизия.

## Killer feature — детектор паттернов

### Паттерн-детектор v1 — Strategy / Factory / Visitor / Builder

- **NodeID:** 1.6 (новый узел)
- **Горизонт:** 6+ месяцев
- **Комментарий:** на основе накопленного графа `ContextInfo` + `Relations`
  можно детектировать паттерны.
- **Зависит от:** стабильный граф, ≥ 3 реальных проекта, исследование

**Идеи:**

- **Strategy** — класс с несколькими реализациями одного интерфейса
- **Factory** — метод, возвращающий разные типы по параметру
- **Visitor** — `Accept/Visit` пары
- **Builder** — цепочка методов, возвращающих `this`

### Паттерн-детектор v2 — Composite / Observer / Decorator

- **NodeID:** 1.6.x
- **Горизонт:** 9+ месяцев

- **Composite** — рекурсивная структура с `Children` + `Parent`
- **Observer** — `Subscribe/Notify` пары
- **Decorator** — обёртка с тем же интерфейсом

### Cross-language pattern

- **NodeID:** 1.6.y
- **Горизонт:** 12+ месяцев
- **Комментарий:** TS-компонент вызывает C#-сервис. Паттерн на границе языков.

## Идеи по экспорту

### JSON-API

- **NodeID:** 3.4 (новый узел)
- **Комментарий:** REST endpoint для получения контекстов.
  - `GET /api/contexts?action=create&domain=roslyn`
  - `GET /api/contexts/{fullName}`
  - `GET /api/dataset/{action}/{domain}`

### Interactive HTML

- **NodeID:** 3.2.x
- **Комментарий:** фильтры, сортировки, поиск в матрице.
  Сейчас HTML статичный.

### Diff-view

- **NodeID:** 3.5 (новый узел)
- **Комментарий:** сравнение контекстов между коммитами.
  Что добавилось, что ушло, что изменило coverage.

### Historical charts

- **NodeID:** 3.5.x
- **Комментарий:** динамика coverage по неделям.

## Инфраструктура

### CI pipeline

- **NodeID:** 4.3 (новый узел)
- **Комментарий:** GitHub Actions / GitLab CI.
  - build
  - test
  - lint
  - публикация NuGet-пакетов Kits

### Docker image

- **NodeID:** 4.4 (новый узел)
- **Комментарий:** всё в одном контейнере.
  `ContextBrowser` + `http-server` + `plantuml-server`.

### NuGet package

- **NodeID:** 4.5 (новый узел)
- **Комментарий:** публикация Kits для других проектов.
  `ContextKit`, `TensorKit`, `SemanticKit` — потенциально полезны.

### Incremental build

- **NodeID:** 4.6 (новый узел)
- **Комментарий:** не пересобирать UmlKit/HtmlKit без изменений.

## Интеграции

### IDE-плагин

- **NodeID:** 4.7 (новый узел)
- **Комментарий:** VS Code / Rider для просмотра контекстов.
  Hover над классом → показать его контексты.

### Slack/Telegram bot

- **NodeID:** 4.8 (новый узел)
- **Комментарий:** уведомления о coverage.
  - «Coverage упал ниже 50% в roslyn»
  - «Добавлено 10 новых контекстов create»

### Jira-интеграция

- **NodeID:** 4.9 (новый узел)
- **Комментарий:** связь context с задачей.
  `// context: create, loader // JIRA-123` → ссылка на задачу.

## Экспериментальное

### LLM-аннотации

- **NodeID:** 5.3 (новый узел)
- **Комментарий:** LLM дописывает `// context:` там, где автор забыл.
  Требует осторожности — легко получить мусор.

### NLP по комментариям

- **NodeID:** 5.4 (новый узел)
- **Комментарий:** понять намерение по свободному тексту.
  Если автор написал `// создаёт новый контекст`, понять, что это `create`.

### Embedding-based search

- **NodeID:** 5.5 (новый узел)
- **Комментарий:** искать похожие методы.
  «Найди мне все методы, похожие на `RoslynSyntaxCompiler.CreateCompilation`».

## Известные проблемы

### GitHub Issue #1: рекурсивный DFS → StackOverflow

- **NodeID:** 1.5.4
- **Статус:** в работе (см. `in-progress.md`)

### GitHub Issue #2: `#warning incorrect mapping for void Console.Writeline(string? value)`

- **NodeID:** 2.3.3.3
- **Комментарий:** `CSharpSyntaxWrapperMethodArtifitial` — incorrect mapping для void-методов.

### GitHub Issue #3: `#warning this is incorrect` в `CSharpSyntaxNodeWrapper*`

- **NodeID:** 2.3.2.x
- **Комментарий:** `GetShortName()` в нескольких wrapper-ах возвращает `GetName()`.

### GitHub Issue #4: `#warning add elementVisibilityParam`

- **NodeID:** 2.3.x
- **Комментарий:** `CSharpSyntaxWrapperInvocation.GetContextInfoDto()` хардкодит `public`.

### GitHub Issue #5: `#warning namespace is expected`

- **NodeID:** 3.1.4.3
- **Комментарий:** `SequenceParticipantsManager` предполагает `Namespace.*` в именах.

## Правила ведения

1. **Раз в квартал** — пересмотреть список.
2. **Мёртвые идеи** — помечаем `(dead)`, через год удаляем.
3. **Появившийся приоритет** — переносим в [`todo.md`](todo.md).
4. **Backlog не для «когда-нибудь»** — если идея не имеет шанса в ближайший год,
   лучше записать в личный блокнот, не сюда.