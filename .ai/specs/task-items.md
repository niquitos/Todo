# Задачи (Task Items)

## Why
Пользователь не может планировать работу в спринтах без задач. Доска с тремя пустыми колонками бесполезна — нужны задачи, которые можно перетаскивать между колонками и упорядочивать внутри них.

## What
Полноценное управление задачами внутри спринта: создание, редактирование, удаление, перетаскивание между колонками (New → InProgress → Done) и изменение порядка внутри колонки. Задачи фильтруются по активному спринту.

## Context

**Relevant files:**
- `src/backend/AgileBoard/Domain/Sprint.cs` — текущий Aggregate Root, TaskItem будет ссылаться на SprintId
- `src/backend/AgileBoard/Adapters/Persistence/AppDbContext.cs` — добавится DbSet<TaskItem>
- `src/backend/AgileBoard/Adapters/WebApi/Controllers/SprintController.cs` — образец контроллера
- `src/backend/AgileBoard/Adapters/Persistence/Repositories/SprintRepository.cs` — образец репозитория
- `src/frontend/src/sprints/pages/SprintBoardPage.tsx` — сюда добавится интеграция с задачами
- `src/frontend/src/sprints/components/SprintColumns.tsx` — замена заглушек на реальные задачи
- `src/frontend/src/sprints/components/SprintColumn.tsx` — drag-and-drop target
- `src/frontend/src/sprints/sprint-board.css` — стили карточек задач и drag-and-drop

**Patterns to follow:**
- CQRS: `IRequest<T>` для запросов, `IRequest` для команд (MediatR), образец в `CreateSprintCommand.cs`
- Repository: интерфейс + реализация, образец в `ISprintRepository.cs` / `SprintRepository.cs`
- DDD Lite: Aggregate Root с factory method и инкапсуляцией, образец в `Sprint.cs`
- TDD: тесты пишутся до реализации (Red → Green → Refactor)
- Frontend feature-structure: `types/`, `api/`, `hooks/`, `components/` в `src/frontend/src/tasks/`
- Модальные окна и формы: существующие стили `.modal-overlay`, `.modal`, `.form-group` в `sprint-board.css`

**Key decisions already made:**
- Backend: ASP.NET Core 8 Controllers + EF Core 8 + MediatR 12.2 + PostgreSQL
- Frontend: React 19 + TypeScript + Vite, без внешних UI-библиотек
- Drag-and-drop: нативный HTML5 Drag and Drop API (без новых зависимостей)
- 3 фиксированные колонки: "Новые" (New), "В процессе" (InProgress), "Сделаны" (Done)
- Колонки не хранятся в БД — это enum на backend и константы на frontend
- Frontend тесты — out of scope (ручная проверка)
- Позиция задачи внутри колонки определяется полем `Position` (int)

## Требования

### Функциональные

**FR-1: Сущность TaskItem**
- Поля: Id, Name (обязательно), Description (опционально), SprintId, ColumnType (New/InProgress/Done), Position (int)
- Position определяет порядок внутри колонки (0 = верх)

**FR-2: Список задач спринта**
- При выборе спринта загружаются только задачи этого спринта
- Задачи распределены по колонкам согласно ColumnType
- Внутри колонки задачи отсортированы по Position

**FR-3: Создание задачи**
- Кнопка "+" в заголовке каждой колонки
- Модальное окно с полями: Name (обязательно), Description (опционально)
- По умолчанию SprintId = активный спринт, ColumnType = колонка вызова
- Созданная задача появляется сверху колонки (Position = 0, остальные сдвигаются)

**FR-4: Редактирование задачи**
- Кнопка-карандаш на карточке задачи
- Модальное окно: изменение Name, Description
- Сохранение изменений

**FR-5: Удаление задачи**
- Кнопка-урна на карточке задачи
- Диалог подтверждения перед удалением
- После удаления — задача исчезает из колонки, позиции пересчитываются

**FR-6: Перетаскивание между колонками**
- Нативный HTML5 Drag and Drop
- При переносе в другую колонку меняется ColumnType и Position
- Задача вставляется в позицию, куда её бросили

**FR-7: Перетаскивание внутри колонки**
- Изменение порядка задач через drag-and-drop
- Обновление Position у затронутых задач

**FR-8: Счётчик задач**
- В заголовке каждой колонки отображается количество задач

### Нефункциональные

**NFR-1: Производительность**
- **Metric:** CRUD операции < 200ms
- **Metric:** Обновление позиций после перетаскивания < 300ms

**NFR-2: Drag-and-drop UX**
- **Metric:** Визуальная обратная связь при перетаскивании (подсветка целевой позиции)

**NFR-3: Оптимистичные обновления**
- **Metric:** UI обновляется до ответа сервера, откатывается при ошибке

## Ограничения

**Must:**
- CQRS для всех операций (MediatR)
- Repository pattern для доступа к данным
- Aggregate Root TaskItem с инкапсулированной логикой
- TDD для всего backend кода
- Нативный HTML5 Drag and Drop API (без библиотек)
- Переиспользовать существующие стили модальных окон и форм

**Must not:**
- Не добавлять новых NuGet/NPM пакетов
- Не создавать сущность "Колонка" в БД (ColumnType — enum)
- Не менять структуру Sprint или SprintController
- Не добавлять WebSocket/SignalR (оптимистичные обновления через fetch)

