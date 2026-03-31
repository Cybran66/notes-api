# 📝 Notes API

Backend-проект на **C# / ASP.NET Core**, реализующий REST API для управления заметками.

Проект демонстрирует базовые принципы разработки серверных приложений:

* работа с HTTP API
* CRUD-операции
* интеграция с базой данных
* структурирование кода

---

## 🚀 Возможности

* Создание заметок
* Получение списка заметок
* Получение заметки по ID
* Обновление заметок
* Удаление заметок
* Поиск заметок по названию
* Архивирование заметок

---

## 🛠️ Технологии

* **C#**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **PostgreSQL**
* **Swagger / OpenAPI**

---

## 📦 Структура проекта

```
NotesApi/
├── Controllers/     # API контроллеры
├── Models/          # Сущности (Note)
├── Dtos/            # DTO объекты
├── Data/            # DbContext
├── appsettings.json # Конфигурация
└── Program.cs       # Точка входа
```

---

## ⚙️ Установка и запуск

### 1. Клонировать репозиторий

```
git clone https://github.com/your-username/notes-api.git
cd notes-api
```

---

### 2. Настроить PostgreSQL

Создайте базу данных:

```sql
CREATE DATABASE notesdb;
```

Создайте пользователя:

```sql
CREATE USER notesuser WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE notesdb TO notesuser;
```

---

### 3. Настроить подключение

Откройте `appsettings.json` и укажите:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=notesdb;Username=notesuser;Password=your_password"
}
```

---

### 4. Применить миграции

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

### 5. Запустить проект

```
dotnet run
```

После запуска открой:

👉 [https://localhost:xxxx/swagger](https://localhost:xxxx/swagger)

---

## 📡 Примеры запросов

### Создание заметки

```
POST /api/notes
```

```json
{
  "title": "Первая заметка",
  "content": "Тестовый текст"
}
```

---

### Получить все заметки

```
GET /api/notes
```

---

### Поиск

```
GET /api/notes/search?query=test
```

---

## 💡 О проекте

Небольшой сервис для управления заметками, реализованный с использованием ASP.NET Core и PostgreSQL.
Проект отражает базовую структуру backend-приложения и демонстрирует работу с REST API и базой данных.

---

## 📈 Возможные улучшения

* Добавить аутентификацию (JWT)
* Реализовать пагинацию
* Добавить логирование
* Docker и деплой

---

## 👨‍💻 Автор

Cybran66
