# Snapshots / Before — каталог

Каталог «замороженных» sequence-диаграмм для регрессионного набора
R1–R8 (см. `Jira/Jira.001/readme.md`).

**Назначение:** зафиксировать состояние **до** исправлений
`Jira.001.01`–`Jira.001.08`. Используется для сверки на Этапе 5
и регрессии на Этапе 6.

**Дата фиксации:** TBD
**Команда:** TBD (какая команда запускалась)
**Версия:** TBD (git commit hash)

## Оглавление

| R | Файл | Сценарий | Что проверяет | Статус |
|---|------|----------|---------------|--------|
| R1 | `R1_S1_Actor_Transition.puml` | S1 Actor → Transition | Простой вызов | TBD |
| R2 | `R2_S2_A_Foo__B_Bar.puml` | S2 A.Foo → B.Bar | Один класс → другой класс | TBD |
| R3 | `R3_S3_Orchestrator.puml` | S3 FlowOrchestrator ↔ AnotherService | Bidirectional | TBD |
| R4 | `R4_S4_TaskService.puml` | S4 TaskService + 4 сервиса | Множественные ветвления | TBD |
| R5 | `R5_S6_Alpha_Beta_Gamma.puml` | S6 Alpha → Beta → Gamma | Циклические ссылки | TBD |
| R6 | `R6_Adapter_Adapt.puml` | `ContextInfoSerializableModelAdapter.Adapt` | LINQ-цепочки | TBD |
| R7 | `R7_CsvGenerator.puml` | `CsvGenerator.GenerateHeatmap` | Dictionary, File | TBD |
| R8 | `R8_CacheService.puml` | `ContextInfoCacheService.GetOrParseAndCacheAsync` | Асинхронные вызовы, lock | TBD |

**Статусы:**

- `OK` — снапшот есть, каталогизирован
- `Missing` — снапшот отсутствует по причине (см. ниже)
- `Partial` — снапшот есть, но не полностью соответствует ожидаемому

## Missing / Partial (если применимо)

Заполняется, если снапшот отсутствует или неполный.

### Пример

```
R7 — Missing
Причина: класс CsvGenerator помечен // context: build, csv, heatmap.
В выводе есть sequence_domain_csv.puml, но отдельного файла для R7 нет.
Решение: снять общий snapshot, в каталоге сослаться на него.
```

## Как снимались снапшоты

### R1–R5

- Прогон: `ContextBrowser --project ContextBrowser`
- Источник: `ContextSamples/ContextSamples/S1..S6/*.cs`
- Фильтр: домен (`S1`, `S2`, `S3`, `S4`, `S6`)
- Файлы: `output/ContextBrowser/puml/sequence_domain_<S>.puml`
- Копирование: `cp output/ContextBrowser/puml/sequence_domain_S1.puml`
  `ContextBrowserTests/Snapshots/Before/R1_S1_Actor_Transition.puml`

### R6–R8

- Прогон: `ContextBrowser --project ContextBrowser`
- Источник: код самого `ContextBrowser` / `Kits/ContextKit`
- Фильтр: по action или domain (см. таблицу выше)
- Файлы: `output/ContextBrowser/puml/sequence_*.puml`

## Как читать снапшоты

Каждый `.puml` — это **сырой** вывод приложения на момент фиксации.
Он **не нормализован** (timestamps, порядок участников и т.п. сохранены).
Нормализация — задача тестов (`ContextBrowserTests/Snapshots/Sequence/`),
не задача этого каталога.

**Почему без нормализации:** это эталон «как есть». Если нормализовать
сразу — потеряем контекст. Нормализация — отдельный слой.

## Соглашения

- **Не редактировать вручную.** Если снапшот «грязный» — так и должно быть.
  Правки — только через перезапуск.
- **Не удалять.** Даже если снапшот «неправильный» — он отражает реальность.
- **Не переиспользовать.** Для новой версии — новый файл
  `R<N>_<short-name>__v2.puml`.

## Связанные файлы

- `CHECKS.md` — чек-лист симптомов
- `Jira/Jira.001/readme.md` — Epic, регрессионный набор R1–R8
- `docs/_threads/sequence-acceptance.md` — критерии приёмки (будет создан)