**Out of scope:**
- Валидация (кроме обязательности Name)
- Drag-and-drop на мобильных (touch events)
- Анимации появления/исчезновения
- Множественное выделение задач
- Цветовые метки/теги задач
- Назначение ответственных
- Frontend автотесты

## Критерии приёмки

**AC-1:** Кнопка "+" в заголовке колонки → открывается модальное окно создания задачи
**AC-2:** Создание задачи → задача появляется в колонке, счётчик обновляется
**AC-3:** Переключение спринта → отображаются только задачи выбранного спринта
**AC-4:** Перетаскивание задачи в другую колонку → задача перемещается, счётчики обновляются
**AC-5:** Перетаскивание внутри колонки → порядок задач изменяется
**AC-6:** Кнопка-карандаш → открывается модальное окно редактирования
**AC-7:** Кнопка-урна → диалог подтверждения → задача удаляется
**AC-8:** Счётчик показывает актуальное количество задач в каждой колонке

## Проектирование

### Стек

**Выбранные технологии:**
- **Backend:** ASP.NET Core 8 Controllers + MediatR 12.2 — соответствует существующей архитектуре (CQRS)
- **Database:** PostgreSQL 15+ — существующая БД проекта
- **ORM:** EF Core 8 — Code First, миграции
- **Testing:** NUnit 3.14 + Moq 4.20 — unit и integration тесты
- **Frontend:** React 19 + TypeScript + Vite — без новых зависимостей
- **Drag-and-drop:** Нативный HTML5 Drag and Drop API — не требует пакетов

**Альтернативы (отклонены):**
- **react-beautiful-dnd / @dnd-kit** — отклонено: ограничение проекта «не добавлять новых NPM пакетов»
- **Отдельный контроллер для перемещения** — рассмотрено и принято: endpoint `/move` внутри TaskItemController для операций перетаскивания
- **Хранение колонок в БД** — отклонено: 3 фиксированные колонки, достаточно enum

### Архитектура

#### Компоненты backend

| Компонент | Ответственность | Интерфейсы |
|-----------|-----------------|------------|
| TaskItemController | HTTP endpoints для задач | GET/POST/PUT/DELETE `/api/sprints/{sprintId}/tasks`, PUT `/move` |
| CreateTaskItemCommand | Создание задачи + сдвиг позиций | `IRequest<Guid>` |
| UpdateTaskItemCommand | Обновление Name/Description | `IRequest` |
| DeleteTaskItemCommand | Удаление + пересчёт позиций | `IRequest` |
| MoveTaskItemCommand | Смена колонки и/или позиции | `IRequest` |
| GetTaskItemsQuery | Список задач спринта | `IRequest<IEnumerable<TaskItemDto>>` |
| ITaskItemRepository | Доступ к задачам в БД | GetBySprintId, GetById, Add, Update, Delete, GetMaxPosition |
| TaskItem | Aggregate Root | Create(), Update(), Move() |

#### Компоненты frontend

| Компонент | Ответственность | Интерфейсы |
|-----------|-----------------|------------|
| SprintBoardPage | Интеграция задач в доску, модальные окна | Стейт для create/edit/delete/confirm |
| SprintColumns | Распределение задач по колонкам, кнопка "+" | Props: sprintId, taskItems, callbacks |
| SprintColumn | Рендер карточек, drag-and-drop зона | Props: columnType, tasks, onDrop, onAddClick |
| TaskCard | Карточка задачи с кнопками ✏️/🗑️ | Props: task, onEdit, onDelete |
| TaskForm | Форма создания/редактирования | Props: task?, onSubmit, onSuccess |
| ConfirmDialog | Диалог подтверждения удаления | Props: message, onConfirm, onCancel |
| useTaskItems | Хук управления задачами + оптимистичные обновления | Возвращает tasks, create, update, remove, move |

#### Поток данных

```
[Drag on Frontend] → [Оптимистичное обновление UI] → [PUT /api/sprints/{sprintId}/tasks/move]
                                                              ↓
                                                     [MoveTaskItemHandler]
                                                              ↓
                                                    [Пересчёт Position в БД]
                                                              ↓
                                                    [200 OK / 400 Error → откат UI]
```

```
[Кнопка "+" в колонке] → [Модальное окно TaskForm] → [POST /api/sprints/{sprintId}/tasks]
                                                              ↓
                                                     [CreateTaskItemHandler]
                                                              ↓
                                          [TaskItem.Create(name, desc, sprintId, columnType, position=0)]
                                                              ↓
                                                    [Сдвиг существующих задач на +1]
```

### Паттерны

**Pattern 1: CQRS для задач (MediatR)**
- **Problem:** Разделение операций чтения и записи
- **Solution:** `IRequest<T>` для GetTaskItemsQuery, `IRequest` для команд Create/Update/Delete/Move
- **Location:** `src/backend/AgileBoard/Application/UseCases/Tasks/`

**Pattern 2: Repository для TaskItem**
- **Problem:** Абстракция доступа к данным
- **Solution:** `ITaskItemRepository` с методами GetBySprintId, GetById, Add, Update, Delete, GetMaxPosition
- **Location:** `src/backend/AgileBoard/Adapters/Persistence/Repositories/`

**Pattern 3: Aggregate Root TaskItem**
- **Problem:** Инкапсуляция бизнес-логики задачи
- **Solution:** Методы `Create()`, `Update()`, `Move()` внутри TaskItem. Position управляется командами, а не самим агрегатом (т.к. требует знания о других задачах)
- **Location:** `src/backend/AgileBoard/Domain/TaskItem.cs`

