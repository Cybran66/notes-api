# Notes API

Portfolio backend project on ASP.NET Core + PostgreSQL.

It provides:
- CRUD for notes
- title search
- revision history for each note
- seeded demo data
- enhanced Swagger UI with bilingual toggle (RU/EN) and a visual overview panel

## Stack

- C#
- ASP.NET Core Web API (controller-based)
- Entity Framework Core
- PostgreSQL
- Swagger / OpenAPI (Swashbuckle)

## Endpoints

- `GET /api/notes`
- `GET /api/notes/{id}`
- `GET /api/notes/{id}/history`
- `POST /api/notes`
- `PUT /api/notes/{id}`
- `DELETE /api/notes/{id}`
- `GET /api/notes/search?query=...`

## Database setup

Edit `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=notesdb;Username=notesuser;Password=your_password"
}
```

## Run

1. Restore dependencies:
   - `dotnet restore`
2. Restore local tools:
   - `dotnet tool restore`
3. Run:
   - `dotnet run`
4. Open docs:
   - `http://localhost:5139/?lang=ru`
   - `http://localhost:5139/?lang=en`

## Notes

- On startup, migrations are applied automatically.
- SeedData adds demo records if they are missing, so API is never empty in demos.
- Revision history entries are created automatically on create/update/delete operations.
