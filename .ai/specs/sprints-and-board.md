# Спринты и доска

**Статус:** ✅ Реализовано

## Why
Пользователю необходимо управление спринтами и визуализация задач через доску. Это базовая функция Agile/Scrum, которая позволяет планировать работу и отслеживать прогресс в рамках итераций.

## What
Веб-интерфейс с панелью управления спринтами: создание, редактирование, удаление, выбор через ComboBox. Доска — это визуальное представление спринта с тремя колонками-заглушками (Новые, В процессе, Сделаны). Задачи будут добавлены в будущем.

## Context

**Статус реализации:** Все задачи T1-T7 выполнены.

**Relevant files:**
- `CLAUDE.md` — общая архитектура и паттерны проекта
- `.ai/specs/infrastructure.md` — базовая структура и паттерны (CQRS, Repository, DDD Lite)
- `src/backend/AgileBoard/Domain/Sprint.cs` — Aggregate Root спринта
- `src/backend/AgileBoard/Domain/SprintId.cs` — Value Object для ID спринта
- `src/backend/AgileBoard/Adapters/Persistence/AppDbContext.cs` — DbContext с DbSet<Sprint>
- `src/backend/AgileBoard/Adapters/Persistence/Repositories/SprintRepository.cs` — репозиторий для спринтов
- `src/backend/AgileBoard/Adapters/Persistence/Configurations/SprintConfiguration.cs` — конфигурация EF Core
- `src/backend/AgileBoard/Adapters/Persistence/Migrations/` — миграция CreateSprintsTable
- `src/backend/AgileBoard/Application/UseCases/Sprints/` — CQRS команды и запросы
- `src/backend/AgileBoard/Adapters/WebApi/Controllers/SprintController.cs` — CRUD endpoints
- `src/backend/AgileBoard/Adapters/WebApi/Filters/ExceptionHandlingMiddleware.cs` — обработка SprintOverlapException
- `src/frontend/src/sprints/` — React компоненты (pages, components, hooks, types, api)

**Patterns to follow:**
- **CQRS:** IRequest/IRequest<TResult> для команд и запросов (infrastructure.md, строки 140-143)
- **Repository:** IReadRepository (expressions), IWriteRepository (aggregates) (infrastructure.md, строки 145-148)
- **DDD Lite:** Aggregate Roots с инкапсулированной логикой (infrastructure.md, строки 150-153)
- **TDD:** Red → Green → Refactor для backend (infrastructure.md, строки 155-159)
- **Структура папок frontend:** `src/frontend/<feature>/components/, hooks/, types/` (CLAUDE.md)

**Реализованные паттерны:**
- **MediatR:** Используется для CQRS (IMediator в контроллерах)
- **TDD:** Все backend тесты написаны и проходят (Unit + Integration)

**Key decisions already made:**
- Backend: ASP.NET Core 8 Controllers + EF Core + PostgreSQL
- Frontend: React + TypeScript (Vite)
- Тесты: NUnit (backend), без тестов для frontend
- Порты: Backend 5000, Frontend 3000, PostgreSQL 5432

## Требования

### Функциональные

**FR-1: Сущность Спринт**
- Спринт имеет: Id, Название, Дата начала, Дата окончания, Описание
- Доска является частью спринта (не существует отдельно)

**FR-2: Создание спринта**
- Форма с полями: Название (обязательно), Диапазон дат (обязательно), Описание (опционально)
- Валидация: диапазон дат не должен пересекаться с существующими спринтами

**FR-3: Редактирование спринта**
- Форма редактирования с теми же полями
- Валидация пересечения диапазонов (исключая текущий спринт)
- Сохранение изменений

**FR-4: Удаление спринта**
- Удаление спринта
- Подтверждение перед удалением

**FR-5: Выбор активного спринта**
- ComboBox/Select для переключения между спринтами
- Выбранный спринт становится активным
- Доска отображается для выбранного спринта

**FR-6: Доска спринта**
- Три колонки: Новые, В процессе, Сделаны
- Колонки заполняют доступное пространство по вертикали (без скролла страницы)
- Колонки пустые (задачи будут добавлены в будущем)

### Нефункциональные

**NFR-1: Валидация пересечения дат**
- **Metric:** Серверная валидация выполняется < 100ms
- **Metric:** Сообщение об ошибке понятно пользователю

**NFR-2: UI отзывчивость**
- **Metric:** Переключение спринта < 500ms

**NFR-3: Адаптивность доски**
- **Metric:** Колонки занимают 100% доступной высоты без скролла страницы
- **Metric:** Внутренний скролл только внутри колонок при необходимости


## Ограничения

**Must:**
- Использовать CQRS паттерн для всех операций
- Использовать Repository pattern для доступа к данным
- Aggregate Root для Sprint
- Валидация пересечения дат на уровне домена/приложения
- TDD для backend кода

**Must not:**
- Не создавать отдельную сущность "Доска" — доска это проекция спринта
- Не добавлять новые внешние библиотеки без необходимости
- Не трогать существующие настройки Docker/БД