**Pattern 4: Оптимистичные обновления (Frontend)**
- **Problem:** Мгновенный отклик UI при перетаскивании
- **Solution:** Изменение локального состояния до отправки API-запроса. При ошибке — откат к предыдущему состоянию
- **Location:** `src/frontend/src/tasks/hooks/useTaskItems.ts`

**Pattern 5: Position management (Backend)**
- **Problem:** Вставка задачи в произвольную позицию колонки
- **Solution:** При создании — Position=0, все существующие +1. При перемещении — сдвиг задач в исходной колонке вниз, в целевой — вверх от новой позиции. При удалении — сдвиг оставшихся вниз
- **Location:** Handlers команд CreateTaskItem, MoveTaskItem, DeleteTaskItem

### API-контракты

```
GET /api/sprints/{sprintId}/tasks
Response: [{ "id": "uuid", "name": "string", "description": "string?", "sprintId": "uuid", "columnType": "New|InProgress|Done", "position": 0 }]
Status: 200 OK

POST /api/sprints/{sprintId}/tasks
Request: { "name": "string", "description": "string?", "columnType": "New|InProgress|Done" }
Response: { "id": "uuid", "name": "string", "description": "string?", "sprintId": "uuid", "columnType": "...", "position": 0 }
Status: 201 Created

PUT /api/sprints/{sprintId}/tasks/{taskId}
Request: { "name": "string", "description": "string?" }
Response: 204 No Content

DELETE /api/sprints/{sprintId}/tasks/{taskId}
Response: 204 No Content

PUT /api/sprints/{sprintId}/tasks/move
Request: { "taskId": "uuid", "newColumnType": "New|InProgress|Done", "newPosition": 0 }
Response: 204 No Content
```

**Обработка ошибок:**
- `NotFoundException` → 404 Not Found (через существующий ExceptionHandlingMiddleware)

### Миграция

```sql
CREATE TABLE "TaskItems" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(1000) NULL,
    "SprintId" uuid NOT NULL,
    "ColumnType" character varying(20) NOT NULL,
    "Position" integer NOT NULL,
    CONSTRAINT "PK_TaskItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TaskItems_Sprints_SprintId" FOREIGN KEY ("SprintId") REFERENCES "Sprints"("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_TaskItems_SprintId_ColumnType_Position" ON "TaskItems" ("SprintId", "ColumnType", "Position");
```

### Риски

| Риск | Вероятность | Влияние | Митигация |
|------|-------------|---------|-----------|
| Гонки при быстрых перетаскиваниях | Средняя | Среднее | Оптимистичные обновления + откат при конфликте |
| Несогласованность Position | Низкая | Высокое | Все операции Position в одной транзакции EF Core |
| Сложность нативного DnD | Средняя | Среднее | Чёткий контракт onDragStart/onDragOver/onDrop, минимальная логика |
| Каскадное удаление sprint → tasks | Низкая | Высокое | ON DELETE CASCADE в миграции |

### Этапы реализации

**Stage 1:** Domain — TaskItemId + ColumnType + TaskItem aggregate root + unit тесты
**Stage 2:** Persistence — TaskItemConfiguration, ITaskItemRepository, TaskItemRepository, миграция + integration тесты
**Stage 3:** Application — CQRS команды и запросы + unit тесты
**Stage 4:** WebApi — TaskItemController + integration тесты
**Stage 5:** Frontend — TaskItem types, API, useTaskItems hook
**Stage 6:** Frontend — TaskCard, TaskForm, ConfirmDialog компоненты
**Stage 7:** Интеграция — модификация SprintColumns/SprintColumn/SprintBoardPage, drag-and-drop

### Docker-развёртывание

Изменения в Docker-конфигурации не требуются. Новая миграция применяется автоматически через `EnsureCreated()` при старте backend-контейнера. Frontend собирается штатно через multi-stage Dockerfile (node build → nginx). Порты и сетевые настройки без изменений.

**Проверка деплоя:**
```
docker-compose up --build -d
docker-compose ps                    # 3 сервиса в статусе "Up"
curl http://localhost:5000/api/sprints               # Спринты доступны
curl http://localhost:5000/api/sprints/{id}/tasks     # Задачи доступны
```

## Задачи

### T1: Доменная модель TaskItem Aggregate

**Files:** 
- `src/backend/AgileBoard/Domain/ColumnType.cs` — enum (New, InProgress, Done)
- `src/backend/AgileBoard/Domain/TaskItemId.cs` — value object wrapper над Guid
- `src/backend/AgileBoard/Domain/TaskItem.cs` — Aggregate Root
- `src/backend/AgileBoard.Tests/Unit/Domain/TaskItemTests.cs` — unit тесты

**Do:**
- Создать enum `ColumnType` со значениями: `New`, `InProgress`, `Done`
- Создать value object `TaskItemId(Guid Value)` с `New()` — по образцу `SprintId`
- Создать aggregate root `TaskItem` с полями: Id, Name, Description, SprintId, ColumnType, Position
- Добавить private parameterless constructor для EF Core
- Добавить factory method `TaskItem.Create(name, description, sprintId, columnType, position)`
- Добавить метод `Update(name, description)`
- Добавить метод `Move(columnType, position)` — меняет колонку и позицию

