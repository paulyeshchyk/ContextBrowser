---
title: Structure
sectionType: Page
pureTitle: Structure
sectionIndex: ""
---

# Документация ContextBrowser

Навигация по всем разделам.

```
docs/
├── 00-overview.md              # быстрый тур
├── 01-goals.md                 # цели и roadmap
├── 02-glossary.md              # термины
│
├── 10-architecture/            # КАК устроено
│   ├── 01-layers.md            # слои и связность
│   ├── 02-dependencies.md      # правила зависимостей
│   └── 03-pipeline.md          # pipeline парсинга
│
├── 20-domains/                 # ЧТО делает
│   ├── 01-context-model.md     # модель контекста
│   ├── 02-parsing.md           # парсинг C# и TS
│   └── 03-export.md            # экспорт UML/HTML
│
├── 30-components/              # КОМПОНЕНТЫ
│   ├── kits.md                 # карта всех Kit-ов
│   └── app.md                  # ContextBrowser приложение
│
├── 40-contexts/                # КАРТА контекстов
│   └── mindmap-numbered.md     # нумерованный mindmap
│
├── release/                    # СТАТУС
│   ├── done.md
│   ├── in-progress.md
│   ├── todo.md
│   └── backlog.md
│
└── managers/                   # ДЛЯ МЕНЕДЖЕРОВ
    └── index.md
```

## Рекомендуемый порядок чтения

**Новый разработчик:**
1. `00-overview.md`
2. `01-goals.md`
3. `10-architecture/01-layers.md`
4. `30-components/kits.md`

**Менеджер:**
1. `managers/index.md`
2. `01-goals.md`
3. `release/`

**Хочешь пофиксить баг:** сразу `40-contexts/mindmap-numbered.md` — найдёшь нужный класс за 30 секунд.