**Out of scope:**
- Задачи спринта (будут добавлены в будущем)
- Перетаскивание задач (drag-and-drop) между колонками
- Оценка задач (story points)
- Фильтрация и поиск задач
- История изменений спринта
- Шаблоны спринтов
- Экспорт/импорт данных
- E2E тесты
- **Frontend тесты** — React компоненты не покрываются автотестами, только ручная проверка через `npm run dev`

## Критерии приёмки

**AC-1:** Создание спринта с валидными данными → спринт появляется в БД и в UI
**AC-2:** Создание спринта с пересекающимся диапазоном → ошибка валидации
**AC-3:** Редактирование спринта → изменения сохраняются
**AC-4:** Удаление спринта → спринт удалён из БД
**AC-5:** Выбор спринта в ComboBox → доска отображается для выбранного спринта
**AC-6:** Доска имеет 3 колонки с корректными заголовками
**AC-7:** Колонки занимают доступное пространство без скролла страницы


## Проектирование

### Стек

**Выбранные технологии:**
- **Backend:** ASP.NET Core 8 Controllers — соответствует архитектуре проекта, явные контроллеры для CRUD операций
- **Frontend:** React 19 + TypeScript + Vite — компонентный подход с типизацией, быстрый dev-сервер
- **Database:** PostgreSQL 15+ — реляционная БД для хранения спринтов
- **ORM:** EF Core 8 — Code First подход с миграциями
- **UI Components:** Нативные HTML элементы + CSS — минимальные зависимости на старте

**Альтернативы (отклонены):**
- **Minimal API вместо Controllers** — отклонено: Controllers более явные для CRUD операций
- **MediatR для CQRS** — отклонено: добавим при необходимости, сейчас достаточно простых интерфейсов
- **Готовые UI библиотеки (MUI, AntD)** — отклонено: избыточно для 3 колонок и ComboBox

### Архитектура

#### Компоненты backend

| Компонент | Ответственность | Интерфейсы |
|-----------|-----------------|------------|
| SprintController | HTTP endpoints для CRUD спринтов | GET/POST/PUT/DELETE /api/sprints |
| CreateSprintCommand | Команда создания спринта | IRequest<Guid> |
| UpdateSprintCommand | Команда обновления спринта | IRequest |
| DeleteSprintCommand | Команда удаления спринта | IRequest |
| GetSprintsQuery | Запрос списка спринтов | IRequest<IEnumerable<SprintDto>> |
| SprintRepository | Доступ к спринтам в БД | GetById, GetAll, Add, Update, Delete |
| SprintAggregate | Доменная модель спринта | OverlapsWith() |

#### Компоненты frontend

| Компонент | Ответственность | Интерфейсы |
|-----------|-----------------|------------|
| SprintBoard | Страница доски с управлением спринтами | Маршрут /sprints |
| SprintSelect | ComboBox выбора спринта | onChange(sprintId) |
| SprintForm | Форма создания/редактирования | onSubmit(sprintData) |
| SprintColumns | Три колонки доски | Отображение колонок |

#### Поток данных

```
[Frontend:3000] → [SprintController:5000] → [Command/Query Handler] → [SprintRepository] → [PostgreSQL:5432]
                                              ↓
                                    [OverlapsWith Validation]
```

### Паттерны

**Pattern 1: CQRS для спринтов**
- **Problem:** Разделение операций чтения (список спринтов) и записи (создание/редактирование/удаление)
- **Solution:** IRequest<TResult> для запросов, IRequest для команд
- **Location:** `src/backend/AgileBoard/Application/UseCases/Sprints/`

**Pattern 2: Repository для доступа к данным**
- **Problem:** Абстракция доступа к данным спринтов
- **Solution:** ISprintRepository с методами для работы с Aggregate Root
- **Location:** `src/backend/AgileBoard/Adapters/Persistence/Repositories/`

**Pattern 3: Domain Validation**
- **Problem:** Предотвращение создания невалидных спринтов
- **Solution:** Методы валидации внутри Sprint Aggregate (проверка пересечения дат)
- **Location:** `src/backend/AgileBoard/Domain/SprintAggregate.cs`

**Pattern 4: TDD для backend**
- **Problem:** Гарантия работоспособности кода
- **Solution:** Red → Green → Refactor цикл при реализации
- **Location:** `src/backend/AgileBoard.Tests/Unit/UseCases/Sprints/`

### API-контракты

**Реализовано:**

```
GET /api/sprints
Response: [{ "id": "uuid", "name": "string", "startDate": "date", "endDate": "date", "description": "string" }]
Status: 200 OK

GET /api/sprints/{id}
Response: { "id": "uuid", "name": "string", "startDate": "date", "endDate": "date", "description": "string" }
Status: 200 OK
Примечание: колонки возвращаются на frontend (статические данные)

POST /api/sprints
Request: { "name": "string", "startDate": "date", "endDate": "date", "description": "string?" }
Response: { "id": "uuid", "name": "string", "startDate": "date", "endDate": "date", "description": "string" }
Status: 201 Created

PUT /api/sprints/{id}
Request: { "name": "string", "startDate": "date", "endDate": "date", "description": "string?" }
Response: 204 No Content

DELETE /api/sprints/{id}
Response: 204 No Content
```

**Обработка ошибок:**
- `SprintOverlapException` → 400 Bad Request (через ExceptionHandlingMiddleware)
- `NotFoundException` → 404 Not Found

