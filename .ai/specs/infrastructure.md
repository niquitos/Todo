# Infrastructure

## Why
Создать базовую структуру проекта для полноценной разработки веб-приложения с бекендом, фронтендом и базой данных. Необходимо для начала разработки функционала.

## What
Инициализированная структура папок с настроенными проектами .NET (бекенд), React/TypeScript (фронтенд), конфигурацией Docker и базой данных PostgreSQL. Все компоненты запускаются через docker-compose.

## Context

**Relevant files:**
- `CLAUDE.md` — общие инструкции проекта
- `.gitignore` — правила исключения

**Patterns to follow:**
- Выяснится на этапе проектирования

**Key decisions already made:**
- Выяснится на этапе проектирования

## Требования

### Функциональные

**FR-1: Структура папок**
- Выяснится на этапе проектирования

**FR-2: Backend проект**
- выяснится на этапе проектирования

**FR-3: Frontend проект**
- выяснится на этапе проектирования

**FR-4: Docker конфигурация**
- Выяснится на этапе проектирования

**FR-5: Database**
- Выяснится на этапе проектирования

### Нефункциональные

**NFR-1: Запуск проекта**
- **Metric:** `docker-compose up` поднимает все сервисы

**NFR-2: Сборка**
- **Metric:** Docker образы собираются без ошибок

## Ограничения

**Must:**
- .NET 8 или новее
- Node.js 20+ LTS
- PostgreSQL 15+
- Multi-stage Docker сборку

**Must not:**
- Не добавлять сложные зависимости на старте
- Не добавлять системы кэширования (Redis) пока не потребуется
- Не добавлять message brokers

**Out of scope:**
- CI/CD пайплайны
- E2E тесты (только unit и integration для backend)
- Frontend тесты
- Системы мониторинга и логирования
- Reverse proxy (nginx)

## Критерии приёмки

**AC-1:** `docker-compose up` запускает все 3 сервиса (backend, frontend, database)
**AC-2:** Frontend доступен на: Выяснится на этапе проектирования
**AC-3:** Backend API доступен на: Выяснится на этапе проектирования
**AC-4:** База данных запускается и принимает подключения
**AC-5:** Frontend может делать запросы к backend (CORS настроен)

## Проектирование

### Стек

**Выбранные технологии:**
- **Backend:** ASP.NET Core Controllers + EF Core — стандартный стек для .NET приложений, хорошая интеграция с Docker
- **Frontend:** React (TypeScript) — компонентный подход, типизация
- **Database:** PostgreSQL — надёжная реляционная БД
- **ORM:** Entity Framework Core — Code First подход, миграции
- **Testing:** NUnit — unit-тесты для .NET

**Альтернативы (отклонены):**
- Minimal API вместо Controllers — отклонено: Controllers более явные для новичков
- xUnit вместо NUnit — отклонено: NUnit выбран как предпочтительный
- MongoDB/SQLite — отклонено: PostgreSQL для реляционных данных

### Архитектура

#### Слои backend

| Слой | Ответственность | Компоненты |
|------|-----------------|------------|
| Adapters/Infrastructure | Внешние сервисы | Email, Cache, External APIs |
| Adapters/Persistence | Доступ к данным | Repositories, DbContext, Миграции |
| Adapters/WebApi | HTTP интерфейс | Controllers, Мапперы |
| Application | Бизнес-логика | Use Cases, Валидаторы |
| Domain | Бизнес-модели | Aggregate Roots, Entities, Value Objects |

#### Поток данных

```
[Frontend:3000] → [Backend:5000] → [Controllers] → [UseCases] → [Repositories] → [PostgreSQL:5432]
```

#### Структура папок

```
src/
├── frontend/
│   └── <feature_folder>/
│       ├── components/
│       ├── hooks/
│       └── types/
└── backend/
    ├── AgileBoard/
    │   ├── Adapters/
    │   │   ├── Infrastructure/
    │   │   ├── Persistence/
    │   │   └── WebApi/
    │   ├── Application/
    │   │   ├── UseCases/
    │   │   └── Common/
    │   └── Domain/
    └── AgileBoard.Tests/
        ├── Unit/
        │   ├── Adapters/
        │   ├── UseCases/
        │   └── Domain/
        ├── Integration/
        └── Utils/
```

