# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**AgileBoard** — новый проект, начинающийся с нуля. Планируется как смешанный проект (.NET + возможный frontend).

## Current State

- Пустой проект, только инициализированный git-репозиторий
- Ветка по умолчанию: `main`
- `.gitignore` настроен для .NET/Node.js/общих файлов

## Commands

**Git:**
```
git status
git log --oneline
git branch --show-current
```

## Architecture

**Структура:**
- `.ai/specs/` — спецификации задач и функций
- `.gitignore` — правила исключения для git

**Спецификации:**
- Все задачи и требования документируются в `.ai/specs/*.md`
- Каждая спецификация содержит: Why, What, Requirements, Tasks, Test Cases
