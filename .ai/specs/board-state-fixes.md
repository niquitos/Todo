# board-state-fixes

## Why
Несколько багов в UX доски: состояние не сохраняется между перезагрузками, перенос задач не обновляет видимость, создание задачи показывает её не на том спринте, доска слишком широкая.

## What
Исправить 4 проблемы: persistence активного спринта, синхронизация задач после update, фильтрация при создании, ограничение ширины всего сайта (не только доски).

## Context

**Relevant files:**
- `src/frontend/src/sprints/pages/SprintBoardPage.tsx` — точка входа, хранит active sprint
- `src/frontend/src/sprints/hooks/useSprintBoard.ts` — хук для работы со спринтом и задачами
- `src/frontend/src/tasks/hooks/useTaskItems.ts` — хук для CRUD задач
- `src/frontend/src/sprints/components/SprintBoard.tsx` — рендер доски

**Patterns to follow:**
- Frontend feature-based structure: `types/api/hooks/components/pages`
- State management через React hooks (useState/useContext)

**Key decisions already made:**
- Спринты и задачи загружаются с бэкенда через API
- Единый источник правды — backend, frontend кэширует

## Требования

### Функциональные

**FR-1: Persistence активного спринта**
- При перезагрузке страницы восстанавливается последний активный спринт
- Если сохранённого нет — показывается первый спринт из списка

**FR-2: Синхронизация задач после переноса**
- При обновлении задачи (перенос на другой спринт) задача удаляется с текущей доски и появляется на целевой
- После успешного API-запроса по обновлению, данные доски перезапрашиваются для получения актуального состояния

**FR-3: Фильтрация задач при создании**
- При создании задачи через модалку, если выбран спринт отличный от текущего, задача появляется на целевой доске и не показывается на текущей.
- Если выбран текущий спринт — задача появляется на доске

**FR-4: Ограничение ширины сайта**
- Максимальная ширина сайта: 1200px
- Центрирование на экране при ширине > 1200px

### Нефункциональные

**NFR-1: Производительность**
- Перезапрос данных после мутаций не должен блокировать UI
- **Metric:** < 100ms на инвалидацию кэша

## Ограничения

**Must:**
- Не менять API-контракты бэкенда
- Сохранять функциональность (drag-n-drop, модалки)

**Must not:**
- Не добавлять новые зависимости (только React built-ins + localStorage)
- Не трогать backend-логику

**Out of scope:**
- Исправление багов drag-n-drop (только видимость/фильтрация)
- Персистентность position задач

## Критерии приёмки

**AC-1:** Перезагрузка страницы → активный спринт сохраняется и восстанавливается
**AC-2:** Перенос задачи на другой спринт → задача исчезает с текущей доски и появляется на целевой
**AC-3:** Создание задачи для другого спринта → после создания пользователь видит текущий спринт без созданной задачи
**AC-4:** Ширина сайта ≤ 1200px, содержимое центрировано

## Проектирование

### Стек

**Выбранные технологии:**
- **URL Query Parameter** (`?sprint=<id>`) — хранение активного спринта; нативно поддерживается браузером, survives перезагрузку, shareable между вкладками
- **React `useSearchParams`** — чтение/запись query параметров без побочных эффектов
- **CSS `max-width + margin: auto`** — ограничение ширины сайта (1200px)

**Альтернативы (отклонены):**
- `localStorage` — не shareable между вкладками, требует cleanup, усложняет deep-link
- `sessionStorage` — то же что localStorage, но теряется при закрытии вкладки
- React Context — избыточно для одного параметра, требует Provider wrapper

### Паттерны

**Pattern: URL as Source of Truth**
- **Problem:** активный спринт должен персиститься между перезагрузками и быть shareable
- **Solution:** `?sprint=<id>` в URL; React Router `useSearchParams` для чтения/записи
- **Location:** `src/frontend/src/sprints/pages/SprintBoardPage.tsx`