### Миграция (если требуется)

```sql
-- Таблица спринтов
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Sprints" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "Description" character varying(1000) NULL,
    CONSTRAINT "PK_Sprints" PRIMARY KEY ("Id")
);
```

### Риски

| Риск | Вероятность | Влияние | Митигация |
|------|-------------|---------|-----------|
| Пересечение дат спринтов | Средняя | Высокое | Валидация на уровне домена (OverlapsWith) |
| CORS между frontend/backend | Низкая | Среднее | Настроить CORS в Program.cs для localhost:3000 |

### Этапы реализации

**Stage 1:** Domain слой — Sprint Aggregate Root с методом OverlapsWith()
**Stage 2:** Persistence слой — SprintRepository, миграция БД
**Stage 3:** Application слой — CQRS команды и запросы для спринтов
**Stage 4:** WebApi слой — SprintController с endpoints (GET/{id} возвращает board)
**Stage 5:** Frontend — SprintBoard страница с ComboBox и формой
**Stage 6:** Frontend — Три колонки доски
**Stage 7:** Интеграция — подключение frontend к backend API

## Задачи

**Статус:** Все задачи (T1-T7) выполнены ✅

### T1: Доменная модель Sprint Aggregate ✅

**Status:** Реализовано

**Files:** 
- `src/backend/AgileBoard/Domain/Sprint.cs` — агрегат с методами Create, Update, OverlapsWith
- `src/backend/AgileBoard/Domain/SprintId.cs` — value object wrapper над Guid
- `src/backend/AgileBoard.Tests/Unit/Domain/SprintTests.cs` — 4 теста (AC-1.1 — AC-1.4)

**Files:** 
- `src/backend/AgileBoard/Domain/Sprint.cs`
- `src/backend/AgileBoard/Domain/SprintId.cs`
- `src/backend/AgileBoard.Tests/Unit/Domain/SprintTests.cs`

**Do:**
- Создать value object `SprintId` (wrapper над Guid)
- Создать aggregate root `Sprint` с полями: Id, Name, StartDate, EndDate, Description
- Добавить метод валидации `OverlapsWith(DateTime start, DateTime end)` для проверки пересечения дат
- Добавить factory method `Sprint.Create(name, start, end, description)`
- Добавить метод `Update(name, start, end, description)`

**Acceptance Criteria:**

**AC-1.1:** Sprint.Create с валидными данными → создаёт агрегат с присвоенным Id

**AC-1.2:** Sprint.OverlapsWith с непересекающимся диапазоном → возвращает false

**AC-1.3:** Sprint.OverlapsWith с пересекающимся диапазоном → возвращает true

**AC-1.4:** Sprint.Update изменяет свойства агрегата

**Test Cases:**

#### Test-1: Sprint_Create_WithValidData_AssignsId
**Type:** Unit
**Links:** AC-1.1

**Preconditions:**
- None

**Action:**
```csharp
var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Description");
```

**Expected:**
```
sprint.Id != Guid.Empty
sprint.Name == "Sprint 1"
```

**Verify command:**
```
dotnet test --filter "Sprint_Create_WithValidData_AssignsId"
```

#### Test-2: Sprint_OverlapsWith_NonOverlappingRange_ReturnsFalse
**Type:** Unit
**Links:** AC-1.2

**Preconditions:**
- Спринт с датами [2026-01-01, 2026-01-14]

**Action:**
```csharp
var overlaps = sprint.OverlapsWith(DateTime.Parse("2026-01-15"), DateTime.Parse("2026-01-28"));
```

**Expected:**
```
overlaps == false
```

**Verify command:**
```
dotnet test --filter "Sprint_OverlapsWith_NonOverlappingRange_ReturnsFalse"
```

#### Test-3: Sprint_OverlapsWith_OverlappingRange_ReturnsTrue
**Type:** Unit
**Links:** AC-1.3

**Preconditions:**
- Спринт с датами [2026-01-01, 2026-01-14]

**Action:**
```csharp
var overlaps = sprint.OverlapsWith(DateTime.Parse("2026-01-10"), DateTime.Parse("2026-01-20"));
```

**Expected:**
```
overlaps == true
```

**Verify command:**
```
dotnet test --filter "Sprint_OverlapsWith_OverlappingRange_ReturnsTrue"
```

#### Test-4: Sprint_Update_ChangesProperties
**Type:** Unit
**Links:** AC-1.4

**Preconditions:**
- Создан спринт

**Action:**
```csharp
sprint.Update("New Name", newStart, newEnd, "New Description");
```

**Expected:**
```
sprint.Name == "New Name"
sprint.Description == "New Description"
```

**Verify command:**
```
dotnet test --filter "Sprint_Update_ChangesProperties"
```

**Dependencies:**
- **Blocks:** T2, T3

**Size:** S (~1-2 часа)

**Commit:**
```
git add src/backend/AgileBoard/Domain/Sprint.cs src/backend/AgileBoard/Domain/SprintId.cs src/backend/AgileBoard.Tests/Unit/Domain/SprintTests.cs
git commit -m "Add Sprint aggregate root with OverlapsWith validation"
```

---

