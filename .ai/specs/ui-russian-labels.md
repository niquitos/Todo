# UI Russian Labels Bugfix

## Why
В UI приложения присутствуют английские надписи — заголовок страницы, плейсхолдеры, тексты ошибок. Это inconsistency с общим русскоязычным интерфейсом.

## What
Все видимые пользователю строки в frontend переведены на русский язык. В интерфейсе не осталось английского текста.

## Context

**Relevant files:**
- `src/frontend/index.html` — `<title>` и `lang`
- `src/frontend/src/sprints/pages/SprintBoardPage.tsx` — заголовок "Sprint Board"
- `src/frontend/src/sprints/components/SprintForm.tsx` — placeholder "Sprint 1"
- `src/frontend/src/sprints/api/sprintsApi.ts` — 5 сообщений об ошибках
- `src/frontend/src/tasks/api/taskItemsApi.ts` — 5 сообщений об ошибках
- `src/frontend/src/sprints/hooks/useSprintForm.ts` — "Operation failed"
- `src/frontend/src/sprints/hooks/useSprints.ts` — 4 fallback-сообщения
- `src/frontend/src/tasks/hooks/useTaskItems.ts` — 5 fallback-сообщений

**Patterns to follow:**
- Существующие русские строки в компонентах (например, labels в SprintForm.tsx уже на русском)
- Лаконичный стиль без восклицательных знаков

## Требования

### Функциональные

**FR-1: Заголовок страницы и мета-теги**
- `<title>` заменён с "AgileBoard" на "AgileBoard — Доска задач"
- `lang="en"` заменён на `lang="ru"`

**FR-2: Заголовок на странице**
- "Sprint Board" заменён на "Доска спринта"

**FR-3: Плейсхолдеры**
- "Sprint 1" заменён на "Спринт 1"

**FR-4: Сообщения об ошибках API**
- Все англоязычные fallback-сообщения в `sprintsApi.ts` и `taskItemsApi.ts` переведены на русский

**FR-5: Fallback-сообщения в хуках**
- Все англоязычные fallback в хуках (`useSprints`, `useSprintForm`, `useTaskItems`) переведены на русский

## Ограничения

**Must:**
- Менять только строковые литералы, не трогать логику

**Must not:**
- Не менять backend
- Не менять сигнатуры функций, имена переменных, типы

**Out of scope:**
- Локализация (i18n) — просто фикс строк
- Backend сообщения об ошибках

## Проектирование

### Подход

Замена строковых литералов без изменения логики. Каждый файл правится атомарно — одна задача = один файл (кроме тривиальных).

### Маппинг строк

#### index.html
| Было | Стало |
|------|-------|
| `<html lang="en">` | `<html lang="ru">` |
| `<title>AgileBoard</title>` | `<title>AgileBoard — Доска задач</title>` |

#### SprintBoardPage.tsx
| Было | Стало |
|------|-------|
| `Sprint Board` | `Доска спринта` |

#### SprintForm.tsx
| Было | Стало |
|------|-------|
| `Sprint 1` | `Спринт 1` |

#### sprintsApi.ts
| Было | Стало |
|------|-------|
| `Failed to fetch sprints` | `Не удалось загрузить спринты` |
| `Failed to fetch sprint` | `Не удалось загрузить спринт` |
| `Failed to create sprint` | `Не удалось создать спринт` |
| `Failed to update sprint` | `Не удалось обновить спринт` |
| `Failed to delete sprint` | `Не удалось удалить спринт` |

#### taskItemsApi.ts
| Было | Стало |
|------|-------|
| `Failed to fetch task items` | `Не удалось загрузить задачи` |
| `Failed to create task item` | `Не удалось создать задачу` |
| `Failed to update task item` | `Не удалось обновить задачу` |
| `Failed to delete task item` | `Не удалось удалить задачу` |
| `Failed to move task item` | `Не удалось переместить задачу` |

