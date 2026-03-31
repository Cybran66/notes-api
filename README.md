# Notes API (ASP.NET Core + PostgreSQL)

Small portfolio backend project for a junior/intern role: CRUD API for notes with title search.

## Stack

- ASP.NET Core Web API (Controller-based)
- Entity Framework Core
- PostgreSQL
- Swagger (Swashbuckle)

## Endpoints

- `GET /api/notes` - get all notes
- `GET /api/notes/{id}` - get note by id
- `POST /api/notes` - create note
- `PUT /api/notes/{id}` - update note
- `DELETE /api/notes/{id}` - delete note
- `GET /api/notes/search?query=...` - search notes by `Title`

## PostgreSQL connection setup

Edit `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=notes_db;Username=postgres;Password=postgres"
}
```

- `Database` - your database name
- `Username` - your PostgreSQL user
- `Password` - user password

## Run

1. Restore packages:
   - `dotnet restore`
2. Restore local tools:
   - `dotnet tool restore`
3. Apply migrations:
   - `dotnet ef database update`
4. Start API:
   - `dotnet run`
5. Open Swagger UI:
   - `https://localhost:7077/swagger`