### T2: Репозиторий и миграция БД для спринтов ✅

**Status:** Реализовано

**Files:**
- `src/backend/AgileBoard/Adapters/Persistence/Configurations/SprintConfiguration.cs` — конфигурация EF Core
- `src/backend/AgileBoard/Adapters/Persistence/Repositories/SprintRepository.cs` — реализация ISprintRepository
- `src/backend/AgileBoard/Adapters/Persistence/Repositories/ISprintRepository.cs` — интерфейс репозитория
- `src/backend/AgileBoard/Adapters/Persistence/AppDbContext.cs` — DbSet<Sprint>
- `src/backend/AgileBoard/Adapters/Persistence/Migrations/20260507091206_CreateSprintsTable.cs` — миграция
- `src/backend/AgileBoard.Tests/Integration/Persistence/SprintRepositoryTests.cs` — 4 интеграционных теста

**Do:**
- Добавить `DbSet<Sprint>` в `AppDbContext`
- Создать конфигурацию `SprintConfiguration` для маппинга полей
- Создать `SprintRepository` с методами: GetById, GetAll, Add, Update, Delete
- Создать и применить миграцию для таблицы Sprints

**Acceptance Criteria:**

**AC-2.1:** SprintRepository.GetAll возвращает все спринты из БД

**AC-2.2:** SprintRepository.GetById возвращает спринт по Id

**AC-2.3:** SprintRepository.Add сохраняет спринт в БД

**AC-2.4:** SprintRepository.Update обновляет спринт в БД

**AC-2.5:** SprintRepository.Delete удаляет спринт из БД

**AC-2.6:** Миграция создаёт таблицу Sprints с правильной схемой

**Test Cases:**

#### Test-5: SprintRepository_Add_ThenGetById_ReturnsSprint
**Type:** Integration
**Links:** AC-2.2, AC-2.3

**Preconditions:**
- Тестовая БД инициализирована
- Создан спринт через repository.Add()

**Action:**
```csharp
var found = await repository.GetByIdAsync(sprint.Id);
```

**Expected:**
```
found != null
found.Id == sprint.Id
found.Name == sprint.Name
```

**Verify command:**
```
dotnet test --filter "SprintRepository_Add_ThenGetById_ReturnsSprint"
```

#### Test-6: SprintRepository_GetAll_ReturnsAllSprints
**Type:** Integration
**Links:** AC-2.1

**Preconditions:**
- В БД добавлено 3 спринта

**Action:**
```csharp
var all = await repository.GetAllAsync();
```

**Expected:**
```
all.Count() == 3
```

**Verify command:**
```
dotnet test --filter "SprintRepository_GetAll_ReturnsAllSprints"
```

#### Test-7: SprintRepository_Update_ThenGetById_ReturnsUpdated
**Type:** Integration
**Links:** AC-2.4

**Preconditions:**
- Спринт существует в БД

**Action:**
```csharp
sprint.Update("Updated", ...);
await repository.UpdateAsync(sprint);
var found = await repository.GetByIdAsync(sprint.Id);
```

**Expected:**
```
found.Name == "Updated"
```

**Verify command:**
```
dotnet test --filter "SprintRepository_Update_ThenGetById_ReturnsUpdated"
```

#### Test-8: SprintRepository_Delete_ThenGetById_ReturnsNull
**Type:** Integration
**Links:** AC-2.5

**Preconditions:**
- Спринт существует в БД

**Action:**
```csharp
await repository.DeleteAsync(sprint.Id);
var found = await repository.GetByIdAsync(sprint.Id);
```

**Expected:**
```
found == null
```

**Verify command:**
```
dotnet test --filter "SprintRepository_Delete_ThenGetById_ReturnsNull"
```

**Dependencies:**
- **Blocked by:** T1
- **Blocks:** T3

**Size:** S (~1-2 часа)

**Commit:**
```
git add src/backend/AgileBoard/Adapters/Persistence/ src/backend/AgileBoard/Migrations/
git commit -m "Add SprintRepository and database migration"
```

---

### T3: CQRS команды и запросы для спринтов ✅

**Status:** Реализовано

**Files:**
- `src/backend/AgileBoard/Application/UseCases/Sprints/CreateSprintCommand.cs` — команда + обработчик
- `src/backend/AgileBoard/Application/UseCases/Sprints/UpdateSprintCommand.cs` — команда + обработчик
- `src/backend/AgileBoard/Application/UseCases/Sprints/DeleteSprintCommand.cs` — команда + обработчик
- `src/backend/AgileBoard/Application/UseCases/Sprints/GetSprintsQuery.cs` — запрос + обработчик
- `src/backend/AgileBoard/Application/UseCases/Sprints/GetSprintByIdQuery.cs` — запрос + обработчик
- `src/backend/AgileBoard/Application/UseCases/Sprints/SprintDto.cs` — DTO записи
- `src/backend/AgileBoard/Application/UseCases/Sprints/CreateSprintDto.cs` — DTO создания
- `src/backend/AgileBoard/Application/UseCases/Sprints/UpdateSprintDto.cs` — DTO обновления
- `src/backend/AgileBoard/Application/UseCases/Sprints/SprintOverlapException.cs` — исключение пересечения
- `src/backend/AgileBoard/Application/UseCases/Sprints/NotFoundException.cs` — исключение не найдено
- `src/backend/AgileBoard.Tests/Unit/UseCases/Sprints/*.cs` — 8 unit тестов

