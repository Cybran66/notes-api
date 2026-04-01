using Microsoft.EntityFrameworkCore;
using NotesApi.Models;

namespace NotesApi.Data;

public static class SeedData
{
    private const string CreatedAction = "Создана";
    private const string UpdatedAction = "Обновлена";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<NotesDbContext>();

        await context.Database.MigrateAsync();

        var now = DateTime.UtcNow;
        var sampleNotes = BuildSampleNotes(now);

        var existingTitles = await context.Notes
            .Select(note => note.Title)
            .ToHashSetAsync();

        var notesToInsert = sampleNotes
            .Where(sample => !existingTitles.Contains(sample.Title))
            .Select(sample => new Note
            {
                Title = sample.Title,
                Content = sample.Content,
                IsArchived = sample.IsArchived,
                CreatedAt = sample.CreatedAt
            })
            .ToList();

        if (notesToInsert.Count > 0)
        {
            context.Notes.AddRange(notesToInsert);
            await context.SaveChangesAsync();
        }

        var seedTitles = sampleNotes.Select(sample => sample.Title).ToList();
        var seededNotes = await context.Notes
            .Where(note => seedTitles.Contains(note.Title))
            .ToListAsync();

        if (seededNotes.Count == 0)
        {
            return;
        }

        var seededNoteIds = seededNotes.Select(note => note.Id).ToList();
        var revisionActionsByNote = await context.NoteRevisions
            .Where(revision => seededNoteIds.Contains(revision.NoteId))
            .GroupBy(revision => revision.NoteId)
            .ToDictionaryAsync(
                group => group.Key,
                group => group.Select(revision => revision.Action).ToHashSet());

        var revisionsToInsert = new List<NoteRevision>();

        foreach (var note in seededNotes)
        {
            var actions = revisionActionsByNote.TryGetValue(note.Id, out var actionSet)
                ? actionSet
                : new HashSet<string>();

            if (!actions.Contains(CreatedAction))
            {
                revisionsToInsert.Add(new NoteRevision
                {
                    NoteId = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    IsArchived = note.IsArchived,
                    Action = CreatedAction,
                    ChangedAt = note.CreatedAt.AddMinutes(2)
                });
            }
        }

        var titlesForUpdateRevision = new HashSet<string>
        {
            "Идеи для портфолио",
            "Задачи на неделю",
            "Архивная заметка",
            "Roadmap: backend intern",
            "Final demo note"
        };

        foreach (var note in seededNotes.Where(note => titlesForUpdateRevision.Contains(note.Title)))
        {
            var actions = revisionActionsByNote.TryGetValue(note.Id, out var actionSet)
                ? actionSet
                : new HashSet<string>();

            if (!actions.Contains(UpdatedAction))
            {
                revisionsToInsert.Add(new NoteRevision
                {
                    NoteId = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    IsArchived = note.IsArchived,
                    Action = UpdatedAction,
                    ChangedAt = note.CreatedAt.AddHours(4)
                });
            }
        }

        if (revisionsToInsert.Count > 0)
        {
            context.NoteRevisions.AddRange(revisionsToInsert);
            await context.SaveChangesAsync();
        }
    }

    private static List<SeedNote> BuildSampleNotes(DateTime now)
    {
        return
        [
            new("План изучения C#", "Повторить async/await, LINQ, EF Core и HTTP pipeline в ASP.NET Core.", false, now.AddDays(-24)),
            new("Идеи для портфолио", "Показать историю изменений, красивый Swagger UI и аккуратную документацию.", false, now.AddDays(-23)),
            new("Roadmap: backend intern", "Finish Notes API, add pagination, and deploy demo to a free cloud tier.", false, now.AddDays(-22)),
            new("Задачи на неделю", "1) Подготовить README. 2) Прогнать API через Postman. 3) Обновить резюме.", false, now.AddDays(-21)),
            new("Архивная заметка", "Черновик старой идеи: оставить в архиве для демонстрации фильтрации.", true, now.AddDays(-20)),
            new("Собеседование", "Подготовить ответы по SOLID, REST, миграциям и жизненному циклу запроса.", false, now.AddDays(-19)),
            new("Ошибка EF Core", "Если migration не применяется, проверить connection string и права на схему public.", false, now.AddDays(-18)),
            new("Productivity tips", "Break tasks into small steps and keep commit messages short and descriptive.", false, now.AddDays(-17)),
            new("Набросок пет-проекта", "Добавить теги, архив/восстановление и endpoint истории правок.", false, now.AddDays(-16)),
            new("Learning SQL", "Practice JOIN, GROUP BY, window functions, and index basics every day.", false, now.AddDays(-15)),
            new("Архив: старый план", "Старый учебный план, который больше не используется.", true, now.AddDays(-14)),
            new("Проверка перед пушем", "dotnet build, run smoke checks, then push to GitHub.", false, now.AddDays(-13)),
            new("Refactoring notes", "Keep controller simple, avoid overengineering, extract only useful abstractions.", false, now.AddDays(-12)),
            new("Demo note for Swagger", "This note exists so the API list is never empty on first run.", false, now.AddDays(-11)),
            new("Тест endpoint-ов", "Проверить GET, POST, PUT, DELETE и history в Swagger UI.", false, now.AddDays(-10)),
            new("Подготовка к стажировке", "Собрать 2-3 проекта и описать их в резюме в формате problem-solution.", false, now.AddDays(-9)),
            new("Debug checklist", "Check logs, reproduce issue, isolate bug, fix, verify, write summary.", false, now.AddDays(-8)),
            new("Архив: old backlog", "Old backlog item kept for archived state examples.", true, now.AddDays(-7)),
            new("API polish", "Improve descriptions, add examples, and keep status codes consistent.", false, now.AddDays(-6)),
            new("Notes search ideas", "Search by title now; maybe later add tag and date filters.", false, now.AddDays(-5)),
            new("Code style", "Prefer readability, small methods, and clear naming for interview tasks.", false, now.AddDays(-4)),
            new("Mini retrospective", "What worked well: iterative delivery and visible user feedback.", false, now.AddDays(-3)),
            new("Data seeding", "Seed data helps reviewers see a live API immediately.", false, now.AddDays(-2)),
            new("Final demo note", "Use this note to show history endpoint after an update.", false, now.AddDays(-1))
        ];
    }

    private sealed record SeedNote(string Title, string Content, bool IsArchived, DateTime CreatedAt);
}
