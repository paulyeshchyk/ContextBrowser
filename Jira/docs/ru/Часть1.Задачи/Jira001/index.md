---
title: Jira.001
sectionType: Page
pureTitle: Jira.001
id: Jira.001
type: Epic
parent: —
priority: High
status: Open
reporter: owner
assignee: AI + owner
labels:
  - uml
  - sequence
  - activation
  - refactor
components:
  - UmlKit
  - ExporterKit
affects-version: TBD
fix-version: TBD
blocked-by: []
blocks: []
related: []
files:
  - Kits/UmlKit/Managers/SequenceActivationManager.cs
  - Kits/UmlKit/Managers/SequenceInvocationManager.cs
  - Kits/UmlKit/Managers/SequenceParticipantsManager.cs
  - Kits/UmlKit/Managers/SequenceTransitionManager.cs
  - Kits/UmlKit/PlantUmlSpecification/UmlActivate.cs
  - Kits/UmlKit/Renderer/UmlTransitionRenderer/UmlTransitionRendererFlat.cs
  - Kits/ExporterKit/Uml/DiagramCompiler/CoCompiler/Sequence/UmlDiagramCompilerSequence.cs
tests:
  - 'ContextBrowserTests/Uml/Sequence/*.cs'
  - ContextBrowserTests/Invariants/SequenceInvariants.cs
  - 'ContextBrowserTests/Snapshots/Sequence/*.cs'
snapshots:
  - ContextBrowserTests/Snapshots/Before/
  - ContextBrowserTests/Snapshots/After/
reference:
  - docs/_threads/sequence-acceptance.md
agent-context:
  - Jira/howTo.md
---

# Jira.001 — Исправление построения sequence диаграмм

## Description

Sequence-диаграмма, генерируемая для action `convert`, содержит
**10 классов ошибок**, из-за которых диаграмма искажает реальный
ход вызовов. Цель — устранить ошибки и получить диаграмму,
семантически соответствующую коду.

**Подопытный:** `ContextInfoSerializableModelAdapter.Adapt(List<ContextInfo>)`.

## Root cause (сводка)

| Причина | Симптом                               | Файл                                                           |
| ------- | ------------------------------------- | -------------------------------------------------------------- |
| A       | Двойная активация callee              | `SequenceActivationManager.cs`, `SequenceInvocationManager.cs` |
| B       | Self-call caller                      | `SequenceActivationManager.cs`                                 |
| C       | `UmlActivate` смешивает роли          | `UmlActivate.cs`                                               |
| D       | `callStack` cleanup на след. итерации | `UmlTransitionRendererFlat.cs`                                 |
| E       | Transition ≠ дерево вызовов           | `UmlTransitionDtoBuilder.cs`                                   |
| F       | `SystemCall` без источника            | `SequenceTransitionManager.cs`                                 |
| G       | `Actor` для LINQ-методов              | `SequenceParticipantsManager.cs`                               |
| H       | Три участника вместо одного           | `UmlTransitionDtoBuilder.cs`                                   |

## Preflight

Перед началом правок:

1. **Снапшоты Before.** Собрать текущие `.puml` для R1–R8
   (список ниже), сложить в `Snapshots/Before/`.
2. **Красные тесты.** Написать:
   - 5 unit-тестов (по одному на A, B, C, D, G)
   - 1 snapshot-тест для подопытного (R6)
   - Метрические инварианты M1–M7 для R1–R8
3. **Эталон.** Создать `docs/_threads/sequence-acceptance.md`
   с эталоном диаграммы для подопытного.

## Acceptance criteria (M1–M7)

| #   | Метрика                               | До  | После |
| --- | ------------------------------------- | --- | ----- |
| M1  | Кол-во `actor` (не людей)             | > 0 | 0     |
| M2  | Кол-во `activate` − `deactivate`      | ≠ 0 | = 0   |
| M3  | Кол-во возвратов с типом (не `done`)  | 0   | ≥ 1   |
| M4  | Self-call caller на один переход      | ≥ 1 | 0     |
| M5  | Дублирующие `activate` подряд         | ≥ 1 | 0     |
| M6  | Явный entry-point                     | нет | есть  |
| M7  | Один участник на namespace библиотеки | нет | есть  |

## Регрессионный набор (R1–R8)

| #   | Сценарий                                          | Что проверяет             |
| --- | ------------------------------------------------- | ------------------------- |
| R1  | `S1` Actor → Transition                           | Простой вызов (baseline)  |
| R2  | `S2` A.Foo → B.Bar                                | Один класс → другой класс |
| R3  | `S3` FlowOrchestrator ↔ AnotherService            | Bidirectional             |
| R4  | `S4` TaskService + 4 сервиса                      | Множественные ветвления   |
| R5  | `S6` Alpha → Beta → Gamma                         | Циклические ссылки        |
| R6  | `ContextInfoSerializableModelAdapter.Adapt`       | LINQ-цепочки              |
| R7  | `CsvGenerator.GenerateHeatmap`                    | Dictionary, File          |
| R8  | `ContextInfoCacheService.GetOrParseAndCacheAsync` | Асинхронные вызовы, lock  |

## Подзадачи (Sub-tasks)

| ID          | Название                                          | Причина     | Priority | Status |
| ----------- | ------------------------------------------------- | ----------- | -------- | ------ |
| Jira.001.01 | UmlActivate: разделить роли                       | C           | High     | Open   |
| Jira.001.02 | RenderActivateCaller: убрать self-call            | B           | High     | Open   |
| Jira.001.03 | Убрать двойную активацию callee                   | A           | High     | Open   |
| Jira.001.04 | Actor → participant для не-людей                  | G           | Medium   | Open   |
| Jira.001.05 | Группировка методов одного класса в один участник | H           | High     | Open   |
| Jira.001.06 | SystemCall: явный entry-point                     | F           | Medium   | Open   |
| Jira.001.07 | callStack: cleanup на границе перехода            | D           | Medium   | Open   |
| Jira.001.08 | Пометить встроенные LINQ-методы                   | E (часть 1) | High     | Open   |

**Возможные (появятся по ходу):**

| ID          | Название                                      | Причина     | Status   |
| ----------- | --------------------------------------------- | ----------- | -------- |
| Jira.001.09 | OutgoingTransitionBuilder: пропуск встроенных | E (часть 2) | Reserved |
| Jira.001.10 | Группировка LINQ в loop                       | E (часть 3) | Reserved |
| Jira.001.11 | Возвраты с типами                             | E (часть 4) | Reserved |
| Jira.001.12 | Финальная сверка с эталоном                   | —           | Reserved |
| Jira.001.13 | Регрессия R1–R8                               | —           | Reserved |

## Definition of Done (для Epic)

- [ ] Все 8 подзадач в статусе `Done`
- [ ] `Snapshots/After/convert.puml` совпадает структурно с эталоном
- [ ] Все метрики M1–M7 улучшены на R1–R8
- [ ] Регрессия зелёная
- [ ] `docs/release/done.md` обновлён
- [ ] `docs/40-contexts/` содержит эталон

## Notes

- **Глубина, не ширина.** Идём по подзадачам последовательно.
  Не начинаем 02, пока 01 не в статусе `Done`.
- **Тесты не знают про Jira.** Связь — только через этот readme
  и через `Jira/backlog.md`.
- **Комментарии в коде.** По согласованию — метка `// Jira.001.01`
  перед изменённым блоком (для трассировки через `git log`).
