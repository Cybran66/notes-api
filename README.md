# Notes API

Небольшой, но реалистичный backend-проект для портфолио стажера: API для заметок на ASP.NET Core + PostgreSQL.

Проект демонстрирует:
- CRUD-операции для заметок
- поиск по заголовку
- историю изменений заметок (ревизии)
- автонаполнение демо-данными
- улучшенный Swagger UI с переключением языка `RU/EN` и визуальным обзорным блоком

## Технологии

- C#
- ASP.NET Core Web API (Controller-based)
- Entity Framework Core
- PostgreSQL
- Swagger / OpenAPI (Swashbuckle)

## Функционал API

- `GET /api/notes` — получить список заметок
- `GET /api/notes/{id}` — получить заметку по id
- `POST /api/notes` — создать заметку
- `PUT /api/notes/{id}` — обновить заметку
- `DELETE /api/notes/{id}` — удалить заметку
- `GET /api/notes/search?query=...` — поиск по `Title`
- `GET /api/notes/{id}/history` — история изменений заметки

## Особенности проекта

### 1. История изменений (Revision History)

Для каждой заметки фиксируются ревизии с действием:
- `Создана`
- `Обновлена`
- `Удалена`

История хранится отдельно в таблице `NoteRevisions` и доступна через endpoint `history`.

### 2. Демо-данные из коробки

При запуске приложение:
1. Применяет миграции автоматически.
2. Проверяет наличие seed-заметок.
3. Добавляет недостающие записи (и ревизии), чтобы API сразу было с данными.

Это позволяет открыть Swagger и сразу увидеть «живой» результат без ручного `POST`.

### 3. Улучшенный Swagger UI

- Корневой адрес документации: `/`
- Переключение языка интерфейса: `?lang=ru` / `?lang=en`
- Визуальный блок с:
  - описанием проекта
  - кнопкой быстрого перехода к `GET /api/notes`
  - сводкой и списком записей, уже лежащих в БД

## Структура проекта

```text
NotesApi/
  Controllers/
    NotesController.cs
  Data/
    NotesDbContext.cs
    SeedData.cs
  DTOs/
    CreateNoteDto.cs
    UpdateNoteDto.cs
    NoteDto.cs
    NoteRevisionDto.cs
  Models/
    Note.cs
    NoteRevision.cs
  Migrations/
    ...
  wwwroot/swagger-ui/
    custom.css
    custom.js
  Program.cs
  appsettings.json
```

## Настройка PostgreSQL

Открой `appsettings.json` и укажи свои параметры:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=notesdb;Username=notesuser;Password=your_password"
}
```

Где:
- `Database` — имя базы
- `Username` — пользователь PostgreSQL
- `Password` — пароль пользователя

## Запуск

1. Восстановить пакеты:
   - `dotnet restore`
2. Восстановить локальные инструменты:
   - `dotnet tool restore`
3. Запустить проект:
   - `dotnet run`

Swagger UI:
- `http://localhost:5139/?lang=ru`
- `http://localhost:5139/?lang=en`

## Быстрая проверка сценария

1. Открыть `GET /api/notes` — проверить, что список не пустой.
2. Создать заметку через `POST /api/notes`.
3. Обновить заметку через `PUT /api/notes/{id}`.
4. Проверить `GET /api/notes/{id}/history` — должны быть ревизии.

## Для резюме (коротко)

Разработал REST API для заметок на ASP.NET Core и PostgreSQL: реализовал CRUD, поиск, историю изменений, миграции EF Core, автонаполнение тестовыми данными и двуязычную Swagger-документацию с кастомным UI.
