# Инициализация git-репозитория для AgileBoard

## Why
Проект AgileBoard начинается с нуля. Необходим git-репозиторий для версионирования кода, отслеживания изменений и будущей командной работы.

## What
- Инициализированный git-репозиторий в корневой директории проекта
- Файл `.gitignore` с правилами для .NET/Node.js/общих файлов
- Первый коммит с начальным состоянием проекта
- Ветка `main` как основная

## Context

**Текущее состояние:**
- Директория `D:\Users\LocalUser\source\Проекты\Todo\.ai\specs` существует
- Проект пустой — файлов кода ещё нет
- Требуется создать `.gitignore` перед инициализацией

**Patterns to follow:**
- Стандартный `.gitignore` для смешанного проекта (.NET + возможный frontend)
- Ветка по умолчанию: `main`

## Требования

### Функциональные

**FR-1: Создание репозитория**
- Инициализировать git в корневой директории проекта
- Создать ветку `main`

**FR-2: Файл .gitignore**
- Исключить: `bin/`, `obj/`, `node_modules/`, `.env`, `*.user`, `.vs/`, `.idea/`
- Исключить временные файлы и артефакты сборки

**FR-3: Начальный коммит**
- Сделать первый коммит с `.gitignore`
- Сообщение: "Initial commit: add .gitignore"

### Нефункциональные

**NFR-1: Скорость**
- **Metric:** < 30 секунд на полную инициализацию

**NFR-2: Чистота**
- **Metric:** 0 незакоммиченных файлов после выполнения

## Ограничения

**Must:**
- Использовать стандартные git-команды без флаков обхода хуков
- Создать `.gitignore` до первого коммита

**Must not:**
- Не добавлять удалённые репозитории (origin) — это сделает пользователь
- Не создавать дополнительных веток кроме `main`

**Out of scope:**
- Настройка CI/CD
- Подключение к GitHub/GitLab
- Создание README или документации

## Критерии приёмки

**AC-1:** `git status` показывает чистое состояние (nothing to commit)
**AC-2:** `git branch --show-current` возвращает `main`
**AC-3:** `git log --oneline` показывает 1 коммит
**AC-4:** Файл `.gitignore` существует в корне проекта

## Проектирование

### Стек

**Инструменты:**
- **git** — система версионирования
- **bash** — оболочка для выполнения команд

### Структура .gitignore

```
# .NET / Visual Studio
bin/
obj/
*.user
.vs/
.idea/

# Node.js
node_modules/

# Environment
.env
.env.local
.env.*.local

# Claude Code
.claude/

# OS
.DS_Store
Thumbs.db

# Logs
*.log
npm-debug.log*
```

### Этапы реализации

**Stage 1:** Создание `.gitignore` в корне проекта  
**Stage 2:** Инициализация git-репозитория  
**Stage 3:** Создание ветки `main`  
**Stage 4:** Начальный коммит

### Риски

| Риск | Вероятность | Влияние | Митигация |
|------|-------------|---------|-----------|
| Конфликт имён веток | Низкая | Низкое | Явное указание `-b main` при инициализации |
| Файлы уже в директории | Низкая | Среднее | Проверка через `ls` перед коммитом |

## Задачи

### T1: Создать файл .gitignore

**Files:** `.gitignore`

**Do:**
- Создать файл `.gitignore` в корне проекта
- Добавить правила для .NET, Node.js, environment, .claude/, OS, логов

**Acceptance Criteria:**

**AC-1:** Файл существует в корне проекта  
**AC-2:** Содержит все требуемые правила

**Test Cases:**

#### Test-1: Файл существует
**Type:** Validation  
**Links:** AC-1

**Preconditions:**
- Нет файлов

**Action:**
```
ls -la .gitignore
```

**Expected:**
- Файл найден

**Verify command:**
```
test -f .gitignore && echo "OK" || echo "FAIL"
```

#### Test-2: Содержит требуемые правила
**Type:** Validation  
**Links:** AC-2

**Preconditions:**
- Файл .gitignore существует

**Action:**
```
grep -E "^(bin/|obj/|node_modules/|.env|.claude/)" .gitignore
```

**Expected:**
- Найдены все 5+ правил

**Verify command:**
```
grep -c "^#\|^bin/\|^obj/\|^node_modules/\|^.env\|^.claude/" .gitignore
```

**Dependencies:**
- **Blocks:** T2

**Size:** S (~15 минут)

---

### T2: Инициализировать git-репозиторий

**Files:** `.git/`

**Do:**
- Инициализировать git в корне проекта
- Создать ветку `main`

**Acceptance Criteria:**

**AC-1:** `git branch --show-current` возвращает `main`

**Test Cases:**

#### Test-1: Ветка main существует
**Type:** Validation  
**Links:** AC-1

**Preconditions:**
- .gitignore существует

**Action:**
```
git init -b main
```

**Expected:**
- Репозиторий инициализирован
- Текущая ветка: main

**Verify command:**
```
git branch --show-current
```

**Dependencies:**
- **Blocked by:** T1
- **Blocks:** T3

**Size:** S (~10 минут)

---

### T3: Создать начальный коммит

**Files:** `.git/`

**Do:**
- Добавить .gitignore в staging
- Сделать коммит с сообщением "Initial commit: add .gitignore"

**Acceptance Criteria:**

**AC-1:** `git status` показывает чистое состояние  
**AC-2:** `git log --oneline` показывает 1 коммит

**Test Cases:**

#### Test-1: Чистое состояние
**Type:** Validation  
**Links:** AC-1

**Preconditions:**
- Git инициализирован
- .gitignore добавлен

**Action:**
```
git add .gitignore
git commit -m "Initial commit: add .gitignore"
git status
```

**Expected:**
- nothing to commit, working tree clean

**Verify command:**
```
git status --short
```

#### Test-2: Один коммит
**Type:** Validation  
**Links:** AC-2

**Preconditions:**
- Коммит создан

**Action:**
```
git log --oneline
```

**Expected:**
- 1 коммит с сообщением "Initial commit: add .gitignore"

**Verify command:**
```
git log --oneline | wc -l
```

**Dependencies:**
- **Blocked by:** T2

**Size:** S (~10 минут)