### Паттерны

**Pattern 1: CQRS (Command Query Responsibility Segregation)**
- **Problem:** Разделение операций чтения и записи
- **Solution:** IRequest/IRequest<TResult> для команд и запросов
- **Location:** `src/backend/AgileBoard/Application/Common/`

**Pattern 2: Repository (CQRS-aware)**
- **Problem:** Абстракция доступа к данным
- **Solution:** IReadRepository (expression queries), IWriteRepository (aggregates)
- **Location:** `src/backend/AgileBoard/Adapters/Persistence/`

**Pattern 3: DDD Lite**
- **Problem:** Предотвращение анемичной модели
- **Solution:** Aggregate Roots с инкапсулированной логикой, Value Objects
- **Location:** `src/backend/AgileBoard/Domain/`

**Pattern 4: TDD**
- **Problem:** Гарантия работоспособности кода
- **Solution:** Red → Green → Refactor цикл
- **Flow:** empty method → test → test red → implement → test green
- **Scope:** Только backend (unit + integration тесты)

### Порты сервисов

| Сервис | Порт | Протокол |
|--------|------|----------|
| PostgreSQL | 5432 | TCP |
| Backend API | 5000 | HTTP |
| Frontend | 3000 | HTTP |

### Docker-конфигурация

**Сервисы:**
1. `db` — PostgreSQL с volume для персистентности
2. `backend` — .NET API с hot reload (dev)
3. `frontend` — React Vite с HMR (dev)

**Файлы:**
- `docker-compose.yml` — оркестрация
- `docker-compose.override.yml` — dev overrides
- `.dockerignore` — исключение node_modules, bin, obj

### Риски

| Риск | Вероятность | Влияние | Митигация |
|------|-------------|---------|-----------|
| Сложность структуры для старта | Средняя | Высокое | Начать с минимума, расширять по мере роста |
| CORS проблемы между frontend/backend | Низкая | Среднее | Настроить proxy в Vite |
| EF Core миграции конфликтуют | Средняя | Низкое | Одна команда работает с миграциями |

### Этапы реализации

**Stage 1:** Структура папок — создать все директории
**Stage 2:** Domain + Application слои — ядро бизнес-логики
**Stage 3:** Persistence — DbContext, repositories, миграции
**Stage 4:** WebApi — controllers, маппинг
**Stage 5:** Frontend — React приложение
**Stage 6:** Docker — containerization + интеграция

## Задачи

### T1: Структура папок проекта

**Files:** Вся корневая директория

**Do:**
- Создать папки `src/frontend/`, `src/backend/`, `docker/`
- Создать структуру backend: `AgileBoard/{Adapters/{Infrastructure,Persistence,WebApi},Application/{UseCases,Common},Domain}`
- Создать структуру тестов: `AgileBoard.Tests/{Unit/{Adapters,UseCases,Domain},Integration,Utils}`
- Создать базовые `.gitkeep` файлы в пустых папках

**Acceptance Criteria:**

**AC-1:** Все папки существуют и отображаются в дереве проекта

**Test Cases:**

#### Test-1: Validation
**Type:** Validation
**Links:** AC-1

**Preconditions:**
- Чистая директория проекта

**Action:**
```
tree src /F
```

**Expected:**
- Присутствуют все папки из структуры в разделе "Проектирование"

**Verify command:**
```
test-path "src/backend/AgileBoard/Domain"
test-path "src/backend/AgileBoard/Application/Common"
test-path "src/backend/AgileBoard/Adapters/WebApi"
test-path "src/backend/AgileBoard.Tests/Unit"
test-path "src/backend/AgileBoard.Tests/Integration"
test-path "src/frontend"
```

**Dependencies:**
- **Blocks:** T2, T3, T4, T5

**Size:** S (~15 мин)

---

### T2: Domain слой — агрегаты и value objects

**Files:** 
- `src/backend/AgileBoard/Domain/`
- `src/backend/AgileBoard.Tests/Unit/Domain/`