**Do:**
- Создать DTO `SprintDto` и `CreateSprintDto`, `UpdateSprintDto`
- Создать команду `CreateSprintCommand : IRequest<Guid>` и обработчик
- Создать команду `UpdateSprintCommand : IRequest` и обработчик
- Создать команду `DeleteSprintCommand : IRequest` и обработчик
- Создать запрос `GetSprintsQuery : IRequest<IEnumerable<SprintDto>>` и обработчик
- Создать запрос `GetSprintByIdQuery : IRequest<SprintDto>` и обработчик
- Добавить валидацию пересечения дат в `CreateSprintCommandHandler` и `UpdateSprintCommandHandler`

**Acceptance Criteria:**

**AC-3.1:** CreateSprintCommand с валидными данными → возвращает Id нового спринта

**AC-3.2:** CreateSprintCommand с пересекающимися датами → выбрасывает SprintOverlapException

**AC-3.3:** UpdateSprintCommand с валидными данными → обновляет спринт

**AC-3.4:** UpdateSprintCommand с пересекающимися датами → выбрасывает SprintOverlapException

**AC-3.5:** DeleteSprintCommand → удаляет спринт по Id

**AC-3.6:** GetSprintsQuery → возвращает список всех спринтов

**AC-3.7:** GetSprintByIdQuery с существующим Id → возвращает SprintDto

**AC-3.8:** GetSprintByIdQuery с несуществующим Id → выбрасывает NotFoundException

**Test Cases:**

#### Test-9: CreateSprintCommand_ValidData_ReturnsId
**Type:** Unit
**Links:** AC-3.1

**Preconditions:**
- Mock репозитория настроен
- Спринт не пересекается с существующими

**Action:**
```csharp
var result = await handler.Handle(new CreateSprintCommand(dto), CancellationToken.None);
```

**Expected:**
```
result != Guid.Empty
```

**Verify command:**
```
dotnet test --filter "CreateSprintCommand_ValidData_ReturnsId"
```

#### Test-10: CreateSprintCommand_OverlappingDates_ThrowsSprintOverlapException
**Type:** Unit
**Links:** AC-3.2

**Preconditions:**
- Mock репозитория возвращает пересекающийся спринт

**Action:**
```csharp
Func<Task> act = async () => await handler.Handle(validCommand, CancellationToken.None);
```

**Expected:**
```
act.Throws<SprintOverlapException>()
```

**Verify command:**
```
dotnet test --filter "CreateSprintCommand_OverlappingDates_ThrowsSprintOverlapException"
```

#### Test-11: UpdateSprintCommand_ValidData_UpdatesSprint
**Type:** Unit
**Links:** AC-3.3

**Preconditions:**
- Mock репозитория возвращает существующий спринт
- Новые даты не пересекаются

**Action:**
```csharp
await handler.Handle(new UpdateSprintCommand(id, dto), CancellationToken.None);
```

**Expected:**
```
repository.UpdateAsync вызван 1 раз
```

**Verify command:**
```
dotnet test --filter "UpdateSprintCommand_ValidData_UpdatesSprint"
```

#### Test-12: UpdateSprintCommand_OverlappingDates_ThrowsSprintOverlapException
**Type:** Unit
**Links:** AC-3.4

**Preconditions:**
- Mock репозитория возвращает пересекающийся спринт

**Action:**
```csharp
Func<Task> act = async () => await handler.Handle(updateCommand, CancellationToken.None);
```

**Expected:**
```
act.Throws<SprintOverlapException>()
```

**Verify command:**
```
dotnet test --filter "UpdateSprintCommand_OverlappingDates_ThrowsSprintOverlapException"
```

#### Test-13: DeleteSprintCommand_ExistingId_DeletesSprint
**Type:** Unit
**Links:** AC-3.5

**Preconditions:**
- Mock репозитория возвращает существующий спринт

**Action:**
```csharp
await handler.Handle(new DeleteSprintCommand(id), CancellationToken.None);
```

**Expected:**
```
repository.DeleteAsync вызван 1 раз
```

**Verify command:**
```
dotnet test --filter "DeleteSprintCommand_ExistingId_DeletesSprint"
```

#### Test-14: GetSprintsQuery_ReturnsAllSprints
**Type:** Unit
**Links:** AC-3.6

**Preconditions:**
- Mock репозитория возвращает 3 спринта

**Action:**
```csharp
var result = await handler.Handle(new GetSprintsQuery(), CancellationToken.None);
```

**Expected:**
```
result.Count() == 3
```

**Verify command:**
```
dotnet test --filter "GetSprintsQuery_ReturnsAllSprints"
```

#### Test-15: GetSprintByIdQuery_ExistingId_ReturnsSprintDto
**Type:** Unit
**Links:** AC-3.7

**Preconditions:**
- Mock репозитория возвращает существующий спринт

**Action:**
```csharp
var result = await handler.Handle(new GetSprintByIdQuery(id), CancellationToken.None);
```

