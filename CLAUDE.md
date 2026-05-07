# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**AgileBoard** — веб-приложение с архитектурой .NET backend + React frontend.

## Tech Stack

- **Backend:** ASP.NET Core 8 (Controllers) + EF Core 8 + MediatR 12.2
- **Frontend:** React 19 + TypeScript (Vite 6)
- **Database:** PostgreSQL 15+
- **Testing:** NUnit 3.14 + Moq 4.20 (unit/integration)
- **Containerization:** Docker + docker-compose

## Architecture

### Backend Layers

| Layer | Responsibility |
|-------|----------------|
| Adapters/Infrastructure | Внешние сервисы (Email, Cache) |
| Adapters/Persistence | Repositories, DbContext |
| Adapters/WebApi | Controllers, DTOs |
| Application | Use Cases (CQRS), Validators |
| Domain | Aggregate Roots, Entities, Value Objects |

### Data Flow

```
Frontend:3000 (nginx) → Backend:5000 → Controllers → UseCases → Repositories → PostgreSQL:5432
```

Фактический docker-compose мапинг: `5000:8080` (внешний 5000 → контейнер 8080).

### Frontend Architecture

**Feature-based organization:**
```
src/frontend/src/<feature>/
├── types/       # TypeScript интерфейсы
├── api/         # API функции (fetch)
├── hooks/       # React хуки
├── components/  # UI компоненты
└── pages/       # Страницы
```

**Точка входа:** `main.tsx` рендерит страницу напрямую (без App.tsx роутера). Маршрутизация через `react-router-dom` подключается внутри feature-страниц.

**Nginx конфигурация:**
- Раздаёт статику из `/app/dist`
- Проксирует `/api/*` на `backend:8080`
- SPA routing через `try_files`

### Key Patterns

- **CQRS:** `IRequest`/`IRequest<TResult>` (MediatR) для команд и запросов
- **Repository:** `ISprintRepository` с методами GetById, GetAll, Add, Update, Delete
- **DDD Lite:** Aggregate Roots с инкапсулированной логикой, Value Objects
- **TDD:** Red → Green → Refactor (только backend)
- **InternalsVisibleTo:** `AgileBoard.Tests` имеет доступ к internal-членам `AgileBoard`

## Commands

### Docker

```
docker-compose up --build -d     # Сборка и запуск всех сервисов
docker-compose down              # Остановка и удаление контейнеров
docker-compose ps                # Статус контейнеров
docker-compose logs -f backend   # Логи backend
```

### Backend

```
dotnet test                                                 # Все тесты
dotnet test --filter "FullyQualifiedName~TestName"          # Один тест
dotnet test --filter "FullyQualifiedName~SprintTests"       # Все тесты класса
dotnet run --project src/backend/AgileBoard                 # Запуск локально
```

**EF Core миграции:**
```
dotnet ef migrations add <Name> --project src/backend/AgileBoard   # Создать миграцию
dotnet ef database update --project src/backend/AgileBoard         # Применить миграции
```

Миграции применяются автоматически через `EnsureCreated()` при старте backend.

### Frontend

```
npm --prefix src/frontend run dev      # Dev-сервер Vite (HMR)
npm --prefix src/frontend run build    # Production сборка (tsc + vite)
npm --prefix src/frontend run preview  # Preview production сборки
```

## Endpoints

| Endpoint | Метод | Описание |
|----------|-------|----------|
| `/api/sprints` | GET | Список всех спринтов |
| `/api/sprints/{id}` | GET | Спринт по ID |
| `/api/sprints` | POST | Создать спринт |
| `/api/sprints/{id}` | PUT | Обновить спринт |
| `/api/sprints/{id}` | DELETE | Удалить спринт |
| `/swagger` | GET | Swagger UI (только Development) |

### Обработка ошибок

- `SprintOverlapException` → 400 Bad Request (через `ExceptionHandlingMiddleware`)
- `NotFoundException` → 404 Not Found

## Docker Configuration

**Основной файл:** `docker-compose.yml` — 3 сервиса:
1. `db` — PostgreSQL 15 с volume `postgres_data`
2. `backend` — .NET API, порт `5000:8080`
3. `frontend` — React (nginx), порт `3000:80`

**Override:** `docker-compose.override.yml` добавляет:
- `VITE_API_URL=http://backend:8080` — URL API для frontend в Docker-сети

**Внутренняя сеть:** сервисы общаются по именам контейнеров (`backend`, `db`). Connection string в контейнере: `Host=db;Database=agileboard;Username=postgres;Password=postgres`.

## CORS

Политика `"AllowFrontend"` в `Program.cs` разрешает запросы с `http://localhost:3000`. При разработке вне Docker-a frontend может делать запросы к backend на `http://localhost:5000`.

## Ports

| Сервис | Внешний порт | Внутренний порт |
|--------|-------------|-----------------|
| Frontend | 3000 | 80 (nginx) |
| Backend API | 5000 | 8080 |
| PostgreSQL | 5432 | 5432 |

## Project Structure

```
src/
├── frontend/              # React + TypeScript
│   ├── src/
│   │   ├── <feature>/    # Feature folders (sprints, tasks, etc.)
│   │   │   ├── types/    # TypeScript интерфейсы
│   │   │   ├── api/      # API функции (fetch)
│   │   │   ├── hooks/    # React хуки
│   │   │   ├── components/
│   │   │   └── pages/
│   │   └── main.tsx      # Entry point (без App.tsx)
│   ├── Dockerfile        # Multi-stage: node → nginx
│   └── nginx.conf        # SPA routing + API proxy
└── backend/
    ├── AgileBoard/       # Основной проект
    │   ├── Adapters/
    │   │   ├── Infrastructure/
    │   │   ├── Persistence/  # DbContext, Repositories, Migrations
    │   │   └── WebApi/       # Controllers, Filters
    │   ├── Application/
    │   │   ├── UseCases/     # CQRS handlers (по фичам)
    │   │   └── Common/
    │   ├── Domain/           # Aggregates, Value Objects
    │   ├── Program.cs        # DI, CORS, MediatR, Middleware
    │   └── Dockerfile
    └── AgileBoard.Tests/
        ├── Unit/
        │   ├── Domain/
        │   ├── UseCases/
        │   └── Adapters/
        └── Integration/
```

## Specifications

Все задачи и требования документируются в `.ai/specs/*.md`. Каждая спецификация содержит:
- **Why/What** — обоснование и описание
- **Requirements** — функциональные и нефункциональные требования
- **Design** — архитектурные решения
- **Tasks** — задачи с тестами и критериями приёмки

Готовые спецификации (реализованные): `infrastructure.md`, `sprints-and-board.md`.

## Database

**Migrations:** EF Core миграции в `src/backend/AgileBoard/Adapters/Persistence/Migrations/`. Применяются автоматически при старте backend через `EnsureCreated()`.

**Подключение локально (вне Docker):** требуется запущенный PostgreSQL на `localhost:5432` с `POSTGRES_DB=agileboard`.