**Acceptance Criteria:**

**AC-1.1:** `TaskItem.Create` с валидными данными → создаёт агрегат с присвоенным Id
**AC-1.2:** `TaskItem.Update` изменяет Name и Description
**AC-1.3:** `TaskItem.Move` изменяет ColumnType и Position
**AC-1.4:** ColumnType enum содержит 3 значения: New, InProgress, Done

**Test Cases:**

#### Test-1: TaskItem_Create_WithValidData_AssignsId
**Type:** Unit
**Links:** AC-1.1

**Preconditions:** —

**Action:**
```csharp
var task = TaskItem.Create("Task 1", "Description", new SprintId(guid), ColumnType.New, 0);
```

**Expected:**
```
task.Id.Value != Guid.Empty
task.Name == "Task 1"
task.Description == "Description"
task.SprintId.Value == guid
task.ColumnType == ColumnType.New
task.Position == 0
```

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~TaskItem_Create_WithValidData_AssignsId"
```

#### Test-2: TaskItem_Update_ChangesNameAndDescription
**Type:** Unit
**Links:** AC-1.2

**Preconditions:**
- Задача создана через `TaskItem.Create`

**Action:**
```csharp
task.Update("New Name", "New Description");
```

**Expected:**
```
task.Name == "New Name"
task.Description == "New Description"
```

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~TaskItem_Update_ChangesNameAndDescription"
```

#### Test-3: TaskItem_Move_ChangesColumnAndPosition
**Type:** Unit
**Links:** AC-1.3

**Preconditions:**
- Задача создана с ColumnType.New, Position=0

**Action:**
```csharp
task.Move(ColumnType.InProgress, 2);
```

**Expected:**
```
task.ColumnType == ColumnType.InProgress
task.Position == 2
```

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~TaskItem_Move_ChangesColumnAndPosition"
```

**Dependencies:**
- **Blocks:** T2
- **Blocked by:** —

**Size:** S (~1 час)

**Commit:**
```
git add src/backend/AgileBoard/Domain/ColumnType.cs src/backend/AgileBoard/Domain/TaskItemId.cs src/backend/AgileBoard/Domain/TaskItem.cs src/backend/AgileBoard.Tests/Unit/Domain/TaskItemTests.cs
git commit -m "T1: Add TaskItem aggregate root with ColumnType enum"
```

---

### T2: Репозиторий и миграция БД для TaskItem

**Files:**
- `src/backend/AgileBoard/Adapters/Persistence/Configurations/TaskItemConfiguration.cs` — EF Core маппинг
- `src/backend/AgileBoard/Adapters/Persistence/Repositories/ITaskItemRepository.cs` — интерфейс
- `src/backend/AgileBoard/Adapters/Persistence/Repositories/TaskItemRepository.cs` — реализация
- `src/backend/AgileBoard/Adapters/Persistence/AppDbContext.cs` — добавить `DbSet<TaskItem>`
- `src/backend/AgileBoard/Adapters/Persistence/DependencyInjection.cs` — зарегистрировать `ITaskItemRepository`
- `src/backend/AgileBoard/Adapters/Persistence/Migrations/<timestamp>_CreateTaskItemsTable.cs` — миграция
- `src/backend/AgileBoard.Tests/Integration/Persistence/TaskItemRepositoryTests.cs` — integration тесты

**Do:**
- Создать `TaskItemConfiguration` с маппингом: Id (HasConversion), Name (required, max 200), Description (max 1000), SprintId (HasConversion), ColumnType (HasConversion string), Position (required), индекс (SprintId, ColumnType, Position)
- Создать `ITaskItemRepository` с методами: GetBySprintIdAsync, GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync, GetMaxPositionAsync
- Реализовать `TaskItemRepository` по образцу `SprintRepository`
- Добавить `DbSet<TaskItem>` в `AppDbContext` и `ApplyConfiguration(new TaskItemConfiguration())`
- Зарегистрировать `ITaskItemRepository` как Scoped в `DependencyInjection.AddPersistence()`
- Создать и применить EF миграцию

**Acceptance Criteria:**

**AC-2.1:** `AddAsync` сохраняет задачу в БД
**AC-2.2:** `GetByIdAsync` возвращает задачу по Id
**AC-2.3:** `GetBySprintIdAsync` возвращает задачи спринта, отсортированные по Position
**AC-2.4:** `UpdateAsync` обновляет задачу в БД
**AC-2.5:** `DeleteAsync` удаляет задачу из БД
**AC-2.6:** `GetMaxPositionAsync` возвращает максимальный Position для колонки спринта (-1 если пусто)
**AC-2.7:** Миграция создаёт таблицу TaskItems с FK на Sprints (ON DELETE CASCADE)

**Test Cases:**

#### Test-1: TaskItemRepository_Add_ThenGetById_ReturnsTaskItem
**Type:** Integration
**Links:** AC-2.1, AC-2.2

**Preconditions:**
- InMemoryDatabase инициализирована
- Спринт существует в БД

**Action:**
```csharp
var task = TaskItem.Create("Task 1", "Desc", sprint.Id, ColumnType.New, 0);
await repository.AddAsync(task);
var found = await repository.GetByIdAsync(task.Id);
```

**Expected:**
```
found != null
found.Id == task.Id
found.Name == "Task 1"
```

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~TaskItemRepository_Add_ThenGetById_ReturnsTaskItem"
```

