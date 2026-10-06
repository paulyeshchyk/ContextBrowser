# In-progress

Текущая работа. Обновляется раз в неделю.

## 2.3 RoslynKit — TypeScript parser

- **NodeID:** 2.3.9 (будет заведён)
- **Статус:** исследование
- **Ответственный:** @владелец
- **Начато:** TBD
- **Комментарий:** определяемся с парсером (`ts-morph` vs `typescript-eslint` vs `oxc`).
  Нужно понять, поднимать ли Node.js-процесс или использовать нативное .NET-решение.

### Подзадачи

- [ ] **2.3.9.1** выбор TS-парсера
  - сравнительная таблица: скорость, размер, API, зависимости
- [ ] **2.3.9.2** `TypeScriptSemanticSyntaxRouterBuilder`
  - по аналогии с `RoslynSemanticSyntaxRouterBuilder`
- [ ] **2.3.9.3** `TypeScriptSignatureParserChain`
  - по аналогии с `CSharpSignatureParserChain`
- [ ] **2.3.9.4** регистрация в DI
  - добавление в `HostConfigurator` + `SemanticSyntaxRouterBuilderRegistry`

## 2.3 RoslynKit — улучшение графа вызовов

- **NodeID:** 2.3.7
- **Статус:** в работе
- **Ответственный:** @владелец
- **Начато:** TBD
- **Комментарий:** точность `References`/`InvokedBy` сейчас ~90%.
  Цель — 95%+ на реальных проектах (GULF_Backend, Triton Front).

### Подзадачи

- [ ] расширить цепочку `RoslynSymbolLookupHandler*`
  - добавить fallback по namespace + short name
- [ ] улучшить `CSharpSignatureParserStandard*`
  - сложные generic-имена
- [ ] написать тесты на реальных проектах
  - фиксировать метрику точности

## 4.1 ContextBrowser — обновление документации

- **NodeID:** —
- **Статус:** в работе
- **Ответственный:** @владелец
- **Начато:** TBD
- **Комментарий:** этот пакет md-файлов + нумерованный mindmap.

### Подзадачи

- [x] `README.md` — обновлён
- [x] `docs/index.md` — навигация
- [x] `docs/00-overview.md` … `02-glossary.md` — базовые
- [x] `docs/10-architecture/` — 3 файла
- [x] `docs/20-domains/` — 3 файла
- [x] `docs/30-components/` — 2 файла
- [x] `docs/40-contexts/mindmap-numbered.md` + `.puml`
- [x] `docs/release/` — 5 файлов
- [ ] `docs/managers/index.md` — в работе
- [ ] вынести PUML в `docs/50-diagrams/`
  - См. [`todo.md`](todo.md)

## 1.5 GraphKit — итеративный DFS

- **NodeID:** 1.5.4
- **Статус:** в работе
- **Ответственный:** @владелец
- **Начато:** TBD
- **Комментарий:** риск `StackOverflowException` на графах глубиной > 10 000.
  Переписываем рекурсию на `Stack<(item, node, state)>`.

### Подзадачи

- [ ] спроектировать `Stack<(item, node, state)>`
- [ ] сохранить сигнатуру `Run<TItem, TNode>`
- [ ] тесты на графе глубиной 50 000
- [ ] бенчмарк: рекурсия vs итеративный

## Легенда

- 📋 — исследование (не начато)
- 🚧 — активная работа
- 🧪 — тесты / валидация
- ✅ — готово к merge (ожидает review)

## Правила ведения

1. **Каждый понедельник** — актуализация статуса.
2. **Не более 3–4 активных пунктов** одновременно.
3. **Каждый пункт имеет NodeID** из mindmap. Если нет — сначала заведи.
4. **При завершении** — переносим в [`done.md`](done.md), здесь удаляем.
5. **При блокировке** — пометка `🚫 blocked: <причина>` и уведомление владельца.

## Блокеры

Нет.