**Expected:**
```
result.Id == id
result.Name == sprint.Name
```

**Verify command:**
```
dotnet test --filter "GetSprintByIdQuery_ExistingId_ReturnsSprintDto"
```

#### Test-16: GetSprintByIdQuery_NonExistingId_ThrowsNotFoundException
**Type:** Unit
**Links:** AC-3.8

**Preconditions:**
- Mock репозитория возвращает null

**Action:**
```csharp
Func<Task> act = async () => await handler.Handle(new GetSprintByIdQuery(Guid.NewGuid()), CancellationToken.None);
```

**Expected:**
```
act.Throws<NotFoundException>()
```

**Verify command:**
```
dotnet test --filter "GetSprintByIdQuery_NonExistingId_ThrowsNotFoundException"
```

**Dependencies:**
- **Blocked by:** T2
- **Blocks:** T4

**Size:** M (~2-3 часа)

**Commit:**
```
git add src/backend/AgileBoard/Application/UseCases/Sprints/ src/backend/AgileBoard.Tests/Unit/UseCases/Sprints/
git commit -m "Add CQRS commands and queries for Sprints"
```

---

### T4: SprintController с CRUD endpoints ✅

**Status:** Реализовано

**Files:**
- `src/backend/AgileBoard/Adapters/WebApi/Controllers/SprintController.cs` — контроллер с 5 endpoints
- `src/backend/AgileBoard/Adapters/WebApi/Filters/SprintOverlapException.cs` — определение исключения
- `src/backend/AgileBoard/Adapters/WebApi/Filters/ExceptionHandlingMiddleware.cs` — глобальная обработка ошибок
- `src/backend/AgileBoard.Tests/Integration/SprintControllerTests.cs` — 8 интеграционных тестов (AC-4.1 — AC-4.8)

**Do:**
- Создать `SprintController : ControllerBase` с маршрутом `/api/sprints`
- Реализовать endpoint `GET /` → возвращает все спринты
- Реализовать endpoint `GET /{id}` → возвращает спринт с колонками доски
- Реализовать endpoint `POST /` → создаёт спринт, возвращает 201 Created
- Реализовать endpoint `PUT /{id}` → обновляет спринт, возвращает 204 No Content
- Реализовать endpoint `DELETE /{id}` → удаляет спринт, возвращает 204 No Content
- Обработать SprintOverlapException → 400 Bad Request

**Acceptance Criteria:**

**AC-4.1:** GET /api/sprints → 200 OK, массив спринтов

**AC-4.2:** GET /api/sprints/{id} → 200 OK, спринт с колонками

**AC-4.3:** POST /api/sprints с валидными данными → 201 Created, тело спринта

**AC-4.4:** POST /api/sprints с пересекающимися датами → 400 Bad Request

**AC-4.5:** PUT /api/sprints/{id} с валидными данными → 204 No Content

**AC-4.6:** PUT /api/sprints/{id} с пересекающимися датами → 400 Bad Request

**AC-4.7:** DELETE /api/sprints/{id} → 204 No Content

**AC-4.8:** GET /api/sprints/{nonExistingId} → 404 Not Found

**Test Cases:**

#### Test-17: GET/api/sprints_EmptyDatabase_ReturnsEmptyArray
**Type:** Integration
**Links:** AC-4.1

**Preconditions:**
- Backend запущен
- БД пуста

**Action:**
```
GET /api/sprints
```

**Expected:**
- Status: 200 OK
- Body: []

**Verify command:**
```
dotnet test --filter "GET_api_sprints_EmptyDatabase_ReturnsEmptyArray"
```

#### Test-18: GET/api/sprints_WithSprints_ReturnsArray
**Type:** Integration
**Links:** AC-4.1

**Preconditions:**
- Backend запущен
- В БД 2 спринта

**Action:**
```
GET /api/sprints
```

**Expected:**
- Status: 200 OK
- Body: массив из 2 элементов

**Verify command:**
```
dotnet test --filter "GET_api_sprints_WithSprints_ReturnsArray"
```

#### Test-19: GET/api/sprints/{id}_ExistingId_ReturnsSprintWithColumns
**Type:** Integration
**Links:** AC-4.2

**Preconditions:**
- Backend запущен
- Спринт существует в БД

**Action:**
```
GET /api/sprints/{id}
```

**Expected:**
- Status: 200 OK
- Body содержит: id, name, startDate, endDate, columns (3 элемента)

**Verify command:**
```
dotnet test --filter "GET_api_sprints_id_ExistingId_ReturnsSprintWithColumns"
```

#### Test-20: POST/api/sprints_ValidData_Returns201Created
**Type:** Integration
**Links:** AC-4.3

**Preconditions:**
- Backend запущен

**Action:**
```
POST /api/sprints
Body: { "name": "Sprint 1", "startDate": "2026-01-01", "endDate": "2026-01-14", "description": "Test" }
```

**Expected:**
- Status: 201 Created
- Body содержит: id, name, startDate, endDate

**Verify command:**
```
dotnet test --filter "POST_api_sprints_ValidData_Returns201Created"
```

#### Test-21: POST/api/sprints_OverlappingDates_Returns400BadRequest
**Type:** Integration
**Links:** AC-4.4