Мутации (update, create) вызывают refetch спринта для синхронизации UI с сервером (`useSprintBoard`).

### API-контракты

Нет новых эндпоинтов — используются существующие `GET /api/sprints/{id}`, `POST/PUT/DELETE /api/sprints/{sprintId}/tasks/*`.

### Изменения в коде

**`SprintBoardPage.tsx`:**
```tsx
// До:
const [activeSprint, setActiveSprint] = useState<Sprint | null>(null);

// После:
const [searchParams, setSearchParams] = useSearchParams();
const sprintId = searchParams.get('sprint');
// sprintId !== null → загрузить спринт с id=sprintId
// sprintId === null → загрузить первый спринт
```

**`SprintBoardPage.tsx`:**
```tsx
// Обернуть весь контент в div с max-width: 1200px; margin: auto;
```

### Этапы реализации

**Stage 1:** Добавить `useSearchParams` в `SprintBoardPage`, реализовать восстановление и смену sprintId через URL
**Stage 2:** Добавить `max-width: 1200px` к `SprintBoardPage`, центрирование
**Stage 3:** Интеграционное тестирование: перезагрузка, создание задачи, перенос

## Задачи

### T1: Persist активный спринт через URL query parameter

**Files:** `src/frontend/src/sprints/pages/SprintBoardPage.tsx`

**Do:**
- [Добавить `useSearchParams` из `react-router-dom`]
- [При загрузке: `sprintId = searchParams.get('sprint')`]
- [Если `sprintId !== null` — загрузить спринт с этим id]
- [Если `sprintId === null` — загрузить первый спринт]
- [При выборе спринта: `setSearchParams({ sprint: newId })`]

**Acceptance Criteria:**

**AC-1:** Перезагрузка страницы → URL содержит `?sprint=<id>`, показывается этот спринт

**Test Cases:**

#### Test-1: Восстановление из URL
**Type:** E2E
**Links:** AC-1

**Preconditions:**
- Открыта доска со спринтом A
- URL: `http://localhost:3000/?sprint=<sprintA_id>`

**Action:**
```
Перезагрузить страницу (F5)
```

**Expected:**
- Загружается спринт A (тот же что до перезагрузки)

**Verify command:**
```
Открыть http://localhost:3000/?sprint=<id>
Проверить что отображается correct board
```

#### Test-2: Fallback на первый спринт
**Type:** E2E
**Links:** AC-1

**Preconditions:**
- Открыта доска без query параметра (`http://localhost:3000/`)

**Action:**
```
Перезагрузить страницу
```

**Expected:**
- Отображается первый спринт из списка

**Verify command:**
```
Открыть http://localhost:3000/
Проверить что показывается первый спринт
```

#### Test-3: Смена спринта обновляет URL
**Type:** E2E
**Links:** AC-1

**Preconditions:**
- Открыта доска спринта A

**Action:**
```
Выбрать спринт B из селекта
```

**Expected:**
- URL обновляется на `?sprint=<sprintB_id>`
- Отображается спринт B

**Verify command:**
```
Выбрать спринт B
Проверить что URL содержит sprint B id
```

**Dependencies:**
- **Blocks:** T3, T4
- **Blocked by:** —

**Size:** M (~2 часа)

**Git:**
```
git add src/frontend/src/sprints/pages/SprintBoardPage.tsx
git commit -m "persist active sprint via URL query parameter"
```

---

### T2: Ограничить ширину сайта до 1200px

**Files:** `src/frontend/src/sprints/pages/SprintBoardPage.tsx`

**Do:**
- [Обернуть весь контент в `div` с `max-width: 1200px`]
- [Добавить `margin: auto` для центрирования]

**Acceptance Criteria:**

**AC-4:** Ширина сайта ≤ 1200px, содержимое центрировано

**Test Cases:**

#### Test-1: Сайт центрирован на широком экране
**Type:** Validation
**Links:** AC-4