#### Test-2: TaskItemRepository_GetBySprintId_ReturnsOrderedByPosition
**Type:** Integration
**Links:** AC-2.3

**Preconditions:**
- Спринт существует в БД
- Добавлены задачи с Position 2, 0, 1

**Action:**
```csharp
var tasks = await repository.GetBySprintIdAsync(sprint.Id);
```

**Expected:**
```
tasks.Count == 3
tasks[0].Position == 0
tasks[1].Position == 1
tasks[2].Position == 2
```

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~TaskItemRepository_GetBySprintId_ReturnsOrderedByPosition"
```

#### Test-3: TaskItemRepository_Update_ThenGetById_ReturnsUpdated
**Type:** Integration
**Links:** AC-2.4

**Preconditions:**
- Задача сохранена в БД

**Action:**
```csharp
task.Update("Updated", "New Desc");
await repository.UpdateAsync(task);
var found = await repository.GetByIdAsync(task.Id);
```

**Expected:**
```
found.Name == "Updated"
found.Description == "New Desc"
```

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~TaskItemRepository_Update_ThenGetById_ReturnsUpdated"
```

#### Test-4: TaskItemRepository_Delete_ThenGetById_ReturnsNull
**Type:** Integration
**Links:** AC-2.5

**Preconditions:**
- Задача сохранена в БД

**Action:**
```csharp
await repository.DeleteAsync(task.Id);
var found = await repository.GetByIdAsync(task.Id);
```

**Expected:**
```
found == null
```

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~TaskItemRepository_Delete_ThenGetById_ReturnsNull"
```

#### Test-5: TaskItemRepository_GetMaxPosition_EmptyColumn_ReturnsMinusOne
**Type:** Integration
**Links:** AC-2.6

**Preconditions:**
- Спринт существует, колонка пуста

**Action:**
```csharp
var maxPos = await repository.GetMaxPositionAsync(sprint.Id, ColumnType.New);
```

**Expected:**
```
maxPos == -1
```

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~TaskItemRepository_GetMaxPosition_EmptyColumn_ReturnsMinusOne"
```

**Dependencies:**
- **Blocked by:** T1
- **Blocks:** T3

**Size:** M (~2 часа)

**Commit:**
```
git add src/backend/AgileBoard/Adapters/Persistence/Configurations/TaskItemConfiguration.cs src/backend/AgileBoard/Adapters/Persistence/Repositories/ITaskItemRepository.cs src/backend/AgileBoard/Adapters/Persistence/Repositories/TaskItemRepository.cs src/backend/AgileBoard/Adapters/Persistence/AppDbContext.cs src/backend/AgileBoard/Adapters/Persistence/DependencyInjection.cs src/backend/AgileBoard/Adapters/Persistence/Migrations/ src/backend/AgileBoard.Tests/Integration/Persistence/TaskItemRepositoryTests.cs
git commit -m "T2: Add TaskItemRepository and CreateTaskItemsTable migration"
```

---

### T3: CQRS команды и запросы для TaskItem

**Files:**
- `src/backend/AgileBoard/Application/UseCases/Tasks/TaskItemDto.cs` — DTO
- `src/backend/AgileBoard/Application/UseCases/Tasks/CreateTaskItemCommand.cs`
- `src/backend/AgileBoard/Application/UseCases/Tasks/UpdateTaskItemCommand.cs`
- `src/backend/AgileBoard/Application/UseCases/Tasks/DeleteTaskItemCommand.cs`
- `src/backend/AgileBoard/Application/UseCases/Tasks/MoveTaskItemCommand.cs`
- `src/backend/AgileBoard/Application/UseCases/Tasks/GetTaskItemsQuery.cs`
- `src/backend/AgileBoard.Tests/Unit/UseCases/Tasks/CreateTaskItemCommandTests.cs`
- `src/backend/AgileBoard.Tests/Unit/UseCases/Tasks/UpdateTaskItemCommandTests.cs`
- `src/backend/AgileBoard.Tests/Unit/UseCases/Tasks/DeleteTaskItemCommandTests.cs`
- `src/backend/AgileBoard.Tests/Unit/UseCases/Tasks/MoveTaskItemCommandTests.cs`
- `src/backend/AgileBoard.Tests/Unit/UseCases/Tasks/GetTaskItemsQueryTests.cs`

**Do:**
- Создать DTO: `TaskItemDto` (c `FromTaskItem`), `CreateTaskItemDto`, `UpdateTaskItemDto`, `MoveTaskItemDto`
- `CreateTaskItemCommand` → создаёт задачу с Position=0, сдвигает существующие на +1
- `UpdateTaskItemCommand` → находит задачу, вызывает `Update()`, сохраняет
- `DeleteTaskItemCommand` → удаляет задачу, сдвигает оставшиеся в колонке вниз
- `MoveTaskItemCommand` → если колонка та же — переупорядочивает позиции; если другая — удаляет из исходной (сдвиг) и вставляет в целевую (сдвиг)
- `GetTaskItemsQuery(SprintId)` → возвращает все задачи спринта, отсортированные по ColumnType + Position
- Все хендлеры используют `ITaskItemRepository`

**Acceptance Criteria:**