**Do:**
- Создать базовые классы: `AggregateRoot<TId>`, `Entity<TId>`, `ValueObject<T>`
- Создать пример агрегата (например, `Task` или `Board`)
- Реализовать Value Object для примера (например, `TaskTitle`, `TaskStatus`)
- Создать пустые unit-тесты для доменных объектов

**Acceptance Criteria:**

**AC-1:** Domain слой компилируется без ошибок
**AC-2:** Unit-тесты запускаются через `dotnet test`

**Test Cases:**

#### Test-1: ValueObject equality
**Type:** Unit
**Links:** AC-1

**Preconditions:**
- Domain слой создан

**Action:**
```csharp
var title1 = new TaskTitle("Test");
var title2 = new TaskTitle("Test");
var result = title1.Equals(title2);
```

**Expected:**
```
result == true
```

**Verify command:**
```
dotnet test --filter "ValueObject" --no-build
```

**Dependencies:**
- **Blocked by:** T1
- **Blocks:** T3

**Size:** M (~1-2 часа)

---

### T3: Application слой — CQRS основы

**Files:**
- `src/backend/AgileBoard/Application/Common/`
- `src/backend/AgileBoard/Application/UseCases/`

**Do:**
- Создать интерфейсы: `IRequest`, `IRequest<TResponse>`, `IRequestHandler<TRequest>`, `IRequestHandler<TRequest, TResponse>`
- Создать интерфейс валидатора: `IValidator<TRequest>`
- Создать пример UseCase (Query + Handler)
- Создать пример UseCase (Command + Handler)

**Acceptance Criteria:**

**AC-1:** Интерфейсы определены и готовы к использованию
**AC-2:** Примеры UseCase компилируются

**Test Cases:**

#### Test-1: Handler registration
**Type:** Unit
**Links:** AC-1

**Preconditions:**
- Application слой создан

**Action:**
```
dotnet build src/backend/AgileBoard
```

**Expected:**
- Build успешен (exit code 0)

**Verify command:**
```
dotnet build src/backend/AgileBoard --no-restore
assert_exit_code 0
```

**Dependencies:**
- **Blocked by:** T2
- **Blocks:** T4

**Size:** M (~1-2 часа)

---

### T4: Persistence слой — EF Core repositories

**Files:**
- `src/backend/AgileBoard/Adapters/Persistence/`
- `src/backend/AgileBoard.Tests/Integration/`

**Do:**
- Создать `AppDbContext : DbContext`
- Создать интерфейсы: `IReadRepository<T>`, `IWriteRepository<T>`
- Реализовать `Repository<T> : IReadRepository<T>, IWriteRepository<T>`
- Настроить `DbSet<T>` для примера агрегата
- Создать первую миграцию
- Создать интеграционные тесты для репозитория

**Acceptance Criteria:**

**AC-1:** Репозитории работают с EF Core
**AC-2:** Миграция создаёт таблицу в БД
**AC-3:** Интеграционные тесты проходят

**Test Cases:**

#### Test-1: Repository write and read
**Type:** Integration
**Links:** AC-1, AC-2

**Preconditions:**
- PostgreSQL запущен
- Миграции применены

**Action:**
```csharp
var entity = new Aggregate { Name = "Test" };
await writeRepository.AddAsync(entity);
await unitOfWork.CommitAsync();
var found = await readRepository.GetByIdAsync(entity.Id);
```

**Expected:**
```
found != null
found.Name == "Test"
```

**Verify command:**
```
dotnet test --filter "Integration" --no-build
```

**Dependencies:**
- **Blocked by:** T3
- **Blocks:** T5

**Size:** L (~2-4 часа)

---

### T5: WebApi слой — контроллеры

**Files:**
- `src/backend/AgileBoard/Adapters/WebApi/`
- `src/backend/AgileBoard.Tests/Unit/Adapters/WebApi/`

**Do:**
- Создать базовый `BaseController` с common логикой
- Создать пример контроллера для агрегата
- Настроить маппинг (Request → Command/Query, Response)
- Настроить CORS для frontend (порт 3000)
- Добавить health check endpoint: `GET /health`
- Настроить DI контейнер (MediatR, EF Core, Repositories)

**Acceptance Criteria:**