**Preconditions:**
- Backend запущен
- В БД существует спринт [2026-01-01, 2026-01-14]

**Action:**
```
POST /api/sprints
Body: { "name": "Overlap Sprint", "startDate": "2026-01-10", "endDate": "2026-01-20" }
```

**Expected:**
- Status: 400 Bad Request

**Verify command:**
```
dotnet test --filter "POST_api_sprints_OverlappingDates_Returns400BadRequest"
```

#### Test-22: PUT/api/sprints/{id}_ValidData_Returns204NoContent
**Type:** Integration
**Links:** AC-4.5

**Preconditions:**
- Backend запущен
- Спринт существует в БД

**Action:**
```
PUT /api/sprints/{id}
Body: { "name": "Updated Name", "startDate": "2026-01-01", "endDate": "2026-01-14", "description": "Updated" }
```

**Expected:**
- Status: 204 No Content

**Verify command:**
```
dotnet test --filter "PUT_api_sprints_id_ValidData_Returns204NoContent"
```

#### Test-23: DELETE/api/sprints/{id}_ExistingId_Returns204NoContent
**Type:** Integration
**Links:** AC-4.7

**Preconditions:**
- Backend запущен
- Спринт существует в БД

**Action:**
```
DELETE /api/sprints/{id}
```

**Expected:**
- Status: 204 No Content

**Verify command:**
```
dotnet test --filter "DELETE_api_sprints_id_ExistingId_Returns204NoContent"
```

**Dependencies:**
- **Blocked by:** T3
- **Blocks:** T5, T6

**Size:** M (~2-3 часа)

**Commit:**
```
git add src/backend/AgileBoard/Adapters/WebApi/Controllers/SprintController.cs src/backend/AgileBoard.Tests/Integration/SprintControllerTests.cs
git commit -m "Add SprintController with CRUD endpoints"
```

---

### T5: Frontend — SprintBoard страница с ComboBox и формой ✅

**Status:** Реализовано

**Files:**
- `src/frontend/src/sprints/types/sprint.ts` — TypeScript интерфейсы (Sprint, CreateSprintDto, UpdateSprintDto)
- `src/frontend/src/sprints/hooks/useSprints.ts` — хук для загрузки и управления спринтами
- `src/frontend/src/sprints/hooks/useSprintForm.ts` — хук для управления формой
- `src/frontend/src/sprints/api/sprintsApi.ts` — API функции (getSprints, createSprint, updateSprint, deleteSprint)
- `src/frontend/src/sprints/components/SprintSelect.tsx` — ComboBox выбора спринта
- `src/frontend/src/sprints/components/SprintForm.tsx` — форма создания/редактирования
- `src/frontend/src/sprints/components/SprintBoard.tsx` — контейнер доски
- `src/frontend/src/sprints/components/SprintColumns.tsx` — три колонки доски
- `src/frontend/src/sprints/components/SprintColumn.tsx` — отдельная колонка
- `src/frontend/src/sprints/pages/SprintBoardPage.tsx` — страница с маршрутом `/sprints`

**Do:**
- Создать тип `Sprint` с полями: id, name, startDate, endDate, description, columns?
- Создать хук `useSprints` для загрузки списка спринтов и выбора активного
- Создать хук `useSprintForm` для управления формой создания/редактирования
- Создать компонент `SprintSelect` — ComboBox для выбора спринта
- Создать компонент `SprintForm` — форма с полями: название, даты, описание
- Создать компонент `SprintBoard` — контейнер страницы
- Создать страницу `SprintBoardPage` с маршрутом `/sprints`

**Acceptance Criteria:**

**AC-5.1:** SprintSelect отображает список спринтов из API

**AC-5.2:** SprintSelect при выборе устанавливает активный спринт

**AC-5.3:** SprintForm отображает поля: название (обязательно), даты (обязательно), описание

**AC-5.4:** SprintForm при создании отправляет POST /api/sprints

**AC-5.5:** SprintForm при редактировании отправляет PUT /api/sprints/{id}

**AC-5.6:** SprintForm показывает ошибку валидации при пересечении дат

**AC-5.7:** SprintBoardPage содержит SprintSelect, SprintForm (для создания), SprintBoard

**Test Cases:**

*Примечание: frontend тесты не требуются по спецификации (Out of scope: E2E тесты, без тестов для frontend)*

**Verify command (manual):**
```
npm run dev
Открыть http://localhost:3000/sprints
Проверить: ComboBox показывает спринты, форма создаёт спринт
```

**Dependencies:**
- **Blocked by:** T4
- **Blocks:** T6

**Size:** M (~2-3 часа)

**Commit:**
```
git add src/frontend/src/sprints/
git commit -m "Add SprintBoard page with ComboBox and form"
```

---

### T6: Frontend — Три колонки доски ✅

**Status:** Реализовано

**Files:**
- `src/frontend/src/sprints/components/SprintColumns.tsx` — контейнер для 3 колонок
- `src/frontend/src/sprints/components/SprintColumn.tsx` — отдельная колонка с заголовком
- `src/frontend/src/sprints/sprints-board.css` — стили flexbox для вертикального заполнения

