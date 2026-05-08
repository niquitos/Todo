# Фикс потери названия при переносе задачи

## Why
При переносе карточки из колонки в колонку происходит полное обновление задачи через PUT запрос. В текущей реализации в функции `handleDrop` вызывается `updateTask` с частичными данными, где `name` передаётся как пустая строка `''` вместо текущего значения. Backend сохраняет эти данные, и название задачи заменяется на пустую строку.

## What
При переносе задачи через drag-and-drop в запросе UPDATE должны передаваться все поля задачи: `name`, `description`, `columnType`, `position`, `sprintId`. Текущие значения `name` и `description` должны быть сохранены, а не заменены на пустые строки.

## Context

**Relevant files:**
- `src/frontend/src/sprints/pages/SprintBoardPage.tsx` — функция `handleDrop` передаёт `name: ''` в updateTask
- `src/frontend/src/tasks/hooks/useTaskItems.ts` — функция `update` передаёт DTO в API
- `src/backend/AgileBoard/Application/UseCases/Tasks/UpdateTaskItemCommand.cs` — обновляет все поля из DTO
- `src/backend/AgileBoard/Application/UseCases/Tasks/TaskItemDto.cs` — DTO для обновления

**Patterns to follow:**
- Frontend: при оптимистичном обновлении нужно сохранять неизменяемые поля
- Backend: UPDATE через PUT должен обновлять только переданные поля ИЛИ требовать все поля

**Key decisions already made:**
- Backend использует PUT запрос, который требует передачи всех полей (не PATCH)
- TaskItemDto для обновления содержит: Name, Description, ColumnType, Position, SprintId

## Требования

### Функциональные

**FR-1: Сохранение названия при переносе**
- При переносе задачи через drag-and-drop, поле `name` должно сохранять текущее значение
- При переносе не должно происходить изменение описания (если оно было)

**FR-2: Все поля в update запросе**
- При любом обновлении задачи через `updateTask`, должны передаваться все поля: name, description, columnType, position, sprintId
- Даже если меняется только одна колонка, остальные поля должны передаваться с текущими значениями

**FR-3: Редактирование задачи через форму**
- При редактировании через модальное окно (TaskForm), должны сохраняться все поля формы
- Если в форме изменяется только Description, поле Name должно остаться без изменений

### Нефункциональные

**NFR-1: Отсутствие потери данных**
- **Metric:** Никакие пользовательские данные (name, description) не должны теряться при операциях переноса/обновления

**NFR-2: Оптимистичные обновления**
- **Metric:** UI должен обновляться мгновенно, откатываться при ошибке

## Ограничения

**Must:**
- PUT запрос должен передавать все поля задачи
- Не использовать PATCH (частичное обновление)
- Сохранять текущие name и description при переносе

**Must not:**
- Не добавлять новых полей в DTO
- Не менять API контракты (оставить PUT с обязательными полями)

**Out of scope:**
- Изменение архитектуры backend на PATCH
- Добавление валидации на frontend

## Критерии приёмки

**AC-1:** Перенос задачи из колонки "В процессе" в "Сделаны" → название сохраняется
**AC-2:** Перенос задачи внутри колонки → название и описание сохраняются
**AC-3:** Редактирование задачи (изменение description) → название сохраняется
**AC-4:** При ошибке обновления → UI откатывается к предыдущему состоянию

## Задачи

### T1: Исправить handleDrop в SprintBoardPage.tsx

**Files:**
- `src/frontend/src/sprints/pages/SprintBoardPage.tsx` — функция `handleDrop`

**Do:**
- Изменить `handleDrop` чтобы передавать все поля задачи при обновлении
- Использовать текущие `name`, `description` из объекта задачи
- Пример: `updateTask(taskId, { name: task.name, description: task.description, columnType, position })`

**Acceptance Criteria:**

**AC-T1.1:** Перенос задачи между колонками сохраняет `name` и `description`
**AC-T1.2:** Перенос задачи внутри колонки сохраняет `name` и `description`

**Verify command (manual):**
```
npm --prefix src/frontend run dev
1. Создать задачу с названием "Тестовая задача"
2. Перетащить задачу из колонки "Новые" в "Сделаны"
3. Проверить, что название осталось "Тестовая задача"
```

**Size:** XS (~15 минут)

**Commit:**
```
git add src/frontend/src/sprints/pages/SprintBoardPage.tsx
git commit -m "T1: Fix handleDrop to pass all task fields on move"
```

---

### T2: Исправить update в useTaskItems.ts

**Files:**
- `src/frontend/src/tasks/hooks/useTaskItems.ts` — функция `update`

**Do:**
- Изменить функцию `update`, чтобы при обновлении передавать все поля задачи
- Использовать текущие `name`, `description` из tasks state, если они не переданы в DTO

**Acceptance Criteria:**

**AC-T2.1:** При обновлении только `columnType` — `name` и `description` сохраняются
**AC-T2.2:** При обновлении только `position` — `name` и `description` сохраняются

**Verify command (manual):**
```
npm --prefix src/frontend run dev
1. Создать задачу с названием и описанием
2. Перетащить внутри колонки (изменив position)
3. Проверить, что название и описание остались без изменений
```

**Dependencies:**
- **Blocks:** T1 (опционально — T1 уже решает проблему для handleDrop, но T2 делает update более надёжным)

**Size:** XS (~15 минут)

**Commit:**
```
git add src/frontend/src/tasks/hooks/useTaskItems.ts
git commit -m "T2: Fix update to preserve name and description on partial updates"
```

---

### T3: Ревью и объединение коммитов

**Do:**
- Проверить, что оба коммита работают корректно
- Объединить коммиты в один если требуется

**Verify command:**
```
git log --oneline -5
git diff HEAD~2 HEAD --stat
```

**Size:** XS (~10 минут)

**Commit:**
```
git log --oneline -2
```