**AC-3.1:** `CreateTaskItemCommand` с валидными данными → возвращает Id, задача имеет Position=0
**AC-3.2:** `CreateTaskItemCommand` сдвигает существующие задачи на +1
**AC-3.3:** `UpdateTaskItemCommand` → обновляет Name/Description
**AC-3.4:** `DeleteTaskItemCommand` → удаляет задачу и пересчитывает позиции
**AC-3.5:** `MoveTaskItemCommand` между колонками → меняет ColumnType, пересчитывает позиции в обеих колонках
**AC-3.6:** `MoveTaskItemCommand` внутри колонки → меняет только Position
**AC-3.7:** `GetTaskItemsQuery` → возвращает задачи спринта
**AC-3.8:** Все операции выбрасывают `NotFoundException` при несуществующем Id

**Test Cases:**

#### Test-1: CreateTaskItemCommand_ValidData_ReturnsId
**Type:** Unit
**Links:** AC-3.1

**Preconditions:**
- Mock репозитория возвращает пустой список

**Action:**
```csharp
var result = await handler.Handle(new CreateTaskItemCommand(sprintId, dto), CancellationToken.None);
```

**Expected:**
```
result != Guid.Empty
```

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~CreateTaskItemCommand_ValidData_ReturnsId"
```

#### Test-2: CreateTaskItemCommand_ShiftsExistingTasks
**Type:** Unit
**Links:** AC-3.2

**Preconditions:**
- Mock репозитория возвращает 2 существующие задачи (Position 0, 1)

**Action:**
```csharp
await handler.Handle(new CreateTaskItemCommand(sprintId, dto), CancellationToken.None);
```

**Expected:**
- `UpdateAsync` вызван для каждой существующей задачи
- Новые позиции: 1 и 2 (были 0 и 1)

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~CreateTaskItemCommand_ShiftsExistingTasks"
```

#### Test-3: UpdateTaskItemCommand_ValidData_UpdatesTask
**Type:** Unit
**Links:** AC-3.3

**Preconditions:**
- Mock репозитория возвращает существующую задачу

**Action:**
```csharp
await handler.Handle(new UpdateTaskItemCommand(taskId, sprintId, dto), CancellationToken.None);
```

**Expected:**
- `UpdateAsync` вызван 1 раз
- Задача имеет новые Name и Description

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~UpdateTaskItemCommand_ValidData_UpdatesTask"
```

#### Test-4: DeleteTaskItemCommand_DeletesAndShifts
**Type:** Unit
**Links:** AC-3.4

**Preconditions:**
- Mock репозитория возвращает задачу и 2 другие в той же колонке (Position 1, 2)

**Action:**
```csharp
await handler.Handle(new DeleteTaskItemCommand(taskId, sprintId), CancellationToken.None);
```

**Expected:**
- `DeleteAsync` вызван для задачи
- Оставшиеся задачи сдвинуты (Position 1→0, 2→1)

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~DeleteTaskItemCommand_DeletesAndShifts"
```

#### Test-5: MoveTaskItemCommand_BetweenColumns_RecalculatesBoth
**Type:** Unit
**Links:** AC-3.5

**Preconditions:**
- Задача в колонке New (Position 0)
- В колонке InProgress есть 1 задача (Position 0)

**Action:**
```csharp
await handler.Handle(new MoveTaskItemCommand(taskId, sprintId, ColumnType.InProgress, 1), CancellationToken.None);
```

**Expected:**
- ColumnType задачи = InProgress
- Position задачи = 1
- Исходная колонка: позиции пересчитаны
- Целевая колонка: задачи с Position >= 1 сдвинуты вниз

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~MoveTaskItemCommand_BetweenColumns_RecalculatesBoth"
```

#### Test-6: MoveTaskItemCommand_WithinColumn_Reorders
**Type:** Unit
**Links:** AC-3.6

**Preconditions:**
- 3 задачи в колонке New (Position 0, 1, 2)
- Перемещаем задачу с Position 0 на Position 2

**Action:**
```csharp
await handler.Handle(new MoveTaskItemCommand(taskId, sprintId, ColumnType.New, 2), CancellationToken.None);
```

**Expected:**
- Задача имеет Position 2
- Бывшие 1 и 2 стали 0 и 1

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~MoveTaskItemCommand_WithinColumn_Reorders"
```

**Dependencies:**
- **Blocked by:** T2
- **Blocks:** T4

**Size:** M (~3 часа)

**Commit:**
```
git add src/backend/AgileBoard/Application/UseCases/Tasks/ src/backend/AgileBoard.Tests/Unit/UseCases/Tasks/
git commit -m "T3: Add CQRS commands and queries for TaskItems"
```

---

### T4: TaskItemController с CRUD и Move endpoints

**Files:**
- `src/backend/AgileBoard/Adapters/WebApi/Controllers/TaskItemController.cs` — контроллер
- `src/backend/AgileBoard.Tests/Integration/TaskItemControllerTests.cs` — integration тесты

**Do:**
- Создать `TaskItemController` с маршрутом `api/sprints/{sprintId:guid}/tasks`
- `GET /` → `GetTaskItemsQuery` → 200 OK
- `POST /` → `CreateTaskItemCommand` → 201 Created
- `PUT /{taskId:guid}` → `UpdateTaskItemCommand` → 204 No Content
- `DELETE /{taskId:guid}` → `DeleteTaskItemCommand` → 204 No Content
- `PUT /move` → `MoveTaskItemCommand` → 204 No Content
- Обработка `NotFoundException` через существующий `ExceptionHandlingMiddleware`

