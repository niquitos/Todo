# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**AgileBoard** — веб-приложение с архитектурой .NET backend + React frontend.

## Tech Stack

- **Backend:** ASP.NET Core 8 (Controllers) + EF Core + PostgreSQL
- **Frontend:** React + TypeScript (Vite)
- **Database:** PostgreSQL 15+
- **Testing:** NUnit (backend unit/integration)
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

**Nginx конфигурация:**
- Раздаёт статику из `/app/dist`
- Проксирует `/api/*` на backend:8080
- SPA routing через `try_files`

### Key Patterns

- **CQRS:** IRequest/IRequest<TResult> для команд и запросов
- **Repository:** IReadRepository (expressions), IWriteRepository (aggregates)
- **DDD Lite:** Aggregate Roots с инкапсулированной логикой
- **TDD:** Red → Green → Refactor (только backend)

## Commands

**Запуск приложения:**
```
docker-compose up --build -d     # Сборка и запуск всех сервисов
docker-compose down              # Остановка и удаление контейнеров
docker-compose ps                # Статус контейнеров
docker-compose logs -f backend   # Логи backend
```

**Порты:**
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000/api
- PostgreSQL: localhost:5432

## Project Structure

```
src/
├── frontend/              # React + TypeScript
│   ├── src/
│   │   ├── <feature>/    # Feature folders (sprints)
│   │   │   ├── types/    # TypeScript интерфейсы
│   │   │   ├── api/      # API функции
│   │   │   ├── hooks/    # React хуки
│   │   │   ├── components/
│   │   │   └── pages/
│   │   └── main.tsx      # Entry point
│   ├── Dockerfile        # Multi-stage: node → nginx
│   └── nginx.conf        # SPA routing + API proxy
└── backend/
    ├── AgileBoard/       # Основной проект
    │   ├── Adapters/
    │   │   ├── Infrastructure/
    │   │   ├── Persistence/  # DbContext, Repositories, Migrations
    │   │   └── WebApi/       # Controllers, Filters
    │   ├── Application/
    │   │   ├── UseCases/     # CQRS handlers
    │   │   └── Common/
    │   ├── Domain/           # Aggregates, Value Objects
    │   ├── Program.cs        # DI, CORS, Middleware
    │   └── Dockerfile
    └── AgileBoard.Tests/
        ├── Unit/
        └── Integration/
```

## Specifications

Все задачи и требования документируются в `.ai/specs/*.md`. Каждая спецификация содержит:
- **Why/What** — обоснование и описание
- **Requirements** — функциональные и нефункциональные требования
- **Design** — архитектурные решения
- **Tasks** — задачи с тестами и критериями приёмки

## Database

**Migrations:** EF Core миграции применяются автоматически при старте backend (`EnsureCreated()`).

**Location:** `src/backend/AgileBoard/Adapters/Persistence/Migrations/`
