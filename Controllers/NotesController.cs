using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.DTOs;
using NotesApi.Models;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private const string CreatedAction = "Создана";
    private const string UpdatedAction = "Обновлена";
    private const string DeletedAction = "Удалена";

    private readonly NotesDbContext _context;

    public NotesController(NotesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все заметки.
    /// </summary>
    /// <returns>Список заметок, отсортированный по дате создания (новые сверху).</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
    {
        var notes = await _context.Notes
            .OrderByDescending(note => note.CreatedAt)
            .Select(note => new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                IsArchived = note.IsArchived,
                CreatedAt = note.CreatedAt
            })
            .ToListAsync();

        return Ok(notes);
    }

    /// <summary>
    /// Получить заметку по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заметки.</param>
    /// <returns>Одна заметка или 404, если запись не найдена.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<NoteDto>> GetById(int id)
    {
        var note = await _context.Notes.FindAsync(id);

        if (note is null)
        {
            return NotFound();
        }

        return Ok(ToDto(note));
    }

    /// <summary>
    /// Получить историю изменений заметки.
    /// </summary>
    /// <param name="id">Идентификатор заметки.</param>
    /// <returns>Список ревизий с действиями и временем изменения.</returns>
    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<IEnumerable<NoteRevisionDto>>> GetHistory(int id)
    {
        var history = await _context.NoteRevisions
            .Where(revision => revision.NoteId == id)
            .OrderByDescending(revision => revision.ChangedAt)
            .Select(revision => new NoteRevisionDto
            {
                Id = revision.Id,
                NoteId = revision.NoteId,
                Title = revision.Title,
                Content = revision.Content,
                IsArchived = revision.IsArchived,
                Action = revision.Action,
                ChangedAt = revision.ChangedAt
            })
            .ToListAsync();

        if (history.Count == 0)
        {
            var noteExists = await _context.Notes.AnyAsync(note => note.Id == id);
            if (!noteExists)
            {
                return NotFound();
            }
        }

        return Ok(history);
    }

    /// <summary>
    /// Поиск заметок по заголовку.
    /// </summary>
    /// <param name="query">Поисковая строка для поля Title.</param>
    /// <returns>Подходящие заметки.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<NoteDto>>> Search([FromQuery] string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Параметр query обязателен.");
        }

        var trimmedQuery = query.Trim();

        var notes = await _context.Notes
            .Where(note => EF.Functions.ILike(note.Title, $"%{trimmedQuery}%"))
            .OrderByDescending(note => note.CreatedAt)
            .Select(note => new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                IsArchived = note.IsArchived,
                CreatedAt = note.CreatedAt
            })
            .ToListAsync();

        return Ok(notes);
    }

    /// <summary>
    /// Создать новую заметку.
    /// </summary>
    /// <param name="dto">Данные для создания заметки.</param>
    /// <returns>Созданная заметка.</returns>
    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create(CreateNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
        {
            return BadRequest("Поля Title и Content не могут быть пустыми.");
        }

        var note = new Note
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            IsArchived = dto.IsArchived
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        AddRevision(note, CreatedAction);
        await _context.SaveChangesAsync();

        var result = ToDto(note);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, result);
    }

    /// <summary>
    /// Обновить существующую заметку.
    /// </summary>
    /// <param name="id">Идентификатор заметки.</param>
    /// <param name="dto">Новые данные заметки.</param>
    /// <returns>Код 204 при успешном обновлении.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
        {
            return BadRequest("Поля Title и Content не могут быть пустыми.");
        }

        var note = await _context.Notes.FindAsync(id);

        if (note is null)
        {
            return NotFound();
        }

        note.Title = dto.Title.Trim();
        note.Content = dto.Content.Trim();
        note.IsArchived = dto.IsArchived;

        AddRevision(note, UpdatedAction);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить заметку по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заметки.</param>
    /// <returns>Код 204 при успешном удалении.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _context.Notes.FindAsync(id);

        if (note is null)
        {
            return NotFound();
        }

        AddRevision(note, DeletedAction);
        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static NoteDto ToDto(Note note)
    {
        return new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            IsArchived = note.IsArchived,
            CreatedAt = note.CreatedAt
        };
    }

    private void AddRevision(Note note, string action)
    {
        _context.NoteRevisions.Add(new NoteRevision
        {
            NoteId = note.Id,
            Title = note.Title,
            Content = note.Content,
            IsArchived = note.IsArchived,
            Action = action
        });
    }
}
