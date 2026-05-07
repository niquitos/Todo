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

**Files:** `src/backend/`, `src/frontend/`, `docker-compose.yml`

**Do:**
- Создать структуру папок для backend (.NET 8)
- Создать структуру папок для frontend (React/TypeScript)
- Создать базовые Docker конфигурации
- Инициализировать .NET проект и npm проект

**Acceptance Criteria:**

**AC-1:** Все директории существуют согласно схеме в разделе "Проектирование"

**Test Cases:**

#### Test-1: Структура папок backend
**Type:** Validation
**Links:** AC-1

**Preconditions:**
- PowerShell/bash терминал открыт в корне проекта

**Action:**
```
ls src/backend/AgileBoard/
```

**Expected:**
- Папки: Adapters/, Application/, Domain/

**Verify command:**
```
Test-Path src/backend/AgileBoard/Adapters && Test-Path src/backend/AgileBoard/Application && Test-Path src/backend/AgileBoard/Domain
```

#### Test-2: Структура папок frontend
**Type:** Validation
**Links:** AC-1

**Preconditions:**
- PowerShell/bash терминал открыт в корне проекта

**Action:**
```
ls src/frontend/
```

**Expected:**
- Существует папка src/frontend/

**Verify command:**
```
Test-Path src/frontend/
```

#### Test-3: .NET проект инициализирован
**Type:** Validation
**Links:** AC-1

**Preconditions:**
- .NET 8 SDK установлен

**Action:**
```
dotnet --version
```

**Expected:**
- Версия .NET 8.x.x

**Verify command:**
```
dotnet --version
```

**Dependencies:**
- **Blocks:** T2, T3
- **Blocked by:** —

**Size:** S (~30 мин)

---

### T2: Persistence — DbContext + DI регистрация

**Files:** `src/backend/AgileBoard/Adapters/Persistence/AppDbContext.cs`, `src/backend/AgileBoard/Adapters/Persistence/DependencyInjection.cs`

**Do:**
- Создать пустой AppDbContext (без миграций)
- Создать метод расширения AddPersistence() для DI
- Добавить connectionString в appsettings.json

**Acceptance Criteria:**

**AC-1:** AppDbContext существует и наследует DbContext
**AC-2:** DI регистрация работает через AddPersistence()

**Test Cases:**

#### Test-1: AppDbContext создаётся
**Type:** Unit
**Links:** AC-1

**Preconditions:**
- EF Core пакет установлен

**Action:**
```
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase("Test")
    .Options;
var context = new AppDbContext(options);
```

**Expected:**
```
context != null
context.Database != null
```

**Verify command:**
```
run_tests --filter "AppDbContext creates"
```

#### Test-2: DI регистрация добавляет сервисы
**Type:** Unit
**Links:** AC-2

**Preconditions:**
- IServiceCollection настроен

**Action:**
```
var services = new ServiceCollection();
services.AddPersistence("Host=localhost;Database=test;Username=postgres;Password=pass");
var provider = services.BuildServiceProvider();
var context = provider.GetService<AppDbContext>();
```

**Expected:**
```
context != null
```

**Verify command:**
```
run_tests --filter "DI registration"
```

**Dependencies:**
- **Blocks:** T3
- **Blocked by:** T1

**Size:** S (~1 час)

---

### T7: Docker — оркестрация сервисов

**Files:** `docker-compose.yml`, `docker-compose.override.yml`, `.dockerignore`, `Dockerfile`

**Do:**
- Создать docker-compose.yml для 3 сервисов
- Настроить Dockerfile для backend и frontend
- Настроить volume для PostgreSQL
- Настроить network между сервисами

**Acceptance Criteria:**

**AC-1:** `docker-compose up` запускает все 3 сервиса
**AC-2:** Frontend доступен на localhost:3000
**AC-3:** Backend API доступен на localhost:5000
**AC-4:** База данных запускается и принимает подключения

**Test Cases:**

#### Test-1: Все сервисы запускаются
**Type:** Validation
**Links:** AC-1

**Preconditions:**
- Docker Desktop запущен
- docker-compose.yml существует

**Action:**
```
docker-compose up -d
```

**Expected:**
- 3 контейнера в статусе "Up"

**Verify command:**
```
docker-compose ps
assert_count 3
assert_status "Up"
```

#### Test-2: Backend отвечает на health check
**Type:** Validation
**Links:** AC-3

**Preconditions:**
- docker-compose up выполнен

**Action:**
```
HTTP GET http://localhost:5000/health
```

**Expected:**
- Status: 200 OK

**Verify command:**
```
http_get http://localhost:5000/health
assert_status 200
```

#### Test-3: PostgreSQL принимает подключения
**Type:** Validation
**Links:** AC-4

**Preconditions:**
- docker-compose up выполнен

**Action:**
```
docker-compose exec db psql -U postgres -c "SELECT 1"
```

**Expected:**
- Возвращает "?column? | 1"

**Verify command:**
```
docker-compose exec db psql -U postgres -c "SELECT 1"
assert_output_contains "1"
```

#### Test-4: Backend → Database подключение
**Type:** Integration
**Links:** AC-4

**Preconditions:**
- docker-compose up выполнен
- Backend запущен

**Action:**
```
HTTP GET http://localhost:5000/health
```

**Expected:**
- Status: 200 OK
- Backend подтверждает подключение к БД

**Verify command:**
```
http_get http://localhost:5000/health
assert_status 200
```

**Dependencies:**
- **Blocks:** —
- **Blocked by:** T1, T2

**Size:** M (~2 часа)

---

