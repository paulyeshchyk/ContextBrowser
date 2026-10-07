---
title: Jira.001.01
sectionType: Page
pureTitle: Jira.001.01
id: Jira.001.01
type: Sub-task
parent: Jira.001
priority: High
status: Open
reporter: owner
assignee: AI
labels: [uml, sequence, activation, refactor]
components: [UmlKit]
affects-version: TBD
fix-version: TBD
blocked-by: []
blocks: [Jira.001.02, Jira.001.03]
related: [Jira.001.07]
files:
  - Kits/UmlKit/PlantUmlSpecification/UmlActivate.cs
tests:
  - ContextBrowserTests/Uml/Sequence/UmlActivateTests.cs
snapshots:
  - ContextBrowserTests/Snapshots/Before/convert.puml
reference:
  - docs/_threads/sequence-acceptance.md
agent-context:
  - Jira/howTo.md
  - Jira/Jira.001/readme.md
---

# Jira.001.01 — UmlActivate: разделить роли

## Description

`UmlActivate` исторически пишет **две строки**:
- call-line: `Source -> Destination : reason`
- declaration: `activate Destination`

Из-за этого в диаграмме появляются дублирующие call-line, потому что
call-line **уже** генерируется transition-механизмом
(`UmlTransitionParticipant.WriteTo`). Нужно разделить роли: `UmlActivate`
отвечает **только** за `activate` / `deactivate`, call-line — вне его.

## Root cause

`UmlActivate.WriteTo` (файл `UmlActivate.cs`):

```csharp
if (!string.IsNullOrEmpty(Source))
{
    writer.Write(Source);
    writer.Write($" -> {Destination}");
    if (!string.IsNullOrWhiteSpace(_reason)) writer.Write($": {_reason}");
    writer.WriteLine();
}
else
{
    if (!string.IsNullOrWhiteSpace(_reason))
    {
        writer.Write($" -> {Destination}");
        writer.Write($": {_reason}");
        writer.WriteLine();
    }
}
if (!SoftActivation)
    writer.WriteLine(Declaration);   // "activate {Destination}"
```

Это делает `UmlActivate` **двойным** артефактом: и transition, и activation.
В контексте, где transition уже добавлен явно (через `AddTransition`),
получается дубль.

## Files affected

- `Kits/UmlKit/PlantUmlSpecification/UmlActivate.cs`

## Dependencies

- **blocked by:** —
- **blocks:** `Jira.001.02` (self-call caller — если `UmlActivate` меняется,
  то и логика caller'а пересматривается)
- **related:** `Jira.001.07` (callStack cleanup — обе правки касаются
  того, что попадает в `Elements`)

## Acceptance criteria

- [ ] `UmlActivate.WriteTo` пишет **только** `activate {Destination}`
      при `SoftActivation == false`
- [ ] Call-line (`Source -> Destination : reason`) больше не эмитится
- [ ] `UmlActivate.Source` остаётся в модели (для будущей диагностики),
      но не пишется в вывод
- [ ] Существующие вызовы `UmlActivate` в коде продолжают работать
      (проверить `grep -r "new UmlActivate" Kits/`)

## Tests to write

- [ ] `ContextBrowserTests/Uml/Sequence/UmlActivateTests.cs`:
  - Тест 1: `UmlActivate(source=null, destination="X", reason="foo", soft=false)` →
    вывод содержит `activate X`, **не содержит** `X -> X`
  - Тест 2: `UmlActivate(source="A", destination="B", reason="bar", soft=false)` →
    вывод содержит `activate B`, **не содержит** `A -> B : bar`
  - Тест 3: `soft=true` → вывод **пустой** (нет `activate`)
- [ ] Snapshot-тест для R6 (`convert.puml`) — должен показать
  меньше строк после правки

## Definition of Done

- [ ] Код изменён (1 файл)
- [ ] Тесты зелёные
- [ ] Snapshot `After/convert.puml` обновлён
- [ ] `Jira/backlog.md` — чекбокс `Jira.001.01`
- [ ] `status` в этом файле → `Done`

## Notes / Investigation

### Связанные вызовы

Перед правкой выполнить:

```
grep -rn "new UmlActivate" Kits/ ContextBrowser/
grep -rn "\.Activate(" Kits/UmlKit/
```

Список мест, где создаётся `UmlActivate`:

- `UmlDiagramSequence.Activate(...)` — два overload'а
- `UmlDiagramState.Activate(...)`
- `UmlDiagramClass.Activate(...)`
- `UmlDiagramMindmap.Activate(...)` — throws

Все они вызывают `new UmlActivate(...)`. После правки
**только** `UmlDiagramSequence` реально влияет на вывод.

### Ожидаемое уменьшение строк

Для диаграммы `convert.puml`:

| До      | После                  |
| ------- | ---------------------- |
| N строк | N − (кол-во call-line) |

Оценка: 12 call-line × 1 строка = −12 строк.

### Риски

| Риск                                             | Митигация                                   |
| ------------------------------------------------ | ------------------------------------------- |
| Кто-то полагался на call-line из `UmlActivate`   | Прогон всех sequence-снапшотов после правки |
| `UmlDiagramState` использует `UmlActivate` иначе | Проверить state-диаграммы до/после          |

### Связанные Jira

- `Jira.001.02` — если `UmlActivate` больше не пишет call-line,
  то self-call caller **точно** нужно убирать отдельно, иначе
  останется «висячий» `activate caller` без call-line.
- `Jira.001.07` — после того, как call-line убран из `UmlActivate`,
  «долгий» `callStack` может стать ещё заметнее (лишний `deactivate`
  будет без парного `activate` в визуальном блоке).
No newline at end of file