**Preconditions:**
- Браузер шириной > 1200px

**Action:**
```
Открыть доску
```

**Expected:**
- Сайт шириной ≤ 1200px
- По бокам равные отступы (центрирование)

**Verify command:**
```
CSS: max-width: 1200px; margin: auto;
DevTools: измерить ширину контейнера
```

#### Test-2: Сайт не ограничена на узком экране
**Type:** Validation
**Links:** AC-4

**Preconditions:**
- Браузер шириной < 1200px

**Action:**
```
Открыть доску
```

**Expected:**
- Сайт занимает всю ширину экрана (100%)

**Verify command:**
```
DevTools: проверить что max-width не применяется
```

**Dependencies:**
- **Blocks:** —
- **Blocked by:** —

**Size:** S (~30 минут)

**Git:**
```
git add src/frontend/src/sprints/pages/SprintBoardPage.tsx
git commit -m "limit board width to 1200px with centering"
```

---

### T3: Перезапрашивать спринт после мутаций (update, create)

**Files:** `src/frontend/src/sprints/hooks/useSprintBoard.ts`, `src/frontend/src/tasks/hooks/useTaskItems.ts`

**Do:**
- [После успешного `PUT /api/sprints/{sprintId}/tasks` — вызвать refetch спринта]
- [После успешного `POST /api/sprints/{sprintId}/tasks` — вызвать refetch спринта]

**Acceptance Criteria:**

**AC-2:** Перенос задачи на другой спринт → задача исчезает с текущей доски и появляется на целевой
**AC-3:** Создание задачи для другого спринта → после создания пользователь видит текущий спринт без созданной задачи

**Test Cases:**

#### Test-1: Перенос задачи обновляет доску
**Type:** Integration
**Links:** AC-2

**Preconditions:**
- Спринт A с задачей T1
- Спринт B существует

**Action:**
```
PUT /api/sprints/{sprintA_id}/tasks
  { targetSprintId: sprintB_id, targetColumn: New, position: 0 }
```

**Expected:**
- Задача T1 больше не на доске A
- Задача T1 появляется на доске B

**Verify command:**
```
http PUT /api/sprints/{sprintA_id}/tasks targetSprintId={sprintB_id}
GET /api/sprints/{sprintA_id} → tasks не содержит task1_id
GET /api/sprints/{sprintB_id} → tasks содержит task1_id
```

#### Test-2: Создание задачи для текущего спринта
**Type:** Integration
**Links:** AC-3

**Preconditions:**
- Открыта доска спринта A

**Action:**
```
POST /api/sprints/{sprintA_id}/tasks
  { title: "New Task", column: New, position: 0 }
```

**Expected:**
- Задача появляется на доске A

**Verify command:**
```
POST /api/sprints/{sprintA_id}/tasks { title: "Test", column: New, position: 0 }
GET /api/sprints/{sprintA_id} → tasks содержит новую задачу
```

#### Test-3: Создание задачи для другого спринта
**Type:** Integration
**Links:** AC-3

**Preconditions:**
- Открыта доска спринта A

**Action:**
```
POST /api/sprints/{sprintB_id}/tasks
  { title: "New Task", column: New, position: 0 }
```

**Expected:**
- Задача не появляется на доске A
- При переходе на спринт B — задача видна

**Verify command:**
```
POST /api/sprints/{sprintB_id}/tasks { title: "Test", column: New, position: 0 }
GET /api/sprints/{sprintA_id} → tasks не содержит новую задачу
GET /api/sprints/{sprintB_id} → tasks содержит новую задачу
```

**Dependencies:**
- **Blocks:** —
- **Blocked by:** T1

**Size:** M (~2 часа)

**Git:**
```
git add src/frontend/src/sprints/hooks/useSprintBoard.ts src/frontend/src/tasks/hooks/useTaskItems.ts
git commit -m "refetch sprint after task create/update for sync"
```

