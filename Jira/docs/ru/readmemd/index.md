---
title: readme.md
sectionType: Page
pureTitle: readme.md
sectionIndex: ""
---

# Jira — локальная система отслеживания задач

## Что это

Отдельная от кодовой базы система тикетов. Живёт **вне** репозитория
ContextBrowser, чтобы не хламить кодовую базу.

**Правило:** ни один `.cs`, `.csproj`, `.puml` не должен ссылаться на `Jira/`.
Связь односторонняя — Jira знает про код, код про Jira не знает.

## Структура

```text
Jira/
├── readme.md              ← этот файл
├── howTo.md               ← что дать AI в новом треде
├── backlog.md             ← чекбоксы завершённых задач
├── _template/
│   └── readme.md          ← шаблон подпапки
│
├── Jira.001/              ← Epic: Sequence Diagram Fix
│   ├── readme.md          ← описание эпика
│   ├── Jira.001.01/
│   │   └── readme.md
│   ├── Jira.001.02/
│   │   └── readme.md
│   └── … (до 001.08)
│
├── Jira.002/              ← следующий эпик
│   └── …
└── …
```

## Правила

### 1. Один Epic — одна папка

Имя папки: `Jira.NNN`, где `NNN` — трёхзначный номер, начиная с `001`.
Внутри — `readme.md` с описанием эпика.

### 2. Подзадача — подпапка

Имя: `Jira.NNN.MM`, где `MM` — двузначный номер, начиная с `01`.
Внутри — `readme.md` с описанием подзадачи.

### 3. Глубина

Если подзадача требует дальнейшего дробления — она получает дочерние
`Jira.NNN.MM.KK`. Глубина не ограничена. **Мы идём в глубину, не в ширину.**

### 4. Единообразие

Все `readme.md` в подпапках `Jira.NNN.MM` имеют **одинаковую структуру** —
см. `_template/readme.md`.

### 5. Связь с кодом

Jira **знает** про:
- пути к файлам, которые правим (`files:`)
- пути к тестам (`tests:`)
- пути к снапшотам (`snapshots:`)
- имена классов/методов (в тексте)

Код **не знает** про Jira. Исключение — комментарии в коде
(опционально, по согласованию).

### 6. Обновление

- `status` в readme меняется по мере работы
- при завершении подзадачи — чекбокс в `backlog.md`
- при завершении эпика — чекбокс эпика в `backlog.md`

### 7. Никаких ссылок в кодовой базе

В `.cs` и `.csproj` — ни одной ссылки на `Jira/`.
Если нужна трассировка — через git-commit message:

```text
Jira.001.01 — UmlActivate: разделить роли
```

## Формат полей

Метаданные — в YAML frontmatter в начале `readme.md`:

```yaml
---
id: Jira.001.01
type: Sub-task
parent: Jira.001
priority: High
status: Open
reporter: owner
assignee: AI
labels: [uml, sequence, activation]
components: [UmlKit]
affects-version: TBD
fix-version: TBD
blocked-by: []
blocks: []
related: []
files:
  - Kits/UmlKit/PlantUmlSpecification/UmlActivate.cs
tests:
  - ContextBrowserTests/Uml/Sequence/UmlActivateTests.cs
snapshots:
  - ContextBrowserTests/Snapshots/Before/convert.puml
reference:
  - docs/_threads/sequence-acceptance.md
agent-context:
  - howTo.md
  - Jira.001/readme.md
---
```

## Соглашения об именах

| Что                | Формат           | Пример                          |
| ------------------ | ---------------- | ------------------------------- |
| Папка Epic         | `Jira.NNN`       | `Jira.001`                      |
| Папка Sub-task     | `Jira.NNN.MM`    | `Jira.001.01`                   |
| Папка Sub-sub-task | `Jira.NNN.MM.KK` | `Jira.001.01.01`                |
| Файл описания      | `readme.md`      | всегда `readme.md`              |
| Статус             | из списка        | `Open` / `In Progress` / `Done` |

## Что НЕ делаем

- ❌ Не заводим ветки git per-ticket — все правки в одной ветке
- ❌ Не пишем ссылки на Jira в коде
- ❌ Не заводим подпапку, пока задача не сформулирована
- ❌ Не удаляем `readme.md` после закрытия — это исторический артефакт