**Acceptance Criteria:**

**AC-4.1:** `GET /api/sprints/{sprintId}/tasks` → 200 OK, массив задач
**AC-4.2:** `POST /api/sprints/{sprintId}/tasks` → 201 Created, тело задачи
**AC-4.3:** `PUT /api/sprints/{sprintId}/tasks/{taskId}` → 204 No Content
**AC-4.4:** `DELETE /api/sprints/{sprintId}/tasks/{taskId}` → 204 No Content
**AC-4.5:** `PUT /api/sprints/{sprintId}/tasks/move` → 204 No Content
**AC-4.6:** Несуществующий Id → 404 Not Found

**Test Cases:**

#### Test-1: GET_tasks_EmptySprint_ReturnsEmptyArray
**Type:** Integration
**Links:** AC-4.1

**Preconditions:**
- Backend запущен (TestServer)
- Спринт существует, задач нет

**Action:**
```
GET /api/sprints/{sprintId}/tasks
```

**Expected:**
- Status: 200 OK
- Body: []

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~GET_tasks_EmptySprint_ReturnsEmptyArray"
```

#### Test-2: POST_tasks_ValidData_Returns201Created
**Type:** Integration
**Links:** AC-4.2

**Preconditions:**
- Backend запущен (TestServer)
- Спринт существует

**Action:**
```
POST /api/sprints/{sprintId}/tasks
Body: { "name": "Task 1", "description": "Desc", "columnType": "New" }
```

**Expected:**
- Status: 201 Created
- Body содержит: id, name="Task 1", columnType="New", position=0

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~POST_tasks_ValidData_Returns201Created"
```

#### Test-3: PUT_tasks_move_ValidData_Returns204
**Type:** Integration
**Links:** AC-4.5

**Preconditions:**
- Backend запущен (TestServer)
- Спринт и задача существуют

**Action:**
```
PUT /api/sprints/{sprintId}/tasks/move
Body: { "taskId": "...", "newColumnType": "InProgress", "newPosition": 0 }
```

