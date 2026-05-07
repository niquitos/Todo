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
Frontend:3000 → Backend:5000 → Controllers → UseCases → Repositories → PostgreSQL:5432
```

### Key Patterns

- **CQRS:** IRequest/IRequest<TResult> для команд и запросов
- **Repository:** IReadRepository (expressions), IWriteRepository (aggregates)
- **DDD Lite:** Aggregate Roots с инкапсулированной логикой
- **TDD:** Red → Green → Refactor (только backend)

## Commands

**Git:**
```
git status
git log --oneline
git branch --show-current
```

**Build & Run (Docker):**
```
docker-compose up          # Запуск всех сервисов
docker-compose up -d       # Фоновый режим
docker-compose ps          # Статус контейнеров
```

**.NET:**
```
dotnet build               # Сборка
dotnet test                # Все тесты
dotnet test --filter "Name~TestName"  # Один тест
```

**Node.js:**
```
npm install                # Установка зависимостей
npm run dev                # Dev-сервер (Vite)
```

## Project Structure

```
src/
├── frontend/              # React + TypeScript
│   └── <feature>/
│       ├── components/
│       ├── hooks/
│       └── types/
└── backend/
    ├── AgileBoard/        # Основной проект
    │   ├── Adapters/
    │   │   ├── Infrastructure/
    │   │   ├── Persistence/
    │   │   └── WebApi/
    │   ├── Application/
    │   │   ├── UseCases/
    │   │   └── Common/
    │   └── Domain/
    └── AgileBoard.Tests/  # NUnit тесты
        ├── Unit/
        └── Integration/
```

## Specifications

Все задачи и требования документируются в `.ai/specs/*.md`. Каждая спецификация содержит:
- **Why/What** — обоснование и описание
- **Requirements** — функциональные и нефункциональные требования
- **Design** — архитектурные решения
- **Tasks** — задачи с тестами и критериями приёмки