**AC-1:** API запускается на порту 5000
**AC-2:** Health check возвращает 200 OK
**AC-3:** CORS настроен для localhost:3000

**Test Cases:**

#### Test-1: Health check endpoint
**Type:** Validation
**Links:** AC-2

**Preconditions:**
- Backend запущен на порту 5000

**Action:**
```
HTTP GET http://localhost:5000/health
```

**Expected:**
- Status: 200 OK
- Body: `{"status":"healthy"}`

**Verify command:**
```
http_get http://localhost:5000/health
assert_status 200
assert_field status == "healthy"
```

#### Test-2: CORS headers
**Type:** Validation
**Links:** AC-3

**Preconditions:**
- Backend запущен

**Action:**
```
HTTP OPTIONS http://localhost:5000/api/example
Origin: http://localhost:3000
```

**Expected:**
- Status: 200 OK
- Header `Access-Control-Allow-Origin: http://localhost:3000`

**Verify command:**
```
http_options http://localhost:5000/api/example -H "Origin: http://localhost:3000"
assert_header "Access-Control-Allow-Origin" == "http://localhost:3000"
```

**Dependencies:**
- **Blocked by:** T4
- **Blocks:** T6, T7

**Size:** L (~3-4 часа)

---

### T6: Frontend — React приложение

**Files:**
- `src/frontend/`

**Do:**
- Инициализировать React + TypeScript проект через Vite
- Создать базовую структуру папок по фичам
- Настроить proxy на backend (localhost:5000)
- Создать приветственную страницу с проверкой connection к API
- Создать пример компонента с запросом к `/health`
- Настроить ESLint + Prettier

**Acceptance Criteria:**

**AC-1:** Frontend запускается на порту 3000
**AC-2:** Vite HMR работает (изменения видны без перезагрузки)
**AC-3:** Запрос к backend успешен

**Test Cases:**

#### Test-1: Frontend serves page
**Type:** Validation
**Links:** AC-1

**Preconditions:**
- Frontend запущен

**Action:**
```
HTTP GET http://localhost:3000
```

**Expected:**
- Status: 200 OK
- Body содержит HTML с React root

**Verify command:**
```
http_get http://localhost:3000
assert_status 200
assert_body_contains "root"
```

**Dependencies:**
- **Blocked by:** T1
- **Blocks:** T7

**Size:** M (~2-3 часа)

---

### T7: Docker конфигурация

**Files:**
- `docker-compose.yml`
- `docker-compose.override.yml`
- `docker/backend/Dockerfile`
- `docker/frontend/Dockerfile`
- `.dockerignore` (backend + frontend)

**Do:**
- Создать Dockerfile для backend (multi-stage: build → run)
- Создать Dockerfile для frontend (multi-stage: build → serve)
- Создать docker-compose.yml с 3 сервисами (db, backend, frontend)
- Создать docker-compose.override.yml для dev (volumes, watch)
- Настроить .dockerignore для исключения bin, obj, node_modules
- Настроить health checks для всех сервисов

**Acceptance Criteria:**

**AC-1:** `docker-compose up` поднимает все сервисы
**AC-2:** Все health checks проходят
**AC-3:** Сервисы видят друг друга

**Test Cases:**

#### Test-1: Все сервисы запущены
**Type:** Validation
**Links:** AC-1

**Preconditions:**
- Docker установлен

**Action:**
```
docker-compose up -d
docker-compose ps
```

**Expected:**
- 3 сервиса в статусе "Up"
- Health: healthy

**Verify command:**
```
docker-compose ps
assert_count_services == 3
assert_all_healthy
```

#### Test-2: Backend доступен из контейнера
**Type:** Validation
**Links:** AC-3

**Preconditions:**
- Docker compose запущен

**Action:**
```
docker-compose exec backend wget -q -O - http://localhost:5000/health
```

**Expected:**
- Status: 200 OK
- Body: `{"status":"healthy"}`

**Verify command:**
```
docker-compose exec backend wget -q -O - http://localhost:5000/health
assert_body_contains "healthy"
```

**Dependencies:**
- **Blocked by:** T5

**Size:** L (~3-4 часа)