**Expected:**
- Status: 204 No Content

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~PUT_tasks_move_ValidData_Returns204"
```

#### Test-4: DELETE_tasks_ExistingId_Returns204
**Type:** Integration
**Links:** AC-4.4

**Preconditions:**
- Backend запущен (TestServer)
- Задача существует

**Action:**
```
DELETE /api/sprints/{sprintId}/tasks/{taskId}
```

**Expected:**
- Status: 204 No Content

**Verify command:**
```
dotnet test --filter "FullyQualifiedName~DELETE_tasks_ExistingId_Returns204"
```

**Dependencies:**
- **Blocked by:** T3
- **Blocks:** T5

**Size:** M (~2 часа)

**Commit:**
```
git add src/backend/AgileBoard/Adapters/WebApi/Controllers/TaskItemController.cs src/backend/AgileBoard.Tests/Integration/TaskItemControllerTests.cs
git commit -m "T4: Add TaskItemController with CRUD and Move endpoints"
```

---

### T5: Frontend — типы, API, useTaskItems hook

**Files:**
- `src/frontend/src/tasks/types/taskItem.ts` — TypeScript интерфейсы
- `src/frontend/src/tasks/api/taskItemsApi.ts` — API функции
- `src/frontend/src/tasks/hooks/useTaskItems.ts` — хук с оптимистичными обновлениями

**Do:**
- Создать типы: `TaskItem`, `CreateTaskItemDto`, `UpdateTaskItemDto`, `MoveTaskItemDto`, `ColumnType`
- Создать API функции: `getTaskItems(sprintId)`, `createTaskItem(sprintId, dto)`, `updateTaskItem(sprintId, taskId, dto)`, `deleteTaskItem(sprintId, taskId)`, `moveTaskItem(sprintId, dto)`
- Создать хук `useTaskItems(sprintId)`:
  - Загружает задачи при смене sprintId
  - `create` — оптимистично добавляет задачу в state, делает POST
  - `update` — оптимистично обновляет в state, делает PUT
  - `remove` — оптимистично удаляет из state, делает DELETE
  - `move` — оптимистично переупорядочивает state, делает PUT /move
  - При ошибке API — откатывает state к предыдущему состоянию

**Acceptance Criteria:**

**AC-5.1:** `useTaskItems` загружает задачи при изменении sprintId
**AC-5.2:** `create` добавляет задачу в локальный state до ответа сервера
**AC-5.3:** `move` переупорядочивает задачи в state до ответа сервера
**AC-5.4:** При ошибке сервера state откатывается

**Test Cases:**

*Фронтенд тесты out of scope. Ручная проверка:*

**Verify command (manual):**
```
npm --prefix src/frontend run dev
Открыть http://localhost:3000
Проверить через DevTools Network: запросы к /api/sprints/{id}/tasks уходят при выборе спринта
```

**Dependencies:**
- **Blocked by:** T4
- **Blocks:** T6

**Size:** M (~2 часа)

**Commit:**
```
git add src/frontend/src/tasks/
git commit -m "T5: Add TaskItem types, API and useTaskItems hook"
```

---

### T6: Frontend — TaskCard, TaskForm, ConfirmDialog компоненты

**Files:**
- `src/frontend/src/tasks/components/TaskCard.tsx` — карточка с кнопками ✏️/🗑️
- `src/frontend/src/tasks/components/TaskForm.tsx` — форма создания/редактирования
- `src/frontend/src/tasks/components/ConfirmDialog.tsx` — диалог подтверждения

**Do:**
- Создать `TaskCard` — отображает название, описание (если есть), кнопки ✏️ и 🗑️ в правом верхнем углу. Атрибут `draggable`
- Создать `TaskForm` — поля Name (обязательно), Description (опционально). Кнопки «Создать»/«Обновить» и «Отмена»
- Создать `ConfirmDialog` — модальное окно с сообщением, кнопки «Да»/«Нет» (или «Удалить»/«Отмена»)

**Acceptance Criteria:**

**AC-6.1:** `TaskCard` отображает name, description (если есть), кнопки ✏️ и 🗑️
**AC-6.2:** `TaskCard` имеет `draggable` атрибут
**AC-6.3:** `TaskForm` валидирует наличие Name
**AC-6.4:** `TaskForm` вызывает `onSubmit` с данными формы
**AC-6.5:** `ConfirmDialog` показывает сообщение и ждёт выбора пользователя

**Test Cases:**

*Фронтенд тесты out of scope. Ручная проверка:*

**Verify command (manual):**
```
npm --prefix src/frontend run dev
1. Проверить рендер TaskCard: название, описание, кнопки
2. Проверить TaskForm: валидация пустого Name
3. Проверить ConfirmDialog: кнопки «Удалить»/«Отмена»
```

**Dependencies:**
- **Blocked by:** T5
- **Blocks:** T7

**Size:** M (~2 часа)

**Commit:**
```
git add src/frontend/src/tasks/components/
git commit -m "T6: Add TaskCard, TaskForm and ConfirmDialog components"
```

---

### T7: Интеграция — модификация доски и drag-and-drop

**Files:**
- `src/frontend/src/sprints/components/SprintColumns.tsx` — модификация
- `src/frontend/src/sprints/components/SprintColumn.tsx` — модификация
- `src/frontend/src/sprints/pages/SprintBoardPage.tsx` — модификация
- `src/frontend/src/sprints/sprint-board.css` — стили карточек и DnD

**Do:**
- **SprintBoardPage:** интегрировать `useTaskItems(sprintId)`, добавить стейт модалок: `createModal(columnType)`, `editModal(task)`, `confirmDelete(task)`. Передать колбэки в SprintColumns
- **SprintColumns:** принимать `tasks`, `callbacks`. Распределять задачи по колонкам. Рендерить кнопку "+" в заголовке каждой колонки и счётчик задач
- **SprintColumn:** рендерить TaskCard вместо заглушки «Пусто». Обработчики `onDragOver`/`onDrop`. Подсветка drop-зоны
- **Drag-and-drop логика:**
  - `onDragStart(e, taskId)` — сохраняет taskId в dataTransfer
  - `onDragOver(e)` — preventDefault + подсветка
  - `onDrop(e, columnType, targetPosition)` — вызывает `moveTask(taskId, columnType, targetPosition)`
- **CSS:** стили для `.task-card`, `.task-card-actions`, `.column-counter`, `.drag-over`, `.drop-indicator`
- **Счётчик:** бейдж с числом задач рядом с заголовком колонки

**Acceptance Criteria:**

**AC-7.1:** Кнопка "+" в заголовке колонки открывает модалку создания с предустановленным ColumnType
**AC-7.2:** Созданная задача появляется сверху колонки
**AC-7.3:** Счётчик показывает количество задач в каждой колонке
**AC-7.4:** Задачу можно перетащить в другую колонку
**AC-7.5:** Задачу можно перетащить в другую позицию внутри колонки
**AC-7.6:** При перетаскивании есть визуальная обратная связь (подсветка)
**AC-7.7:** ✏️ открывает модалку редактирования, 🗑️ — диалог подтверждения
**AC-7.8:** После подтверждения удаления задача исчезает
**AC-7.9:** Переключение спринта загружает задачи нового спринта

**Test Cases:**

*Фронтенд тесты out of scope. Ручная E2E проверка:*

**Verify command (manual):**
```
npm --prefix src/frontend run dev
dotnet run --project src/backend/AgileBoard

1. Выбрать спринт → колонки видны
2. Нажать "+" в колонке «Новые» → модалка создания
3. Ввести название, сохранить → карточка в колонке, счётчик "1"
4. Создать ещё 2 задачи → счётчик "3", порядок верный
5. Перетащить задачу в «В процессе» → счётчики обновились
6. Перетащить задачу внутри колонки → порядок изменился
7. Нажать ✏️ → модалка редактирования → изменить → сохранить
8. Нажать 🗑️ → диалог подтверждения → «Да» → задача удалена, счётчик обновился
9. Переключить спринт → только задачи нового спринта
```

**Docker-верификация (при приёмке PR):**
```
docker-compose up --build -d
docker-compose ps                    # 3 сервиса "Up"
docker-compose exec db psql -U postgres -d agileboard -c "\dt" | grep TaskItems
curl -s http://localhost:5000/api/sprints | python3 -c "import sys,json; d=json.load(sys.stdin); assert isinstance(d,list)"
```

**Dependencies:**
- **Blocked by:** T6
- **Blocks:** —

**Size:** M (~3 часа)

**Commit:**
```
git add src/frontend/src/sprints/ src/frontend/src/sprints/sprint-board.css
git commit -m "T7: Integrate TaskItems with board, add drag-and-drop"
```

---