**Do:**
- Создать компонент `SprintColumns` — контейнер для 3 колонок
- Создать компонент `SprintColumn` — отдельная колонка с заголовком
- Добавить CSS для вертикального заполнения: flexbox, 100% высоты
- Добавить скролл внутри колонок при необходимости
- Заполнить колонки заглушками (пустые состояния)

**Acceptance Criteria:**

**AC-6.1:** SprintColumns рендерит 3 колонки: "Новые", "В процессе", "Сделаны"

**AC-6.2:** Колонки занимают 100% доступной высоты без скролла страницы

**AC-6.3:** Внутренний скролл появляется только внутри колонки при переполнении

**AC-6.4:** Колонки пустые (заглушки для будущих задач)

**Test Cases:**

*Примечание: frontend тесты не требуются по спецификации*

**Verify command (manual):**
```
npm run dev
Открыть http://localhost:3000/sprints
Проверить: 3 колонки видны, занимают всю высоту, скролл только внутри колонок
```

**Dependencies:**
- **Blocked by:** T5

**Size:** S (~1-2 часа)

**Commit:**
```
git add src/frontend/src/sprints/components/SprintColumns.tsx src/frontend/src/sprints/components/SprintColumn.tsx src/frontend/src/sprints/sprints-board.css
git commit -m "Add three-column SprintBoard layout"
```

---

### T7: Интеграция frontend с backend API ✅

**Status:** Реализовано

**Files:**
- `src/frontend/src/sprints/api/sprintsApi.ts` — функции для вызова API
- `src/frontend/src/main.tsx` — маршрутизация на `/sprints`
- `src/frontend/src/App.tsx` — подключение SprintBoardPage
- `src/backend/AgileBoard/Program.cs` — CORS для localhost:3000, подключение MediatR, ExceptionHandlingMiddleware

**Do:**
- Создать `sprintsApi.ts` с функциями: getSprints, getSprintById, createSprint, updateSprint, deleteSprint
- Настроить маршрутизацию на `/sprints`
- Подключить SprintBoardPage к роуту
- Настроить CORS в backend (Program.cs) для localhost:3000
- Протестировать полный цикл: создание → отображение → редактирование → удаление

**Acceptance Criteria:**

**AC-7.1:** Frontend загружает список спринтов при загрузке страницы

**AC-7.2:** Создание спринта через форму → спринт появляется в ComboBox и на доске

**AC-7.3:** Редактирование спринта → изменения отображаются в UI

**AC-7.4:** Удаление спринта → спринт исчезает из ComboBox и доски

**AC-7.5:** Переключение спринта в ComboBox → доска обновляется для выбранного спринта

**AC-7.6:** CORS настроен, запросы с localhost:3000 на localhost:5000 работают

**Test Cases:**

*Примечание: E2E тесты out of scope, но integration тесты backend уже покрыты в T4*

**Verify command (manual):**
```
docker-compose up -d
dotnet run --project src/backend/AgileBoard
npm run dev --prefix src/frontend

1. Открыть http://localhost:3000/sprints
2. Создать спринт "Sprint 1" с датами [сегодня, +14 дней]
3. Проверить: спринт появился в ComboBox и видна доска
4. Создать второй спринт "Sprint 2"
5. Переключиться на "Sprint 2" → доска обновилась
6. Редактировать "Sprint 1" → изменения сохранились
7. Удалить "Sprint 2" → спринт исчез
```

**Dependencies:**
- **Blocked by:** T5, T6

**Size:** S (~1-2 часа)

**Commit:**
```
git add src/frontend/src/sprints/api/sprintsApi.ts src/frontend/src/main.tsx src/frontend/src/App.tsx
git commit -m "Integrate frontend with backend API and configure CORS"
```

---

## Порядок выполнения

**Выполнено:**
```
T1 (Domain) → T2 (Repository+Migration) → T3 (CQRS) → T4 (Controller)
                                              ↓
T7 (Integration) ← T6 (Columns) ← T5 (Frontend Page+Form) ←────────┘
```

**Критический путь:** T1 → T2 → T3 → T4 → T5 → T6 → T7 ✅

**Фактически затрачено:** ~8 часов (все задачи выполнены)

---

## Итоговый статус

| Задача | Статус | Файлы | Тесты |
|--------|--------|-------|-------|
| T1: Domain | ✅ | Sprint.cs, SprintId.cs | 4 unit теста |
| T2: Repository | ✅ | SprintRepository.cs, миграция | 4 интеграционных теста |
| T3: CQRS | ✅ | 8 команд/запросов | 8 unit тестов |
| T4: Controller | ✅ | SprintController.cs | 8 интеграционных тестов |
| T5: Frontend Page | ✅ | 6 компонентов + хуки | Ручная проверка |
| T6: Columns | ✅ | 2 компонента + CSS | Ручная проверка |
| T7: Integration | ✅ | API + CORS + роутинг | E2E ручная |

**Всего тестов:** 24 (Unit + Integration)

**Запуск тестов:**
```bash
dotnet test
```

**Запуск приложения:**
```bash
docker-compose up -d
dotnet run --project src/backend/AgileBoard
npm run dev --prefix src/frontend
```

**URL:**
- Frontend: http://localhost:3000/sprints
- Backend API: http://localhost:5000/api/sprints