#### useSprints.ts
| Было | Стало |
|------|-------|
| `Failed to load sprints` | `Не удалось загрузить спринты` |
| `Failed to create sprint` | `Не удалось создать спринт` |
| `Failed to update sprint` | `Не удалось обновить спринт` |
| `Failed to delete sprint` | `Не удалось удалить спринт` |

#### useSprintForm.ts
| Было | Стало |
|------|-------|
| `Operation failed` | `Операция не выполнена` |

#### useTaskItems.ts
| Было | Стало |
|------|-------|
| `Failed to load task items` | `Не удалось загрузить задачи` |
| `Failed to create task` | `Не удалось создать задачу` |
| `Failed to update task` | `Не удалось обновить задачу` |
| `Failed to delete task` | `Не удалось удалить задачу` |
| `Failed to move task` | `Не удалось переместить задачу` |

### Риски

| Риск | Митигация |
|------|-----------|
| Дублирование строк между API-слоем и хуками (одинаковые fallback-сообщения) | Допустимо — это разные слои с разной зоной ответственности. Выносить в константы = overengineering для текущего масштаба |

## Задачи

### T1: UI-компоненты и HTML (заголовки, плейсхолдеры)

**Files:** `src/frontend/index.html`, `src/frontend/src/sprints/pages/SprintBoardPage.tsx`, `src/frontend/src/sprints/components/SprintForm.tsx`

**Do:**
- `index.html`: `lang="en"` → `lang="ru"`, `<title>` → "AgileBoard — Доска задач"
- `SprintBoardPage.tsx`: "Sprint Board" → "Доска спринта"
- `SprintForm.tsx`: placeholder "Sprint 1" → "Спринт 1"

**Acceptance Criteria:**

**AC-1:** Открыть приложение → вкладка браузера показывает "AgileBoard — Доска задач"
**AC-2:** Главная страница → заголовок "Доска спринта"
**AC-3:** Форма создания спринта → placeholder "Спринт 1"

**Verify:**
```
npm --prefix src/frontend run build
```
Сборка успешна без ошибок.

**Size:** S

---

### T2: Сообщения об ошибках в API-слое

**Files:** `src/frontend/src/sprints/api/sprintsApi.ts`, `src/frontend/src/tasks/api/taskItemsApi.ts`

**Do:**
- `sprintsApi.ts`: 5 строк → русский (см. маппинг в Проектировании)
- `taskItemsApi.ts`: 5 строк → русский (см. маппинг в Проектировании)

**Acceptance Criteria:**

**AC-4:** Остановить backend (`docker-compose stop backend`) → открыть приложение → все ошибки загрузки на русском

**Verify:**
```
npm --prefix src/frontend run build
```
Сборка успешна без ошибок.

**Size:** S

---

### T3: Fallback-сообщения в хуках

**Files:** `src/frontend/src/sprints/hooks/useSprints.ts`, `src/frontend/src/sprints/hooks/useSprintForm.ts`, `src/frontend/src/tasks/hooks/useTaskItems.ts`

**Do:**
- `useSprints.ts`: 4 строки → русский
- `useSprintForm.ts`: "Operation failed" → "Операция не выполнена"
- `useTaskItems.ts`: 5 строк → русский

**Acceptance Criteria:**

**AC-5:** Остановить backend → попытка создать/обновить/удалить → fallback-сообщения об ошибках на русском

**Verify:**
```
npm --prefix src/frontend run build
```
Сборка успешна без ошибок.

**Size:** S

---

**Dependencies:** T1, T2, T3 независимы, можно выполнять в любом порядке.

## Критерии приёмки

**AC-1:** Открыть приложение → вкладка браузера показывает "AgileBoard — Доска задач"
**AC-2:** Главная страница → заголовок "Доска спринта"
**AC-3:** Форма создания спринта → placeholder "Спринт 1"
**AC-4:** Любая ошибка API → сообщение на русском языке
**AC-5:** Принудительная ошибка (выключенный backend) → все fallback-сообщения на русском
