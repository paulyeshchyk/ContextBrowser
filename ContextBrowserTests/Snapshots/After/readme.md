# Snapshots / After — каталог

Каталог «замороженных» sequence-диаграмм **после** исправлений
`Jira.001.01`–`Jira.001.08`.

**Назначение:** сверка с эталоном на Этапе 5 и регрессия на Этапе 6.

**Статус:** пусто. Будет заполнено после закрытия `Jira.001.01`–`08`.

## Оглавление

| R | Файл | Сценарий | Статус |
|---|------|----------|--------|
| R1 | `R1_S1_Actor_Transition.puml` | S1 Actor → Transition | Open |
| R2 | `R2_S2_A_Foo__B_Bar.puml` | S2 A.Foo → B.Bar | Open |
| R3 | `R3_S3_Orchestrator.puml` | S3 FlowOrchestrator ↔ AnotherService | Open |
| R4 | `R4_S4_TaskService.puml` | S4 TaskService + 4 сервиса | Open |
| R5 | `R5_S6_Alpha_Beta_Gamma.puml` | S6 Alpha → Beta → Gamma | Open |
| R6 | `R6_Adapter_Adapt.puml` | `ContextInfoSerializableModelAdapter.Adapt` | Open |
| R7 | `R7_CsvGenerator.puml` | `CsvGenerator.GenerateHeatmap` | Open |
| R8 | `R8_CacheService.puml` | `ContextInfoCacheService.GetOrParseAndCacheAsync` | Open |

## Соглашения

- Файлы именуются **точно как в `Before/`**. Это позволяет делать
  `diff Before/R6_Adapter_Adapt.puml After/R6_Adapter_Adapt.puml`.
- Файлы снимаются **той же командой**, что и в `Before/`. Разница только
  в git-commit hash.
- Нормализация (timestamps, порядок) — задача тестов, не этого каталога.

## Как обновлять

Каждый раз, когда закрывается подзадача Jira.001.01–08, снапшоты
пересобираются заново. Это правило **не** «обновляем после каждой правки»,
а «обновляем, когда все правки в текущей итерации сделаны».

**Почему:** пересборка — дорого (полный прогон pipeline). Если делать её
после каждой правки, мы будем тратить время на прогон, а не на фиксы.

## Связанные файлы

- `../Before/readme.md` — каталог «до»
- `../Before/CHECKS.md` — чек-лист симптомов
- `Jira/Jira.001/readme.md` — Epic