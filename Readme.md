# Todo

Учебный REST API проект на ASP.NET Core, построенный по принципам Clean Architecture и SOLID. Цель проекта — не просто реализовать CRUD, а прокачать практические навыки backend-разработки на реалистичной, приближенной к production кодовой базе: архитектурные решения важны здесь не меньше, чем функциональность.

Это переписанная на .NET версия ранее реализованного на Java учебного todo-list проекта — способ показать профессиональный рост.

## Архитектура

Проект разделён на 4 слоя по Clean Architecture, с явными зависимостями между ними через project references:

```
src/
  Todo.Domain/           — сущности, доменные инварианты (TodoItem.Create(), TodoItemConstraints)
  Todo.Application/      — use case'ы, DTO, валидаторы, mapping-профили, интерфейсы репозиториев
  Todo.Infrastructure/   — EF Core, DbContext, EF-миграции, реализация репозиториев
  Todo.WebApi/           — контроллеры, DI-композиция, middleware, Program.cs

tests/
  Todo.Tests/             — unit-тесты (сервисы, валидаторы) и интеграционные тесты (WebApplicationFactory)
```

Граф зависимостей однонаправленный:

```
Todo.WebApi ──────┐
                  ├──> Todo.Application ──> Todo.Domain
Todo.Infrastructure ──> Todo.Application ──> Todo.Domain
```

`Domain` и `Application` ничего не знают о том, что их вызывает именно Web API — интерфейс репозитория (`ITodoRepository`) объявлен в `Application`, а реализован в `Infrastructure` (принцип Dependency Inversion).

## Стек

- **ASP.NET Core** (net10.0)
- **Entity Framework Core** с **SQLite**, миграции через `dotnet ef`
- **FluentValidation** — валидация входных DTO через кастомный `IAsyncActionFilter`
- **AutoMapper** — маппинг доменных сущностей в response DTO
- **Serilog** — конфигурируемое логирование (Console/Debug или файловый sink), провайдер переключается через `appsettings.json`
- **Scalar** — интерактивная документация OpenAPI (доступна в Development)
- **xUnit, Moq, FluentAssertions** — тестирование; интеграционные тесты поднимают `WebApplicationFactory` поверх SQLite in-memory

## Ключевые архитектурные решения

- Создание сущности инкапсулировано в доменной модели через статический factory-метод `TodoItem.Create()`, а не в mapping-профиле — инварианты создания принадлежат домену.
- Ограничения на `Title` (`TodoItemConstraints.TitleMinLength/TitleMaxLength`) — единый источник истины, используется и в FluentValidation-валидаторах, и в EF Core Fluent API конфигурации, чтобы исключить рассинхронизацию между слоями.
- Время получается через `TimeProvider` (DI), а не напрямую через `DateTime.UtcNow` — делает время-зависимую логику тестируемой.
- Глобальная обработка ошибок через `IExceptionHandler` + `ProblemDetails` (RFC 9457), запись — через `IProblemDetailsService.TryWriteAsync()`, чтобы не терять централизованную customization pipeline.
- Сервисный слой обёрнут decorator'ом для логирования (`TodoServiceLogger`) через Scrutor — сквозная функциональность не смешана с бизнес-логикой.
- Обновление статуса вынесено в отдельный `UpdateTodoStatusDto`/эндпоинт, а не переиспользует полный `UpdateAsync` — разные операции с разной семантикой не смешиваются в одном контракте.
- Пагинация валидируется явно (`TodoQueryParametersValidator`) — невалидные `page`/`pageSize` отклоняются с `400`, а не тихо приводятся к дефолтным значениям.

## API

Базовый маршрут: `api/v2/Todos`

| Метод | Маршрут | Описание |
|---|---|---|
| `GET` | `/api/v2/Todos` | Список задач: пагинация, фильтрация, поиск, сортировка (см. ниже) |
| `GET` | `/api/v2/Todos/{id}` | Получить задачу по id |
| `POST` | `/api/v2/Todos` | Создать задачу |
| `PUT` | `/api/v2/Todos/{id}` | Обновить задачу целиком |
| `PATCH` | `/api/v2/Todos/{id}/status` | Изменить только статус выполнения |
| `DELETE` | `/api/v2/Todos/{id}` | Удалить задачу |

### Параметры `GET /api/v2/Todos`

| Параметр | Тип | По умолчанию | Описание |
|---|---|---|---|
| `page` | int | `1` | Номер страницы, `>= 1` |
| `pageSize` | int | `10` | Размер страницы, `1..100` |
| `isCompleted` | bool? | — | Фильтр по статусу |
| `search` | string? | — | Поиск по подстроке в `Title` |
| `sortBy` | string | `createdAt` | `title` \| `isCompleted` \| `createdAt` |
| `sortDir` | string | `desc` | `asc` \| `desc` |

Ответ — `PagedResult<TodoResponseDto>`: `items`, `totalCount`, `page`, `pageSize`, `totalPages`, `hasNextPage`, `hasPreviousPage`.

Полная интерактивная документация — через Scalar UI в Development-режиме: `https://localhost:{port}/scalar`. Примеры запросов для всех эндпоинтов — в `src/Todo.WebApi/Todo.http`.

## Запуск проекта

```bash
git clone <repo-url>
cd Todo

dotnet restore
dotnet build

dotnet run --project src/Todo.WebApi
```

По умолчанию API поднимается на `http://localhost:5007` (см. `launchSettings.json`).

### База данных

При первом запуске EF Core Migrations применяются автоматически (`context.Database.MigrateAsync()` в `Program.cs`), файл SQLite создаётся по connection string из `appsettings.json`/`appsettings.Development.json`. На пустой базе при старте засеивается 10 тестовых задач.

Применить или создать новую миграцию вручную:

```bash
dotnet ef migrations add <Name> --project src/Todo.Infrastructure --startup-project src/Todo.WebApi --output-dir Migrations
dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.WebApi
```

## Тестирование

```bash
dotnet test
```

- **Unit-тесты** (`tests/Todo.Tests/Services`, `tests/Todo.Tests/Validators`) — сервисный слой с моками репозитория и маппера, валидаторы FluentValidation.
- **Интеграционные тесты** (`tests/Todo.Tests/Integration`) — полный HTTP-стек через `WebApplicationFactory<Program>` поверх SQLite in-memory (`Data Source=:memory:`), с EF-миграциями, применяемыми тем же кодом, что и в проде.

## Известные ограничения / планы развития

- Optimistic concurrency handling — обработка `DbUpdateException` есть, но concurrency token в модели ещё не добавлен
- Авторизация/аутентификация не реализована — проект пока не предполагает multi-user сценариев

## Мотивация проекта

Переписанная на .NET версия ранее реализованного на Java учебного todo-list проекта — способ продемонстрировать профессиональный рост, с акцентом на архитектурные решения, а не только на функциональность.
